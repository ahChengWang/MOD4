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

        public DemandDomainService(IDemandsRepository demandsRepository,
            IUploadDomainService uploadDomainService)
        {
            _demandsRepository = demandsRepository;
            _uploadDomainService = uploadDomainService;
        }

        public List<DemandEntity> GetDemands(
            string sn = null, string dateStart = null, string dateEnd = null, string categoryId = null, string statusId = null)
        {
            try
            {
                var _nowTime = DateTime.Now;
                DateTime _dateStart = string.IsNullOrEmpty(dateStart) ? _nowTime.AddMonths(-6) : DateTime.Parse(dateStart);
                DateTime _dateEnd = string.IsNullOrEmpty(dateEnd) ? _nowTime : DateTime.Parse(dateEnd).AddDays(1).AddSeconds(-1);

                return _demandsRepository.SelectByConditions(
                    _dateStart, _dateEnd, orderSn: sn,
                    categoryArray: string.IsNullOrEmpty(categoryId) ? null : categoryId.Split(","),
                    statusArray: string.IsNullOrEmpty(statusId) ? null : statusId.Split(","))
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
                        UpdateTime = s.updateTime
                    }).OrderByDescending(ob => ob.CreateTime).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public DemandEntity GetDemandDetail(int sn, string orderId)
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
            };

            var fileArray = _demandDao.uploadFiles.Split(",");

            if (fileArray != null && fileArray.Any())
            {
                _result.UploadFile1 = fileArray[0] ?? "";
                _result.UploadFile2 = fileArray.Length > 1 ? fileArray[1] : "";
                _result.UploadFile3 = fileArray.Length > 2 ? fileArray[2] : "";
            }

            return _result;
        }

        public (FileStream, string) GetDownFileStr(int sn, int fileNo)
        {
            DemandsDao _demandDao = _demandsRepository.SelectDetail(sn);
            string[] fileArray = _demandDao.uploadFiles.Split(",");

            return (new FileStream($@"{_uploadDomainService.GetFileServerUrl()}\upload\{_demandDao.createUser}\{_demandDao.createTime.ToString("yyMMdd")}\{fileArray[fileNo]}",
                FileMode.Open, FileAccess.Read, FileShare.Read)
                , fileArray[fileNo]);
        }

        public (bool, string) InsertDemand(DemandEntity insertEntity, UserEntity userEntity)
        {
            try
            {
                var _nowTime = DateTime.Now;
                var _fileNameStr = "";
                var _insResponse = "";

                _fileNameStr = DoUploadFiles(insertEntity.UploadFileList, userEntity.Account, _nowTime.ToString("yyMMdd"));

                DemandsDao _insDemandsDao = new DemandsDao
                {
                    createUser = userEntity.Account,
                    createTime = _nowTime,
                    updateUser = "",
                    updateTime = _nowTime
                };

                /*
                if (!Directory.Exists($@"{_url}\{_folder}"))
                {
                    Directory.CreateDirectory($@"{_url}\{_folder}");
                }

                foreach (var file in insertEntity.UploadFileList)
                {
                    if (file.Length > 0)
                    {
                        var path = $@"{_url}\{_folder}\{file.FileName}";

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        _fileNameStr += file.FileName + ",";
                    }
                }
                */

                _insDemandsDao.orderNo = $"DE{_nowTime.ToString("yyMMddHHmmss")}";
                _insDemandsDao.categoryId = insertEntity.CategoryId;
                _insDemandsDao.statusId = DemandStatusEnum.Pending;
                _insDemandsDao.subject = insertEntity.Subject;
                _insDemandsDao.content = insertEntity.Content;
                _insDemandsDao.applicant = insertEntity.Applicant;
                _insDemandsDao.jobNo = insertEntity.JobNo;
                _insDemandsDao.uploadFiles = _fileNameStr;

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
                var _updResponse = "";

                DemandsDao _oldDemand = _demandsRepository.SelectDetail(updEntity.OrderSn, updEntity.OrderNo);

                DemandsDao _updDemandsDao = new DemandsDao
                {
                    orderSn = updEntity.OrderSn,
                    orderNo = updEntity.OrderNo,
                    statusId = newStatusId,
                    updateUser = userEntity.Name,
                    updateTime = _nowTime
                };

                if (_oldDemand.statusId == DemandStatusEnum.Pending && newStatusId == DemandStatusEnum.Rejected)
                {
                    _updDemandsDao.rejectReason = updEntity.RejectReason ?? "";
                    return UpdateToReject(_updDemandsDao);
                }
                else if (_oldDemand.statusId == DemandStatusEnum.Pending && newStatusId == DemandStatusEnum.Processing)
                    return UpdateToProcess(_updDemandsDao);
                else if (_oldDemand.statusId == DemandStatusEnum.Processing && newStatusId == DemandStatusEnum.Completed)
                {
                    _fileNameStr = DoUploadFiles(updEntity.UploadFileList, userEntity.Account, _nowTime.ToString("yyMMdd"));
                    _updDemandsDao.completeFiles = _fileNameStr;
                    return UpdateToCompleted(_updDemandsDao);
                }
                else if (_oldDemand.statusId == DemandStatusEnum.Rejected && newStatusId == DemandStatusEnum.Pending)
                {
                    _updDemandsDao.subject = updEntity.Subject;
                    _updDemandsDao.content = updEntity.Content;
                    _fileNameStr = DoUploadFiles(updEntity.UploadFileList, userEntity.Account, _nowTime.ToString("yyMMdd"));
                    _updDemandsDao.uploadFiles = _fileNameStr;
                    return UpdateToPending(_updDemandsDao);
                }
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

        private string DoUploadFiles(List<IFormFile> uploadFiles, string userAcc, string dateTimeStr)
        {
            var _url = _uploadDomainService.GetFileServerUrl();
            var _folder = $@"upload\{userAcc}\{dateTimeStr}";
            var _fileNameStr = "";

            if (!Directory.Exists($@"{_url}\{_folder}"))
            {
                Directory.CreateDirectory($@"{_url}\{_folder}");
            }

            foreach (var file in uploadFiles)
            {
                if (file.Length > 0)
                {
                    var path = $@"{_url}\{_folder}\{file.FileName}";

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    _fileNameStr += file.FileName + ",";
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
