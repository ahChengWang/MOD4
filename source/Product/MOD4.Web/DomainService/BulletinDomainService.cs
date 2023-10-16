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
        private readonly IDepartmentDomainService _departmentDomainService;
        private readonly ICarUXBulletinRepository _carUXBulletinRepository;

        public BulletinDomainService(IUploadDomainService uploadDomainService,
            IAccountDomainService accountDomainService,
            IDepartmentDomainService departmentDomainService,
            ICarUXBulletinRepository carUXBulletinRepository)
        {
            _uploadDomainService = uploadDomainService;
            _accountDomainService = accountDomainService;
            _departmentDomainService = departmentDomainService;
            _carUXBulletinRepository = carUXBulletinRepository;
        }

        public List<BulletinEntity> GetBulletinList(UserEntity userInfo, DateTime? startDate = null, DateTime? endDate = null, string readStatus = "")
        {
            try
            {
                DateTime _nowTime = DateTime.Now;
                List<CarUXBulletinDao> _bulletinList = new List<CarUXBulletinDao>();
                List<CarUXBulletinDetailDao> _bulletinDetailList = new List<CarUXBulletinDetailDao>();
                List<BulletinEntity> _bulletinRep = new List<BulletinEntity>();
                var _defDepartment = _departmentDomainService.GetDeptSectionList();
                
                if (userInfo.Level_id == JobLevelEnum.DL)
                {
                    _bulletinDetailList = _carUXBulletinRepository.SelectDetailByConditions(jobId: userInfo.JobId,readStatus: readStatus);
                    _bulletinList = _carUXBulletinRepository.SelectByConditions(_bulletinDetailList.Select(s => s.BulletinSn).ToList(), startDate: startDate, endDate: endDate);
                    var _authorAccInfoList = _accountDomainService.GetAccountInfo(_bulletinList.Select(s => s.AuthorAccountId).ToList());
                    _bulletinRep.AddRange(from bulletin in _bulletinList
                                          join detail in _bulletinDetailList
                                          on bulletin.SerialNo equals detail.BulletinSn
                                          select new BulletinEntity
                                          {
                                              SerialNo = bulletin.SerialNo,
                                              Subject = bulletin.Subject,
                                              Date = bulletin.Date.ToString("yyyy-MM-dd"),
                                              AuthorName = _authorAccInfoList.FirstOrDefault(f => f.sn == bulletin.AuthorAccountId)?.Name ?? "",
                                              //Content = bulletin.Content,
                                              FileName = bulletin.FileName,
                                              IsNewPost = bulletin.Date >= _nowTime.Date.AddDays(-7),
                                              TargetSections = (from dept in _defDepartment
                                                                join target in bulletin.TargetSections.Split(",").ToList()
                                                                on dept.DeptSn equals Convert.ToInt16(target)
                                                                select dept.DepartmentName).ToList(),
                                              TargetSectionStr = string.Join(" ○ ", (from dept in _defDepartment
                                                                  join target in bulletin.TargetSections.Split(",").ToList()
                                                                  on dept.DeptSn equals Convert.ToInt16(target)
                                                                  select dept.DepartmentName).ToList()),
                                              Status = detail.Status == 1 ? "未讀" : "已讀",
                                              IsNeedUpdate = detail.Status == 1
                                          });
                }
                else
                {
                    _bulletinList = _carUXBulletinRepository.SelectByConditions(_bulletinDetailList.Select(s => s.BulletinSn).ToList(), startDate: startDate, endDate: endDate);
                    var _authorAccInfoList = _accountDomainService.GetAccountInfo(_bulletinList.Select(s => s.AuthorAccountId).ToList());
                    _bulletinRep.AddRange(_bulletinList.Select(s => new BulletinEntity
                    {
                        SerialNo = s.SerialNo,
                        Subject = s.Subject,
                        Date = s.Date.ToString("yyyy-MM-dd"),
                        AuthorName = _authorAccInfoList.FirstOrDefault(f => f.sn == s.AuthorAccountId)?.Name ?? "",
                        //Content = s.Content,
                        //FileName = bulletin.FileName,
                        IsNewPost = s.Date >= _nowTime.Date.AddDays(-7),
                        TargetSections = (from dept in _defDepartment
                                          join target in s.TargetSections.Split(",").ToList()
                                          on dept.DeptSn equals Convert.ToInt16(target)
                                          select dept.DepartmentName).ToList(),
                        Status = "",
                        IsNeedUpdate = false
                    }));
                }

                // 閱讀人數
                var _bulletinDetailRead = _carUXBulletinRepository.SelectDetailByConditions(bulletinSn: _bulletinRep.Select(s => s.SerialNo).ToList()).Where(w => w.Status == 2)
                        .GroupBy(g => g.BulletinSn).Select(data => new
                        {
                            SerialNo = data.Key,
                            Count = data.Count()
                        });
                _bulletinRep.ForEach(f => f.ReadCount = _bulletinDetailRead.FirstOrDefault(detail => detail.SerialNo == f.SerialNo)?.Count ?? 0);

                return _bulletinRep.OrderByDescending(od => od.SerialNo).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<BulletinEntity> GetBulletinByConditions(UserEntity userInfo, string dateRange, string readStatus)
        {
            try
            {
                var _dateRange = dateRange.DetaRangeConvert("yyyy/MM/dd");

                if (_dateRange.startDate == null || _dateRange.endDate == null)
                    throw new Exception("起訖日異常");

                return GetBulletinList(userInfo,startDate: _dateRange.startDate,endDate: _dateRange.endDate, readStatus);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public BulletinEntity GetBulletinDetail(int bulletinSn)
        {
            try
            {
                DateTime _nowTime = DateTime.Now;
                List<CarUXBulletinDetailDao> _bulletinDetailList = new List<CarUXBulletinDetailDao>();

                CarUXBulletinDao _bulletin = _carUXBulletinRepository.SelectByConditions(snList: new List<int> { bulletinSn }).FirstOrDefault();

                if (_bulletin == null)
                    return null;

                var _authorAccInfoList = _accountDomainService.GetAccountInfo(new List<int> { _bulletin.AuthorAccountId });

                BulletinEntity _bulletinRep = new BulletinEntity
                {
                    SerialNo = _bulletin.SerialNo,
                    Subject = _bulletin.Subject,
                    Date = _bulletin.Date.ToString("yyyy-MM-dd"),
                    AuthorName = _authorAccInfoList.FirstOrDefault(f => f.sn == _bulletin.AuthorAccountId)?.Name ?? "",
                    Content = _bulletin.Content,
                    FileName = _bulletin.FileName,
                };

                return _bulletinRep;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string Create(BulletinCreateEntity createEntity, UserEntity userEntity)
        {
            try
            {
                string _uplRes = "";
                DateTime _nowTime = DateTime.Now;

                // 取得公告人員
                var _destSectionEmpList = _accountDomainService.GetAccInfoListByDepartment(createEntity.Target.Split(",").Select(sec => Convert.ToInt32(sec)).ToList())
                        .Where(w => w.Level_id == JobLevelEnum.DL && new List<RoleEnum> { RoleEnum.PM, RoleEnum.TechnicalAssistant, RoleEnum.TeamLeader, RoleEnum.Foreman }.Contains(w.RoleId));

                CarUXBulletinDao _bulletin = new CarUXBulletinDao
                {
                    Subject = createEntity.Subject,
                    OrderNo = Convert.ToInt64(_nowTime.ToString("yyMMddHHmmss")),
                    Content = createEntity.Content,
                    Date = _nowTime.Date,
                    AuthorAccountId = userEntity.sn,
                    TargetSections = createEntity.Target,
                    FileName = createEntity.UploadFile.FileName,
                    FilePath = $"\\{userEntity.JobId}\\{_nowTime.ToString("yyMMdd")}",
                    CreateUser = userEntity.Name,
                    CreateTime = _nowTime
                };

                List<CarUXBulletinDetailDao> _bulletinDetail = _destSectionEmpList
                    .Select(empAcc => new CarUXBulletinDetailDao
                    {
                        JobId = empAcc.JobId,
                        Status = 1
                    }).ToList();


                // FTP 上傳
                _ftpService.FTP_Upload(createEntity.UploadFile, $"FTP_Bulletin/{userEntity.JobId}/{_nowTime.ToString("yyMMdd")}", createEntity.UploadFile.FileName, false, "");

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
                    bool _insDetailRes;
                    int _newSn = 0;

                    _insRes = _carUXBulletinRepository.Insert(_bulletin) == 1;
                    if (_insRes)
                    {
                        _newSn = _carUXBulletinRepository.SelectByConditions(orderNo: _bulletin.OrderNo).FirstOrDefault()?.SerialNo ?? 0;

                        if (_newSn != 0)
                        {
                            _bulletinDetail.ForEach(bulletin => bulletin.BulletinSn = _newSn);

                            _insDetailRes = _carUXBulletinRepository.InsertDetail(_bulletinDetail) == _bulletinDetail.Count;

                            if (_insRes)
                                _scope.Complete();
                            else
                                _uplRes = "新增公告明細異常";
                        }
                    }
                    else
                        _uplRes = "新增公告主檔異常";

                }

                return _uplRes;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string UpdateDetail(int bulletinSn, UserEntity userEntity)
        {
            try
            {
                DateTime _nowTime = DateTime.Now;
                string _result = "";

                CarUXBulletinDao _bulletin = _carUXBulletinRepository.SelectByConditions(snList: new List<int> { bulletinSn }).FirstOrDefault();
                CarUXBulletinDetailDao _bulletinDetail = _carUXBulletinRepository.SelectDetailByConditions(new List<int> { bulletinSn }, jobId: userEntity.JobId).FirstOrDefault();

                if (_bulletin == null || _bulletinDetail == null)
                    return "error 查無公告";

                CarUXBulletinDetailDao _updBulletinDetail = new CarUXBulletinDetailDao
                {
                    BulletinSn = bulletinSn,
                    JobId = userEntity.JobId,
                    ReadDate = _nowTime,
                    Status = 2,
                    UpdateTime = _nowTime
                };

                using (TransactionScope _scope = new TransactionScope())
                {
                    bool _updRes = false;
                    _updRes = _carUXBulletinRepository.UpdateDetail(_updBulletinDetail) == 1;

                    if (_updRes)
                    {
                        _scope.Complete();
                    }
                }

                return _result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Tuple<bool, string, string> Download(int bulletinSn, UserEntity userEntity)
        {
            try
            {
                DateTime _nowTime = DateTime.Now;
                Tuple<bool, string, string> _response = new Tuple<bool, string, string>(false, "", "");

                CarUXBulletinDao _bulletin = _carUXBulletinRepository.SelectByConditions(snList: new List<int> { bulletinSn }).FirstOrDefault();

                if (_bulletin == null)
                    return _response;

                var fileStr = _ftpService.FTP_Download($"FTP_Bulletin{_bulletin.FilePath}", _bulletin.FileName);

                _response = new Tuple<bool, string, string>(true, fileStr, _bulletin.FileName);

                return _response;
            }
            catch (Exception ex)
            {
                throw ex;
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
    }
}
