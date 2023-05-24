using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MOD4.Web.DomainService.Entity;
using System;
using System.IO;
using System.Linq;
using System.Net;

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

        public void FTP_Upload(IFormFile uploadFile, string parentFolder, string fileName)
        {
            string[] _fNameAry = parentFolder.Split("/");
            string _fullPath = "";

            foreach (string fName in _fNameAry)
            {
                _fullPath += fName + "/";
                CreateFolder(_fullPath);
            }

            var request = (FtpWebRequest)WebRequest.Create($"{_serverIP}{parentFolder}");
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

        public string FTP_Download(string parentFolder, string fileName)
        {
            var request = (FtpWebRequest)WebRequest.Create($"{_serverIP}{parentFolder}/{fileName}");
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
