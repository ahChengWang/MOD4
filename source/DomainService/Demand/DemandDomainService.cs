using Microsoft.AspNetCore.Http;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using MOD4.Web.Extension.Interface;
using MOD4.Web.Repostory;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Transactions;
using static System.Net.WebRequestMethods;

namespace MOD4.Web.DomainService.Demand
{
    public class DemandDomainService : BaseDomainService, IDemandDomainService
    {
        private readonly IDemandsRepository _demandsRepository;
        private readonly IUploadDomainService _uploadDomainService;
        private readonly IMAppDomainService _mappDomainService;
        private readonly IAccountDomainService _accountDomainService;
        private readonly IDemadStatusFactory _demadStatusFactory;

        public DemandDomainService(IDemandsRepository demandsRepository,
            IUploadDomainService uploadDomainService,
            IMAppDomainService mappDomainService,
            IAccountDomainService accountDomainService,
            IDemadStatusFactory demadStatusFactory)
        {
            _demandsRepository = demandsRepository;
            _uploadDomainService = uploadDomainService;
            _mappDomainService = mappDomainService;
            _accountDomainService = accountDomainService;
            _demadStatusFactory = demadStatusFactory;
        }

        public List<DemandEntity> GetDemands(UserEntity userEntity,
            string sn = null, string dateStart = null, string dateEnd = null, string categoryId = null, string statusId = "1,2,3,6", string kw = "")
        {
            try
            {
                var _nowTime = DateTime.Now;
                DateTime? _dateStart = string.IsNullOrEmpty(dateStart) ? (DateTime?)null : DateTime.Parse(dateStart);
                DateTime? _dateEnd = string.IsNullOrEmpty(dateEnd) ? (DateTime?)null : DateTime.Parse(dateEnd).AddDays(1).AddSeconds(-1);

                return _demandsRepository.SelectByConditions(
                    dateStart: _dateStart,
                    dateEnd: _dateEnd,
                    orderSn: sn,
                    categoryArray: string.IsNullOrEmpty(categoryId) ? null : categoryId.Split(","),
                    statusArray: string.IsNullOrEmpty(statusId) ? null : statusId.Split(","),
                    kw: kw)
                    .Select(s => new DemandEntity
                    {
                        OrderSn = s.orderSn,
                        OrderNo = s.orderNo,
                        CategoryId = s.categoryId,
                        StatusId = s.statusId,
                        Subject = s.subject,
                        Content = s.content,
                        Applicant = s.applicant,
                        JobNo = s.jobNo,
                        UploadFiles = s.uploadFiles,
                        CreateUser = s.createUser,
                        CreateTime = s.createTime,
                        UpdateUser = s.updateUser,
                        UpdateTime = s.updateTime,
                        UserEditable = s.createUser == userEntity.Account &&
                            (s.statusId == DemandStatusEnum.Rejected || s.statusId == DemandStatusEnum.Verify) &&
                            userEntity.UserMenuPermissionList.CheckPermission(MenuEnum.Demand, PermissionEnum.Update)
                    }).OrderByDescending(ob => ob.CreateTime).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public DemandEntity GetDemandDetail(int sn, UserEntity userEntity, string orderId = "")
        {
            DemandsDao _demandDao = _demandsRepository.SelectDetail(sn, orderId);

            if (userEntity != null &&
                (_demandDao.statusId == DemandStatusEnum.Pending || _demandDao.statusId == DemandStatusEnum.Processing || _demandDao.statusId == DemandStatusEnum.Completed) &&
                !userEntity.UserMenuPermissionList.CheckPermission(MenuEnum.Demand, PermissionEnum.Management))
                throw new Exception("您的申請單已更新, 請回主頁確認");

            DemandEntity _result = new DemandEntity
            {
                OrderSn = _demandDao.orderSn,
                OrderNo = _demandDao.orderNo,
                CategoryId = _demandDao.categoryId,
                Category = _demandDao.categoryId.GetDescription(),
                StatusId = _demandDao.statusId,
                Status = _demandDao.statusId.GetDescription(),
                Subject = _demandDao.subject,
                Content = _demandDao.content,
                Applicant = _demandDao.applicant,
                JobNo = _demandDao.jobNo,
                CreateUser = _demandDao.createUser,
                CreateTime = _demandDao.createTime,
                CreateTimeStr = _demandDao.createTime.ToString("yyyy-MM-dd"),
                RejectReason = string.IsNullOrEmpty(_demandDao.rejectReason) ? "" : _demandDao.rejectReason,
                Remark = string.IsNullOrEmpty(_demandDao.remark) ? "" : _demandDao.remark
            };

            var fileArray = _demandDao.uploadFiles.Split(",");

            if (fileArray != null && fileArray.Any())
            {
                _result.UploadFile1 = fileArray[0] ?? "";
                _result.UploadFile2 = fileArray.Length > 1 ? fileArray[1] : "";
                _result.UploadFile3 = fileArray.Length > 2 ? fileArray[2] : "";
            }

            fileArray = _demandDao.completeFiles?.Split(",") ?? new string[0];

            if (fileArray != null && fileArray.Any())
            {
                _result.CompleteUploadFile1 = fileArray[0] ?? "";
                _result.CompleteUploadFile2 = fileArray.Length > 1 ? fileArray[1] : "";
                _result.CompleteUploadFile3 = fileArray.Length > 2 ? fileArray[2] : "";
            }

            return _result;
        }

        public (FileStream, string) GetDownFileStr(int sn, int typeId, int fileNo)
        {
            DemandsDao _demandDao = _demandsRepository.SelectDetail(sn);

            if (typeId != 1 && typeId != 2)
                return (null, "參數錯誤");
            else if (typeId == 1 && string.IsNullOrEmpty(_demandDao.uploadFiles) ||
                     typeId == 2 && string.IsNullOrEmpty(_demandDao.completeFiles))
                return (null, "查無上傳檔");

            switch (typeId)
            {
                case 1:
                    string[] fileArray = _demandDao.uploadFiles.Split(",");
                    return (new FileStream($@"{_uploadDomainService.GetFileServerUrl()}\upload\{_demandDao.createUser}\{_demandDao.createTime.ToString("yyMMdd")}\{fileArray[fileNo]}",
                        FileMode.Open, FileAccess.Read, FileShare.Read)
                        , fileArray[fileNo]);
                case 2:
                    string[] completeFilesArray = _demandDao.completeFiles.Split(",");
                    return (new FileStream($@"{_uploadDomainService.GetFileServerUrl()}\upload\{_demandDao.updateUser}\{_demandDao.updateTime.ToString("yyMMdd")}\{completeFilesArray[fileNo]}",
                        FileMode.Open, FileAccess.Read, FileShare.Read)
                        , completeFilesArray[fileNo]);
                default:
                    return (null, "查無上傳檔");
            }
        }

        public (bool, string) InsertDemand(DemandEntity insertEntity, UserEntity userEntity)
        {
            try
            {
                var _nowTime = DateTime.Now;
                var _fileNameStr = "";
                var _insResponse = "";
                var _demandOrderSn = 0;

                _fileNameStr = DoUploadFiles(insertEntity.UploadFileList ?? new List<IFormFile>(), userEntity.Account, _nowTime, _nowTime);

                DemandsDao _insDemandsDao = new DemandsDao
                {
                    orderNo = $"DE{_nowTime.ToString("yyMMddHHmmss")}",
                    categoryId = insertEntity.CategoryId,
                    statusId = DemandStatusEnum.Pending,
                    subject = insertEntity.Subject,
                    content = insertEntity.Content,
                    applicant = insertEntity.Applicant,
                    jobNo = insertEntity.JobNo,
                    uploadFiles = _fileNameStr,
                    createUser = userEntity.Account,
                    createTime = _nowTime,
                    updateUser = "",
                    updateTime = _nowTime,
                    isCancel = false
                };

                using (var scope = new TransactionScope())
                {
                    var _insResult = false;

                    _insResult = _demandsRepository.Insert(_insDemandsDao) == 1;
                    _demandOrderSn = _demandsRepository.SelectByConditions(null, null, orderNo: _insDemandsDao.orderNo).FirstOrDefault().orderSn;

                    if (_insResult)
                    {
                        //CatchHelper.Delete(new string[] { $"Eq_Edit:{editEntity.sn}" });
                        scope.Complete();
                    }
                    else
                        _insResponse = "新增失敗";
                }

                // 發送MApp & mail 給作業人員
                if (_insResponse == "")
                {
                    _mappDomainService.SendMsgToOneAsync($"新需求申請單  主旨: {_insDemandsDao.subject}, 申請人: {_insDemandsDao.applicant}", "253425");
                    _mailServer.Send(new MailEntity
                    {
                        To = "WEITING.GUO@INNOLUX.COM",
                        Subject = $"新需求申請單 - 申請人: {_insDemandsDao.applicant}",
                        Content = "<br /><br />" +
                        "您有新需求申請單</a>， <br /><br />" +
                        $"需求單連結：<a href='http://10.54.215.210/MOD4/Demand/Edit?sn={_demandOrderSn}&orderId={_insDemandsDao.orderNo}' target='_blank'>" + _insDemandsDao.orderNo + "</a>"
                    });
                }


                return string.IsNullOrEmpty(_insResponse)
                    ? (true, $"需求單:{_insDemandsDao.orderNo} \n待評估")
                    : (false, _insResponse);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public (bool, string) UpdateDemand(DemandEntity updEntity, UserEntity userEntity)
        {
            try
            {
                var _nowTime = DateTime.Now;
                var _url = _uploadDomainService.GetFileServerUrl();
                var _folder = $@"upload\{userEntity.Account}\{_nowTime.ToString("yyMMdd")}";
                //var _fileNameStr = "";


                DemandsDao _oldDemand = _demandsRepository.SelectDetail(updEntity.OrderSn, updEntity.OrderNo);

                DemandsDao _updDemandsDao = new DemandsDao
                {
                    orderSn = updEntity.OrderSn,
                    orderNo = updEntity.OrderNo,
                    statusId = updEntity.StatusId,
                    subject = string.IsNullOrEmpty(updEntity.Subject) ? _oldDemand.subject : updEntity.Subject,
                    content = updEntity.Content,
                    updateUser = userEntity.Account,
                    updateTime = _nowTime
                };

                // 取得更新狀態 update flow
                var _demandProcess = _demadStatusFactory.GetFlow(updEntity.StatusId)(new DemandFlowEntity
                {
                    InEntity = updEntity,
                    UserInfo = userEntity,
                    OldDemandOrder = _oldDemand,
                    UpdateDemandOrder = _updDemandsDao,
                    DemandsRepository = _demandsRepository,
                    AccountDomainService = _accountDomainService,
                    MAppDomainService = _mappDomainService,
                    MailService = _mailServer,
                    UploadUrl = _uploadDomainService.GetFileServerUrl()
                });

                return _demandProcess;

                /*
                // 剔退
                if (_oldDemand.statusId == DemandStatusEnum.Pending && updEntity.StatusId == DemandStatusEnum.Rejected)
                {
                    _updDemandsDao.rejectReason = updEntity.RejectReason ?? "";
                    //_mappDomainService.SendMsgToOneAsync(_oldDemand.createUser);
                    return UpdateToReject(_updDemandsDao, _oldDemand.createUser);
                }
                // 進行
                else if (_oldDemand.statusId == DemandStatusEnum.Pending && updEntity.StatusId == DemandStatusEnum.Processing)
                    return UpdateToProcess(_updDemandsDao);
                // 進行 => 完成
                else if (_oldDemand.statusId == DemandStatusEnum.Processing && updEntity.StatusId == DemandStatusEnum.Completed)
                {
                    // mgmt 檔案上傳
                    _fileNameStr = DoUploadFiles(updEntity.UploadFileList ?? new List<IFormFile>(), userEntity.Account, _nowTime, _nowTime);
                    _updDemandsDao.completeFiles = _fileNameStr;
                    _updDemandsDao.remark = updEntity.Remark;
                    return UpdateToCompleted(_updDemandsDao, "");
                }
                // 待確認
                else if (_oldDemand.statusId == DemandStatusEnum.Processing && updEntity.StatusId == DemandStatusEnum.Verify)
                {
                    // mgmt 檔案上傳
                    _fileNameStr = DoUploadFiles(updEntity.UploadFileList ?? new List<IFormFile>(), userEntity.Account, _nowTime, _nowTime);
                    _updDemandsDao.completeFiles = _fileNameStr;
                    _updDemandsDao.remark = updEntity.Remark;
                    return UpdateToCompleted(_updDemandsDao, _oldDemand.createUser);
                }
                // user 重送 剔退單
                else if (_oldDemand.statusId == DemandStatusEnum.Rejected && updEntity.StatusId == DemandStatusEnum.Pending)
                {
                    _updDemandsDao.categoryId = updEntity.CategoryId;
                    _updDemandsDao.subject = updEntity.Subject;
                    _updDemandsDao.content = updEntity.Content;
                    _updDemandsDao.applicant = updEntity.Applicant;
                    _updDemandsDao.jobNo = updEntity.JobNo;
                    // user 檔案上傳到訂單建立時的目錄
                    _fileNameStr = DoUploadFiles(updEntity.UploadFileList ?? new List<IFormFile>(), userEntity.Account, _nowTime, _oldDemand.createTime);
                    _updDemandsDao.uploadFiles = _fileNameStr;
                    return UpdateToPending(_updDemandsDao);
                }
                // user 作廢需求單
                else if (_oldDemand.statusId == DemandStatusEnum.Rejected && updEntity.StatusId == DemandStatusEnum.Cancel)
                {
                    _updDemandsDao.isCancel = true;
                    return UpdateToCancel(_updDemandsDao);
                }
                // 待確認 => 完成
                else if (_oldDemand.statusId == DemandStatusEnum.Verify && updEntity.StatusId == DemandStatusEnum.Completed)
                {
                    // mgmt 檔案上傳
                    //_fileNameStr = DoUploadFiles(updEntity.UploadFileList ?? new List<IFormFile>(), userEntity.Account, _nowTime, _nowTime);
                    _updDemandsDao.completeFiles = _oldDemand.completeFiles;
                    _updDemandsDao.remark = _oldDemand.remark;
                    return UpdateToCompleted(_updDemandsDao, _oldDemand.createUser);
                }
                // 待確認中編輯 or 完成後編輯
                else if (_oldDemand.statusId == updEntity.StatusId)
                {
                    // mgmt 檔案上傳
                    _fileNameStr = DoUploadFiles(updEntity.UploadFileList ?? new List<IFormFile>(), userEntity.Account, _nowTime, _nowTime);
                    _updDemandsDao.completeFiles = _fileNameStr;
                    _updDemandsDao.remark = updEntity.Remark;
                    return UpdateToCompleted(_updDemandsDao, "");
                }
                else
                    return (false, "需求單狀態異常");
                */
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private string DoUploadFiles(List<IFormFile> uploadFiles, string userAcc, DateTime nowTime, DateTime folderTime)
        {
            var _url = _uploadDomainService.GetFileServerUrl();
            var _folder = $@"upload\{userAcc}\{folderTime.ToString("yyMMdd")}";
            var _fileNameStr = "";

            if (!Directory.Exists($@"{_url}\{_folder}"))
            {
                Directory.CreateDirectory($@"{_url}\{_folder}");
            }

            foreach (var file in uploadFiles)
            {
                if (file.Length > 0)
                {
                    var _fileArray = file.FileName.Split(".");

                    string _newFileName = "";

                    for (int i = 0; i < _fileArray.Length; i++)
                    {
                        _newFileName += i == _fileArray.Length - 2 ? $"{_fileArray[i]}_{nowTime.ToString("ffff")}." : _fileArray[i];
                    }

                    var path = $@"{_url}\{_folder}\{_newFileName}";

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    _fileNameStr += _newFileName + ",";
                }
            }

            // 未來要調整成 FTP
            //string url = "ftp://10.54.212.193/M6CFM-20211109/layout/test.png";
            //FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
            //request.Credentials = new NetworkCredential("ftpuser", "cimeng0@");
            //request.Method = WebRequestMethods.Ftp.UploadFile;

            //using (Stream ftpStream = request.GetRequestStream())
            //{
            //    insertEntity.UploadFileList[0].CopyTo(ftpStream);
            //}

            // 移除結尾逗號
            if (_fileNameStr.Length > 0)
                _fileNameStr = _fileNameStr.Remove(_fileNameStr.Length - 1, 1);

            return _fileNameStr;
        }

        private (bool, string) UpdateToPending(DemandsDao updDao)
        {
            using (var scope = new TransactionScope())
            {
                var _insResult = false;

                _insResult = _demandsRepository.UpdateToPending(updDao) == 1;

                if (_insResult)
                {
                    scope.Complete();
                    return (true, $"需求單:{updDao.orderNo} \n{updDao.statusId.GetDescription()}");
                }
                else
                    return (false, "更新失敗");
            }
        }

        private (bool, string) UpdateToProcess(DemandsDao updDao)
        {
            using (var scope = new TransactionScope())
            {
                var _insResult = false;

                _insResult = _demandsRepository.UpdateToProcess(updDao) == 1;

                if (_insResult)
                {
                    scope.Complete();
                    return (true, $"需求單:{updDao.orderNo} \n{updDao.statusId.GetDescription()}");
                }
                else
                    return (false, "更新失敗");
            }
        }

        private (bool, string) UpdateToReject(DemandsDao updDao, string createAccount)
        {
            (bool, string) _updateRes = (false, "");

            using (var scope = new TransactionScope())
            {
                var _insResult = false;

                _insResult = _demandsRepository.UpdateToReject(updDao) == 1;

                if (_insResult)
                {
                    scope.Complete();
                    _updateRes = (true, $"需求單:{updDao.orderNo} \n{updDao.statusId.GetDescription()}");
                }
                else
                    _updateRes = (false, "更新失敗");
            }

            // 發送MApp & mail 給申請人
            if (_updateRes.Item1)
            {
                var _createAccInfo = _accountDomainService.GetAccountInfoByConditions(null, null, null, createAccount).FirstOrDefault();
                _mailServer.Send(new MailEntity
                {
                    To = _createAccInfo.Mail,
                    Subject = $"需求申請單 - 剔退通知",
                    Content = "<br /> Dear Sir <br /><br />" +
                    "您有 <a style='text-decoration:underline'>已剔退</a><a style='font-weight:900'> 需求申請單</a>， <br /><br />煩請上系統確認， <br /><br />謝謝"
                });
            }

            return _updateRes;
        }

        private (bool, string) UpdateToCancel(DemandsDao updDao)
        {
            using (var scope = new TransactionScope())
            {
                var _insResult = false;

                _insResult = _demandsRepository.UpdateToCancel(updDao) == 1;

                if (_insResult)
                {
                    scope.Complete();
                    return (true, $"需求單:{updDao.orderNo} \n{updDao.statusId.GetDescription()}");
                }
                else
                    return (false, "更新失敗");
            }
        }

        private (bool, string) UpdateToCompleted(DemandsDao updDao, string createAccount)
        {
            (bool, string) _updateRes = (false, "");

            using (var scope = new TransactionScope())
            {
                var _insResult = false;

                _insResult = _demandsRepository.UpdateToComplete(updDao) == 1;

                if (_insResult)
                {
                    scope.Complete();
                    _updateRes = (true, $"需求單:{updDao.orderNo} \n{updDao.statusId.GetDescription()}");
                }
                else
                    _updateRes = (false, "更新失敗");
            }

            // 需求單 "驗證"&"完成" 發送 MApp & mail 給相關人員
            if (_updateRes.Item1 && !string.IsNullOrEmpty(createAccount) &&
                (updDao.statusId == DemandStatusEnum.Verify || updDao.statusId == DemandStatusEnum.Completed))
            {
                var _createAccInfo = _accountDomainService.GetAccountInfoByConditions(null, null, null, createAccount).FirstOrDefault();

                if (_createAccInfo == null)
                    return (true, "查無申請人, mail 無法發送");

                // 已完成 MApp 通知
                if (updDao.statusId == DemandStatusEnum.Completed)
                    _mappDomainService.SendMsgToOneAsync($"需求單已完成, 主旨: {updDao.subject} 申請人: {_createAccInfo.Name}", "253425");

                // mail 通知
                _mailServer.Send(new MailEntity
                {
                    To = updDao.statusId == DemandStatusEnum.Verify ? _createAccInfo.Mail : "WEITING.GUO@INNOLUX.COM",
                    Subject = updDao.statusId == DemandStatusEnum.Verify ? $"需求申請單 - 待確認通知" : $"需求申請單 - 已完成通知 申請人:({_createAccInfo.Name})",
                    Content = "<br /> Dear Sir <br /><br />" +
                    "您有 <a style='text-decoration:underline'>" +
                    (updDao.statusId == DemandStatusEnum.Verify ? "待確認" : "已完成") +
                    "</a><a style='font-weight:900'> 需求申請單</a>， <br /><br />煩請上系統確認， <br /><br />謝謝"
                });
            }

            return _updateRes;
        }
    }
}
