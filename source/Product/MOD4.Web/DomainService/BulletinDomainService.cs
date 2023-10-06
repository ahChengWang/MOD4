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
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.DomainService
{
    public class BulletinDomainService : BaseDomainService, IBulletinDomainService
    {
        private readonly IUploadDomainService _uploadDomainService;
        private readonly IAccountDomainService _accountDomainService;
        private readonly ICarUXBulletinRepository _carUXBulletinRepository;

        public BulletinDomainService(IUploadDomainService uploadDomainService,
            IAccountDomainService accountDomainService,
            ICarUXBulletinRepository carUXBulletinRepository)
        {
            _uploadDomainService = uploadDomainService;
            _accountDomainService = accountDomainService;
            _carUXBulletinRepository = carUXBulletinRepository;
        }

        public string GetBulletinList(int accountId)
        {
            try
            {

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

        public string Create(BulletinCreateEntity createEntity, UserEntity userEntity)
        {
            try
            {
                string _uplRes = "";
                DateTime _nowTime = DateTime.Now;

                // 取得公告人員
                var _destSectionEmpList = _accountDomainService.GetAccInfoListByDepartment(createEntity.Target.Split(",").Select(sec => Convert.ToInt32(sec)).ToList());

                CarUXBulletinDao _bulletin = new CarUXBulletinDao
                {
                    Subject = createEntity.Subject,
                    Content = createEntity.Content,
                    Date = _nowTime.Date,
                    AuthorAccountId = userEntity.sn,
                    TargetSections = createEntity.Target,
                    CreateUser = userEntity.Name,
                    CreateTime = _nowTime
                };

                List<CarUXBulletinDetailDao> _bulletinDetail = _destSectionEmpList
                    .Select(empAcc => new CarUXBulletinDetailDao 
                    {
                        AccountId = empAcc.sn,
                        Status = 0
                    }).ToList();


                //if (string.IsNullOrEmpty(createEntity.Subject) || string.IsNullOrEmpty(createEntity.Content) || string.IsNullOrEmpty(createEntity.Target))
                //    throw new Exception("欄未必填");

                //string[] _fileNameAry = createEntity.UploadFile.FileName.Split(".");
                //var fileName = Path.GetFileName($"{_fileNameAry[0]}_{Guid.NewGuid().ToString("N").Substring(0, 4)}.{_fileNameAry[1]}");

                //_ftpService.FTP_Upload(createEntity.UploadFile, "FTP_MPS", fileName, false, "");

                //string[] _nameAry = createEntity.UploadFile.FileName.Split(".");

                //MPSUploadHistoryDao _mpsUplHisDao = new MPSUploadHistoryDao
                //{
                //    FileName = fileName,
                //    UploadTime = DateTime.Now,
                //    UploadUser = userEntity.Name
                //};

                using (TransactionScope _scope = new TransactionScope())
                {
                    bool _insRes;

                    _insRes = _carUXBulletinRepository.Insert(_bulletin) == 1;

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
                MPSUploadHistoryDao _mpsDao = new MPSUploadHistoryDao();

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
    }
}
