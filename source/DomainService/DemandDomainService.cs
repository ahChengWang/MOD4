using Microsoft.AspNetCore.Http;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using MOD4.Web.Helper;
using MOD4.Web.Repostory;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Transactions;

namespace MOD4.Web.DomainService
{
    public class DemandDomainService : IDemandDomainService
    {
        private readonly IDemandsRepository _demandsRepository;
        private readonly IUploadDomainService _uploadDomainService;
        private readonly IMAppDomainService _mappDomainService;

        public DemandDomainService(IDemandsRepository demandsRepository,
            IUploadDomainService uploadDomainService,
            IMAppDomainService mappDomainService)
        {
            _demandsRepository = demandsRepository;
            _uploadDomainService = uploadDomainService;
            _mappDomainService = mappDomainService;
        }

        public List<DemandEntity> GetDemands(UserEntity userEntity,
            string sn = null, string dateStart = null, string dateEnd = null, string categoryId = null, string statusId = "1,2,3,4", string kw = "")
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
                            s.statusId == DemandStatusEnum.Rejected &&
                            userEntity.UserMenuPermissionList.CheckPermission(MenuEnum.Demand, PermissionEnum.Update)
                    }).OrderByDescending(ob => ob.CreateTime).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public DemandEntity GetDemandDetail(int sn, string orderId = "")
        {
            DemandsDao _demandDao = _demandsRepository.SelectDetail(sn, orderId);

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
            else if ((typeId == 1 && string.IsNullOrEmpty(_demandDao.uploadFiles)) ||
                     (typeId == 2 && string.IsNullOrEmpty(_demandDao.completeFiles)))
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

        public (bool, string) UpdateDemand(DemandEntity updEntity, DemandStatusEnum newStatusId, UserEntity userEntity)
        {
            try
            {
                var _nowTime = DateTime.Now;
                var _url = _uploadDomainService.GetFileServerUrl();
                var _folder = $@"upload\{userEntity.Account}\{_nowTime.ToString("yyMMdd")}";
                var _fileNameStr = "";

                DemandsDao _oldDemand = _demandsRepository.SelectDetail(updEntity.OrderSn, updEntity.OrderNo);

                DemandsDao _updDemandsDao = new DemandsDao
                {
                    orderSn = updEntity.OrderSn,
                    orderNo = updEntity.OrderNo,
                    statusId = newStatusId,
                    subject = updEntity.Subject,
                    content = updEntity.Content,
                    updateUser = userEntity.Account,
                    updateTime = _nowTime
                };

                // 剔退
                if (_oldDemand.statusId == DemandStatusEnum.Pending && newStatusId == DemandStatusEnum.Rejected)
                {
                    _updDemandsDao.rejectReason = updEntity.RejectReason ?? "";
                    //_mappDomainService.SendMsgToOneAsync(_oldDemand.createUser);
                    return UpdateToReject(_updDemandsDao);
                }
                // 進行
                else if (_oldDemand.statusId == DemandStatusEnum.Pending && newStatusId == DemandStatusEnum.Processing)
                    return UpdateToProcess(_updDemandsDao);
                // 完成
                else if (_oldDemand.statusId == DemandStatusEnum.Processing && newStatusId == DemandStatusEnum.Completed ||
                         _oldDemand.statusId == DemandStatusEnum.Completed && newStatusId == DemandStatusEnum.Completed)
                {
                    // mgmt 檔案上傳
                    _fileNameStr = DoUploadFiles(updEntity.UploadFileList ?? new List<IFormFile>(), userEntity.Account, _nowTime, _nowTime);
                    _updDemandsDao.completeFiles = _fileNameStr;
                    _updDemandsDao.remark = updEntity.Remark;
                    return UpdateToCompleted(_updDemandsDao);
                }
                // user 剔退重送
                else if (_oldDemand.statusId == DemandStatusEnum.Rejected && newStatusId == DemandStatusEnum.Pending)
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
                else if (_oldDemand.statusId == DemandStatusEnum.Rejected && newStatusId == DemandStatusEnum.Cancel)
                {
                    _updDemandsDao.isCancel = true;
                    return UpdateToCancel(_updDemandsDao);
                }
                else
                    return (false, "需求單狀態異常");
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
                        _newFileName += i == (_fileArray.Length - 2) ? $"{_fileArray[i]}_{nowTime.ToString("ffff")}." : _fileArray[i];
                    }

                    var path = $@"{_url}\{_folder}\{_newFileName}";

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    _fileNameStr += _newFileName + ",";
                }
            }

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

        private (bool, string) UpdateToReject(DemandsDao updDao)
        {
            using (var scope = new TransactionScope())
            {
                var _insResult = false;

                _insResult = _demandsRepository.UpdateToReject(updDao) == 1;

                if (_insResult)
                {
                    scope.Complete();
                    return (true, $"需求單:{updDao.orderNo} \n{updDao.statusId.GetDescription()}");
                }
                else
                    return (false, "更新失敗");
            }
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

        private (bool, string) UpdateToCompleted(DemandsDao updDao)
        {
            using (var scope = new TransactionScope())
            {
                var _insResult = false;

                _insResult = _demandsRepository.UpdateToComplete(updDao) == 1;

                if (_insResult)
                {
                    scope.Complete();
                    return (true, $"需求單:{updDao.orderNo} \n{updDao.statusId.GetDescription()}");
                }
                else
                    return (false, "更新失敗");
            }
        }
    }
}
