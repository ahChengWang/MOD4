using Microsoft.AspNetCore.Http;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using System;
using System.IO;
using Ionic.Zip;
using Utility.Helper;
using System.Net;
using MOD4.Web.Repostory.Dao;
using MOD4.Web.Repostory;
using System.Transactions;

namespace MOD4.Web.DomainService
{
    public class ExtensionDomainService : BaseDomainService, IExtensionDomainService
    {
        private readonly IUploadDomainService _uploadDomainService;
        private readonly IMPSUploadHistoryRepository _mpsUploadHistoryRepository;

        public ExtensionDomainService(IUploadDomainService uploadDomainService,
            IMPSUploadHistoryRepository mpsUploadHistoryRepository)
        {
            _uploadDomainService = uploadDomainService;
            _mpsUploadHistoryRepository = mpsUploadHistoryRepository;
        }

        public string Upload(string jobId, ApplyAreaEnum applyAreaId, int itemId, IFormFile uploadFile, UserEntity userEntity)
        {
            try
            {
                var _url = "D:\\";
                var _folder = $@"CertifiedUpload\{jobId}\{applyAreaId.GetDescription()}\{itemId}";

                if (!Directory.Exists($@"{_url}\{_folder}"))
                {
                    Directory.CreateDirectory($@"{_url}\{_folder}");
                }

                string[] _nameArray = uploadFile.FileName.Split(".");

                var path = $@"{_url}\{_folder}\{_nameArray[0]}_{DateTime.Now.ToString("yyyyMMdd")}.{_nameArray[1]}";

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    uploadFile.CopyTo(stream);
                }

                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public (byte[], string) Download(string jobId, ApplyAreaEnum applyAreaId, int itemId, UserEntity userEntity)
        {
            try
            {
                var _url = "D:\\";
                var _folder = $@"CertifiedUpload\{jobId}\{applyAreaId.GetDescription()}\{itemId}";
                string[] _allFiles = Directory.GetFiles($@"{_url}\{_folder}");
                var bytes = default(byte[]);

                if (!Directory.Exists($@"{_url}\{_folder}") || _allFiles.Length == 0)
                {
                    return (null, "人員無相關上傳檔案");
                }

                using (var zip = new ZipFile())
                {
                    //zip.Password = "P@ssW0rd";

                    foreach (var file in _allFiles)
                    {
                        string[] _pathArray = file.Split("\\");
                        zip.AddEntry(_pathArray[_pathArray.Length - 1], new byte[] { });
                    }

                    using (var ms = new MemoryStream())
                    {
                        zip.Save(ms);
                        bytes = ms.ToArray();
                    }
                }

                return (bytes, "");
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }


        #region === MPS ===

        public string MPSUpload(IFormFile uploadFile, UserEntity userEntity)
        {
            try
            {
                string _uplRes = "";
                string[] _fileNameAry = uploadFile.FileName.Split(".");
                var fileName = Path.GetFileName($"{_fileNameAry[0]}_{Guid.NewGuid().ToString("N").Substring(0, 4)}.{_fileNameAry[1]}");

                _ftpService.FTP_Upload(uploadFile, "FTP_MPS", fileName);

                string[] _nameAry = uploadFile.FileName.Split(".");

                MPSUploadHistoryDao _mpsUplHisDao = new MPSUploadHistoryDao
                {
                    FileName = fileName,
                    UploadTime = DateTime.Now,
                    UploadUser = userEntity.Name
                };

                using (TransactionScope _scope = new TransactionScope())
                {
                    bool _insRes;

                    _insRes = _mpsUploadHistoryRepository.Insert(_mpsUplHisDao) == 1;

                    if (_insRes)
                        _scope.Complete();
                    else
                        _uplRes = "上傳記錄新增異常";
                }

                return _uplRes;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public (bool, string, string) Download()
        {
            try
            {
                MPSUploadHistoryDao _mpsDao = _mpsUploadHistoryRepository.SelectLatest();

                if (_mpsDao == null)
                    return (true, "", "");

                var _fileStr = _ftpService.FTP_Download("FTP_MPS", _mpsDao.FileName);

                return (true, _fileStr, _mpsDao.FileName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
