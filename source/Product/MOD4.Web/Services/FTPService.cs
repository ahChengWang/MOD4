using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MOD4.Web.DomainService.Entity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace MOD4.Web
{
    public class FTPService
    {
        private static string _serverIP = string.Empty;
        private static int _port = 21;
        private static string _account = string.Empty;
        private static string _password = string.Empty;

        public FTPService(IConfiguration config)
        {
            _serverIP = config.GetSection("FTPServer").GetValue<string>("ServerIP");
            _port = config.GetSection("FTPServer").GetValue<int>("Port");
            _account = config.GetSection("FTPServer").GetValue<string>("Account");
            _password = config.GetSection("FTPServer").GetValue<string>("Password");
        }

        public FTPService()
        {
        }

        public void FTP_Upload(IFormFile uploadFile, string parentFolder, string fileName, bool isProcessFile, string fullFilePath)
        {
            string[] _fNameAry = parentFolder.Split("/");
            string _fullPath = "";

            foreach (string fName in _fNameAry)
            {
                _fullPath += fName + "/";
                CreateFolder(_fullPath);
            }

            if (isProcessFile)
                using (WebClient client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(_account, _password);
                    client.UploadFile($"{_serverIP}{parentFolder}/{fileName}", WebRequestMethods.Ftp.UploadFile, fullFilePath);
                }
            else
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"{_serverIP}{parentFolder}");
                request = (FtpWebRequest)WebRequest.Create($"{_serverIP}{parentFolder}/{fileName}");
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.KeepAlive = true;
                request.UseBinary = true;
                request.Credentials = new NetworkCredential(_account, _password);

                //上傳檔案
                using (Stream requestStream = request.GetRequestStream())
                {
                    uploadFile.CopyTo(requestStream);
                }

                //上傳成功 (response.StatusCode = FtpStatusCode.ClosingData) // 226 successful
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            }
        }

        public string FTP_Download(string parentFolder, string filePathName)
        {
            var request = (FtpWebRequest)WebRequest.Create($"{_serverIP}{parentFolder}/{filePathName}");
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(_account, _password);

            string _resStr = "";

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    _resStr = Path.GetTempFileName();

                    using (FileStream fs = new FileStream(_resStr, FileMode.Create))
                    {
                        stream.CopyTo(fs);
                    }
                }
            }

            return _resStr;
        }

        public string GetFTPLatestFile(string parentFolder)
        {
            List<(string, DateTime)> _ftpFileList = new List<(string, DateTime)>();
            string pattern = @"^(\d+-\d+-\d+\s+\d+:\d+(?:AM|PM))\s+(<DIR>|\d+)\s+(.+)$";
            Regex regex = new Regex(pattern);

            var request = (FtpWebRequest)WebRequest.Create($"{_serverIP}{parentFolder}");
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            request.Credentials = new NetworkCredential(_account, _password);
            string _resFileName = "";

            using (StreamReader strReader = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                while (!strReader.EndOfStream)
                {
                    string _line = strReader.ReadLine(); 
                    
                    Match match = regex.Match(_line);
                    string s = match.Groups[1].Value;
                    DateTime modified = DateTime.ParseExact(s, "MM-dd-yy  hh:mmtt", CultureInfo.InvariantCulture);
                    string name = match.Groups[3].Value;
                    _ftpFileList.Add((name, modified));
                }

                //using (Stream stream = response.GetResponseStream())
                //{
                //    _resStr = Path.GetTempFileName();

                //    using (FileStream fs = new FileStream(_resStr, FileMode.Create))
                //    {
                //        stream.CopyTo(fs);
                //    }
                //}
            }

            if (_ftpFileList.Any())
            {
                _ftpFileList = _ftpFileList.OrderByDescending(o => o.Item2).ToList();

                _resFileName = _ftpFileList[0].Item1;

                // 只保存近3日檔案
                if (_ftpFileList.Count > 3)
                    DeleteFTPFile(parentFolder, _ftpFileList.Select(file => file.Item1).Skip(3).ToList());
            }

            return _resFileName;
        }

        private string DeleteFTPFile(string parentFolder, List<string> fileList)
        {
            string _delRes = "";

            foreach (string fileName in fileList)
            {
                var request = (FtpWebRequest)WebRequest.Create($"{_serverIP}{parentFolder}/{fileName}");
                request.Method = WebRequestMethods.Ftp.DeleteFile;
                request.Credentials = new NetworkCredential(_account, _password);

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    _delRes += response.StatusDescription;
                }
            }

            return _delRes;
        }

        /// <summary>
        /// 確認 FTP 資料夾存在與否
        /// </summary>
        /// <param name="parentFolder">檔案上層資料夾</param>
        public void CreateFolder(string parentFolder)
        {
            try
            {
                var request = (FtpWebRequest)WebRequest.Create($"{_serverIP}{parentFolder}");
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                request.UseBinary = true;
                request.Credentials = new NetworkCredential(_account, _password);
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream _ftpStream = response.GetResponseStream();
                _ftpStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                return;
            }
        }
    }
}
