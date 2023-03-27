﻿using Microsoft.AspNetCore.Http;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using MOD4.Web.Extension.Interface;
using MOD4.Web.Repostory;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;

namespace MOD4.Web.DomainService.Demand
{
    public class DemandDomainService : BaseDomainService, IDemandDomainService
    {
        private readonly IDemandsRepository _demandsRepository;
        private readonly IUploadDomainService _uploadDomainService;
        private readonly IMAppDomainService _mappDomainService;
        private readonly IAccountDomainService _accountDomainService;
        private readonly IDemadStatusFactory _demadStatusFactory;
        private readonly IMESPermissionRepository _mesPermissionRepository;
        private readonly IMESPermissionApplicantsRepository _mesPermissionApplicantsRepository;
        private readonly IMESPermissionAuditHistoryRepository _mesPermissionAuditHistoryRepository;

        public DemandDomainService(IDemandsRepository demandsRepository,
            IUploadDomainService uploadDomainService,
            IMAppDomainService mappDomainService,
            IAccountDomainService accountDomainService,
            IDemadStatusFactory demadStatusFactory,
            IMESPermissionRepository mesPermissionRepository,
            IMESPermissionApplicantsRepository mesPermissionApplicantsRepository,
            IMESPermissionAuditHistoryRepository mesPermissionAuditHistoryRepository)
        {
            _demandsRepository = demandsRepository;
            _uploadDomainService = uploadDomainService;
            _mappDomainService = mappDomainService;
            _accountDomainService = accountDomainService;
            _demadStatusFactory = demadStatusFactory;
            _mesPermissionRepository = mesPermissionRepository;
            _mesPermissionApplicantsRepository = mesPermissionApplicantsRepository;
            _mesPermissionAuditHistoryRepository = mesPermissionAuditHistoryRepository;
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
                        $"需求單連結：<a href='http://10.54.215.210/CarUX/Demand/Edit?sn={_demandOrderSn}&orderId={_insDemandsDao.orderNo}' target='_blank'>" + _insDemandsDao.orderNo + "</a>"
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


        #region MES Permission

        public List<MESPermissionEntity> GetMESApplicantList(UserEntity userEntity,
            int sn = 0, string dateStart = null, string dateEnd = null, string statusId = "1,2,4,7,8,9", string kw = "")
        {
            try
            {
                var _nowTime = DateTime.Now;
                DateTime? _dateStart = string.IsNullOrEmpty(dateStart) ? (DateTime?)null : DateTime.Parse(dateStart);
                DateTime? _dateEnd = string.IsNullOrEmpty(dateEnd) ? (DateTime?)null : DateTime.Parse(dateEnd).AddDays(1).AddSeconds(-1);

                var _mesOrderDaoList = _mesPermissionRepository.SelectByConditions(_dateStart, _dateEnd, sn, statusId?.Split(",") ?? null, kw);

                if (!userEntity.UserMenuPermissionList.CheckPermission(MenuEnum.MESPermission, PermissionEnum.Management))
                    _mesOrderDaoList = _mesOrderDaoList.Where(w => w.applicantAccountSn == userEntity.sn || w.auditAccountSn == userEntity.sn).ToList();

                var _mesOrderDetailList = _mesPermissionApplicantsRepository.SelectByConditions(_mesOrderDaoList.Select(mes => mes.orderSn).ToList());

                var _responseEntity = _mesOrderDaoList
                        .Select(mes => new MESPermissionEntity
                        {
                            OrderSn = mes.orderSn,
                            OrderNo = mes.orderNo,
                            Status = mes.statusId.GetDescription(),
                            StatusId = mes.statusId,
                            Department = mes.department,
                            SubUnit = mes.subUnit,
                            Applicant = mes.applicant,
                            JobId = mes.jobId,
                            Phone = mes.phone,
                            ApplicantName = string.Join(",", _mesOrderDetailList.Where(w => w.mesPermissionSn == mes.orderSn).Select(s => s.applicantName)),
                            AuditAccountSn = mes.auditAccountSn,
                            CreateTimeStr = mes.createTime.ToString("yyyy-MM-dd"),
                            Url = mes.statusId == DemandStatusEnum.Rejected && mes.applicantAccountSn == userEntity.sn
                                    ? $"./MES/Update/{mes.orderSn}"
                                    : mes.auditAccountSn == userEntity.sn 
                                        ? $"./MES/Audit/{mes.orderSn}"
                                        : $"./MES/Detail/{mes.orderSn}"
                        }).ToList();

                // 查詢待簽核人員姓名
                List<AccountInfoEntity> _accInfoList = _accountDomainService.GetAccountInfo(_responseEntity.Select(s => s.AuditAccountSn).ToList());
                _responseEntity.ForEach(fe => fe.AuditName = _accInfoList.FirstOrDefault(f => f.sn == fe.AuditAccountSn)?.Name ?? "");

                return _responseEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public MESPermissionEntity GetDetail(int mesPermissionSn)
        {
            try
            {
                MESPermissionDao _mesOrder = _mesPermissionRepository.SelectByConditions(null, null, orderSn: mesPermissionSn).FirstOrDefault();
                List<MESPermissionApplicantsDao> _accessOrderDetail = _mesPermissionApplicantsRepository.SelectByConditions(mesOrderSn: new List<int> { _mesOrder.orderSn });
                List<MESPermissionDetailEntity> _permissionList = EnumHelper.GetEnumValue<MESPermissionEnum>().Select(per => new MESPermissionDetailEntity
                {
                    MESPermission = per.GetDescription(),
                    MESPermissionId = (int)per,
                    IsEnable = _mesOrder.permissionList.Split(",").Any(p => p == ((int)per).ToString())
                }).ToList();
                List<MESPermissionAuditHistoryDao> _accessOrderAuditHisList = _mesPermissionAuditHistoryRepository.SelectList(_mesOrder.orderSn);

                return new MESPermissionEntity
                {
                    OrderSn = mesPermissionSn,
                    OrderNo = _mesOrder.orderNo,
                    StatusId = _mesOrder.statusId,
                    Status = _mesOrder.statusId.GetDescription(),
                    Department = _mesOrder.department,
                    SubUnit = _mesOrder.subUnit,
                    Applicant = _mesOrder.applicant,
                    JobId = _mesOrder.jobId,
                    Phone = _mesOrder.phone,
                    PermissionInfo = _permissionList,
                    OtherPermission = _mesOrder.otherPermission,
                    SamePermName = _mesOrder.samePermName,
                    SamePermJobId = _mesOrder.samePermJobId,
                    CreateTimeStr = _mesOrder.createTime.ToString("yyyy/MM/dd"),
                    AuditAccountSn = _mesOrder.auditAccountSn,
                    Applicants = _accessOrderDetail.Select(s => new MESApplicantEntity
                    {
                        ApplicantName = s.applicantName,
                        ApplicantJobId = s.applicantJobId
                    }).ToList(),
                    MESOrderAuditHistory = _accessOrderAuditHisList.Select(his => new MESPermissionAuditHistoryEntity
                    {
                        AuditSn = his.auditSn,
                        AuditAccountName = his.auditAccountName,
                        StatusId = his.statusId,
                        Status = his.statusId.GetDescription(),
                        ReceivedTimeStr = his.receivedTime?.ToString("yyyy/MM/dd HH:mm:ss") ?? "",
                        AuditTimeStr = his.auditTime?.ToString("yyyy/MM/dd HH:mm:ss") ?? "",
                        AuditRemark = his.auditRemark,
                        Duration = his.receivedTime != null
                            ? ((DateTime)(his.auditTime ?? DateTime.Now)).Subtract((DateTime)his.receivedTime).TotalHours.ToString("0.00")
                            : "0"
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public MESPermissionEntity GetAudit(int mesPermissionSn, UserEntity userEntity)
        {
            try
            {
                MESPermissionEntity _audirDetail = GetDetail(mesPermissionSn);

                if (_audirDetail.AuditAccountSn != userEntity.sn)
                    throw new Exception("申請單狀態已變更, 請重新查詢");
                else
                    return _audirDetail;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public (bool, string) CreateMESApplicant(MESPermissionEntity mESPermissionEntity, UserEntity userEntity)
        {
            try
            {
                string _createRes = "";
                DateTime _nowTime = DateTime.Now;
                int _orderSn = 0;

                if (mESPermissionEntity.Applicants.Any(a => string.IsNullOrEmpty(a.ApplicantName) || string.IsNullOrEmpty(a.ApplicantJobId)))
                    return (false, "申請名單不能為空");

                MESPermissionDao _mesPermissionDao = new MESPermissionDao
                {
                    orderNo = "MPRS" + _nowTime.ToString("yyMMddHHmmssff"),
                    statusId = DemandStatusEnum.Signing,
                    department = mESPermissionEntity.Department,
                    subUnit = mESPermissionEntity.SubUnit,
                    applicant = mESPermissionEntity.Applicant,
                    jobId = mESPermissionEntity.JobId,
                    phone = mESPermissionEntity.Phone,
                    auditAccountSn = 6,
                    applicantAccountSn = userEntity.sn,
                    permissionList = string.Join(",", mESPermissionEntity.PermissionInfo.Where(w => w.IsEnable).Select(s => s.MESPermissionId)),
                    otherPermission = mESPermissionEntity.OtherPermission,
                    samePermName = mESPermissionEntity.SamePermName,
                    samePermJobId = mESPermissionEntity.SamePermJobId,
                    createUser = userEntity.Name,
                    createTime = _nowTime,
                };

                List<MESPermissionApplicantsDao> _mesOrderDetails = mESPermissionEntity.Applicants.Select(detail =>
                new MESPermissionApplicantsDao
                {
                    applicantName = detail.ApplicantName,
                    applicantJobId = detail.ApplicantJobId,
                    createUser = userEntity.Name,
                    createTime = _nowTime
                }).ToList();

                // 取得申請人上級
                List<MESPermissionAuditHistoryDao> _mesOrderAuditHistory = GetMESPermAuditFlow(_nowTime, userEntity);

                using (TransactionScope _scope = new TransactionScope())
                {
                    bool _insResult = false;
                    bool _insDetailResult = false;
                    bool _insHistoryResult = false;

                    _insResult = _mesPermissionRepository.Insert(_mesPermissionDao) == 1;

                    _orderSn = _mesPermissionRepository.SelectNewOrderSn(_mesPermissionDao.orderNo).orderSn;
                    _mesOrderDetails.ForEach(detail =>
                    {
                        detail.mesPermissionSn = _orderSn;
                    });
                    _mesOrderAuditHistory.ForEach(his =>
                    {
                        his.mesPermissionSn = _orderSn;
                    });

                    _insDetailResult = _mesPermissionApplicantsRepository.Insert(_mesOrderDetails) == _mesOrderDetails.Count;
                    _insHistoryResult = _mesPermissionAuditHistoryRepository.Insert(_mesOrderAuditHistory) == _mesOrderAuditHistory.Count;

                    if (_insResult && _insDetailResult && _insHistoryResult)
                        _scope.Complete();
                    else
                        _createRes = "新增申請單失敗";
                }

                // 發送MApp & mail 給作業人員
                if (_createRes == "")
                {
                    _mappDomainService.SendMsgToOneAsync($"新MES權限申請單, 申請人: {_mesPermissionDao.applicant}", "253425");
                    _mailServer.Send(new MailEntity
                    {
                        To = "WEITING.GUO@INNOLUX.COM",
                        Subject = $"新MES權限申請單 - 申請人: {_mesPermissionDao.applicant}",
                        Content = "<br /><br />" +
                        "您有新MES權限申請單</a>， <br /><br />" +
                        $"需求單連結：<a href='http://10.54.215.210/CarUX/Demand/MES/Audit/{_orderSn}' target='_blank'>" + _mesPermissionDao.orderNo + "</a>"
                    });

                    return (true, "申請成功");
                }

                return (false, _createRes);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public (bool, string) UpdateMES(MESPermissionEntity mesEntity, UserEntity userEntity)
        {
            try
            {
                MESPermissionDao _mesOrder = _mesPermissionRepository.SelectByConditions(null, null, orderSn: mesEntity.OrderSn).FirstOrDefault();
                List<MESPermissionApplicantsDao> _accessOrderDetail = _mesPermissionApplicantsRepository.SelectByConditions(mesOrderSn: new List<int> { _mesOrder.orderSn });
                List<MESPermissionAuditHistoryDao> _accessOrderAuditHisList = _mesPermissionAuditHistoryRepository.SelectList(_mesOrder.orderSn).OrderBy(ob => ob.auditSn).ToList();
                var _currentAuditFlow = _accessOrderAuditHisList.FirstOrDefault(f => f.auditAccountSn == userEntity.sn && f.auditTime == null);

                if ((_mesOrder.auditAccountSn == userEntity.sn && (_mesOrder.statusId != DemandStatusEnum.Signing && _mesOrder.statusId != DemandStatusEnum.Setting)) ||
                    _currentAuditFlow != null && _currentAuditFlow.statusId == DemandStatusEnum.Completed)
                    return (false, "申請單狀態異常");

                DateTime _nowTime = DateTime.Now;
                string _auditRes = "";
                MESPermissionDao _updMESOrder = new MESPermissionDao()
                {
                    orderSn = mesEntity.OrderSn,
                    statusId = mesEntity.StatusId,
                    applicantAccountSn = userEntity.sn,
                    department = mesEntity.Department,
                    subUnit = mesEntity.SubUnit,
                    applicant = mesEntity.Applicant,
                    jobId = mesEntity.JobId,
                    phone = mesEntity.Phone,
                    samePermName = mesEntity.SamePermName,
                    samePermJobId = mesEntity.SamePermJobId,
                    otherPermission = mesEntity.OtherPermission,
                    auditAccountSn = 6,
                    permissionList = string.Join(",", mesEntity.PermissionInfo.Where(w => w.IsEnable).Select(s => s.MESPermissionId)),
                    updateUser = userEntity.Name,
                    updateTime = _nowTime,
                    isCancel = mesEntity.StatusId == DemandStatusEnum.Cancel
                };
                List<MESPermissionApplicantsDao> _mesOrderDetails = mesEntity.Applicants.Select(detail =>
                new MESPermissionApplicantsDao
                {
                    mesPermissionSn = mesEntity.OrderSn,
                    applicantName = detail.ApplicantName,
                    applicantJobId = detail.ApplicantJobId,
                    createUser = userEntity.Name,
                    createTime = _nowTime
                }).ToList();

                // 取得申請人上級
                List<MESPermissionAuditHistoryDao> _mesOrderAuditHistory = GetMESPermAuditFlow(_nowTime, userEntity);
                _mesOrderAuditHistory.ForEach(mes => mes.mesPermissionSn = mesEntity.OrderSn);

                using (TransactionScope _scope = new TransactionScope())
                {
                    bool _updResMES = false;
                    bool _insResMESDetail = false;
                    bool _insResMESHis = false;

                    _updResMES = _mesPermissionRepository.UpdateMESOrder(_updMESOrder) == 1;

                    if (_updMESOrder.isCancel)
                    {
                        _insResMESDetail = true;
                        _insResMESHis = true;
                    }
                    else
                    {
                        _insResMESDetail = _mesPermissionApplicantsRepository.Delete(_mesOrderDetails.Select(s => s.mesPermissionSn).ToList()) == _mesOrderDetails.Count;
                        _insResMESDetail = _mesPermissionApplicantsRepository.Insert(_mesOrderDetails) == _mesOrderDetails.Count;
                        _insResMESHis = _mesPermissionAuditHistoryRepository.Delete(_mesOrderAuditHistory.Select(s => s.mesPermissionSn).ToList()) == _mesOrderAuditHistory.Count;
                        _insResMESHis = _mesPermissionAuditHistoryRepository.Insert(_mesOrderAuditHistory) == _mesOrderAuditHistory.Count;
                    }

                    if (_updResMES && _insResMESDetail && _insResMESHis)
                        _scope.Complete();
                    else
                        _auditRes = "更新申請單異常";
                }

                if (_auditRes == "")
                {
                    _mappDomainService.SendMsgToOneAsync($"新MES權限申請單 ,申請人: {_updMESOrder.applicant}", "253425");
                    _mailServer.Send(new MailEntity
                    {
                        To = "WEITING.GUO@INNOLUX.COM",
                        Subject = $"新MES權限申請單 - 申請人: {_updMESOrder.applicant}",
                        Content = "<br /><br />" +
                        "您有新MES權限申請單</a>，<br /><br />" +
                        $"申請單連結：<a href='http://10.54.215.210/CarUX/Demand/MES/Audit/{_updMESOrder.orderSn}' target='_blank'>" + _updMESOrder.orderNo + "</a>"
                    });

                    return (true, $"申請單{mesEntity.StatusId.GetDescription()}");
                }
                else
                {
                    return (false, _auditRes);
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }


        public string AuditMES(int orderSn, DemandStatusEnum statusId, string remark, UserEntity userEntity)
        {
            try
            {
                MESPermissionDao _mesOrder = _mesPermissionRepository.SelectByConditions(null, null, orderSn: orderSn).FirstOrDefault();
                List<MESPermissionAuditHistoryDao> _accessOrderAuditHisList = _mesPermissionAuditHistoryRepository.SelectList(_mesOrder.orderSn).OrderBy(ob => ob.auditSn).ToList();
                var _currentAuditFlow = _accessOrderAuditHisList.FirstOrDefault(f => f.auditAccountSn == userEntity.sn && f.auditTime == null);

                if ((_mesOrder.auditAccountSn == userEntity.sn && (_mesOrder.statusId != DemandStatusEnum.Signing && _mesOrder.statusId != DemandStatusEnum.Setting)) ||
                    _currentAuditFlow != null && _currentAuditFlow.statusId == DemandStatusEnum.Completed)
                    return "申請單狀態異常";

                DateTime _nowTime = DateTime.Now;
                string _auditRes = "";
                MESPermissionDao _updMESOrder = new MESPermissionDao()
                {
                    orderSn = _mesOrder.orderSn,
                    statusId = _mesOrder.statusId,
                    auditAccountSn = 0,
                    updateUser = userEntity.Name,
                    updateTime = _nowTime
                };
                List<MESPermissionAuditHistoryDao> _updMESPermAuditHis = new List<MESPermissionAuditHistoryDao>();
                MESPermissionAuditHistoryDao _currentRecord = _accessOrderAuditHisList.FirstOrDefault(f => f.auditAccountSn == userEntity.sn && f.auditTime == null);
                MESPermissionAuditHistoryDao _nextRecord = _accessOrderAuditHisList.FirstOrDefault(f => f.receivedTime == null);

                switch (statusId)
                {
                    case DemandStatusEnum.Rejected:
                        _updMESOrder.statusId = statusId;
                        _currentRecord.statusId = statusId;
                        _currentRecord.auditTime = _nowTime;
                        _currentRecord.auditRemark = remark;
                        break;
                    case DemandStatusEnum.Approve when _nextRecord == null:
                        _updMESOrder.statusId = DemandStatusEnum.Setting;
                        _updMESOrder.auditAccountSn = 6;
                        _currentRecord.statusId = statusId;
                        _currentRecord.auditTime = _nowTime;
                        _currentRecord.auditRemark = remark;
                        break;
                    case DemandStatusEnum.Completed when _nextRecord == null:
                        _updMESOrder.statusId = statusId;
                        _updMESOrder.auditAccountSn = 6;
                        break;
                    default:
                        _updMESOrder.auditAccountSn = _nextRecord.auditAccountSn;
                        _currentRecord.statusId = statusId;
                        _currentRecord.auditTime = _nowTime;
                        _currentRecord.auditRemark = remark;
                        _nextRecord.receivedTime = _nowTime;
                        break;
                }

                if (_currentRecord != null)
                    _updMESPermAuditHis.Add(_currentRecord);
                if (_nextRecord != null)
                    _updMESPermAuditHis.Add(_nextRecord);

                using (TransactionScope _scope = new TransactionScope())
                {
                    bool _updResMES = false;
                    bool _updResMESHis = true;

                    _updResMES = _mesPermissionRepository.Update(_updMESOrder) == 1;
                    if (_updMESPermAuditHis.Any())
                        _updResMESHis = _mesPermissionAuditHistoryRepository.Update(_updMESPermAuditHis) == _updMESPermAuditHis.Count;

                    if (_updResMES && _updResMESHis)
                        _scope.Complete();
                    else
                        _auditRes = "更新申請單簽核異常";
                }

                if (_auditRes != "")
                    return _auditRes;

                // 剔退 通知申請人
                if (_updMESOrder.statusId == DemandStatusEnum.Rejected)
                {
                    _mailServer.Send(new MailEntity
                    {
                        To = _accountDomainService.GetAccountInfo(new List<int> { _mesOrder.applicantAccountSn }).FirstOrDefault().Mail,
                        Subject = "MES 權限申請單 - 退件通知",
                        Content = "<br /> Dear Sir <br /><br />" +
                        "您有<a style='text-decoration:underline'>待重送</a><a style='font-weight:900'>MES 權限申請單</a>， <br /><br />" +
                        $"單號連結：<a href='http://10.54.215.210/CarUX/Demand/MES/Update/{_mesOrder.orderSn}' target='_blank'>" + _mesOrder.orderNo + "</a>"
                    });
                }
                // 發送待簽核 mail
                else if (_nextRecord != null)
                    _mailServer.Send(new MailEntity
                    {
                        To = _nextRecord.Mail,
                        Subject = $"MES 權限申請單 - 待簽核通知 (申請人:{_mesOrder.applicant})",
                        Content = "<br /> Dear Sir <br /><br />" +
                        "您有<a style='text-decoration:underline'>待簽核</a><a style='font-weight:900'>MES 權限申請單</a>， <br /><br />" +
                        $"單號連結：<a href='http://10.54.215.210/CarUX/Demand/MES/Audit/{_mesOrder.orderSn}' target='_blank'>" + _mesOrder.orderNo + "</a>"
                    });
                else if (_updMESOrder.statusId == DemandStatusEnum.Setting)
                {
                    // 通知系統設定人員
                    _mailServer.Send(new MailEntity
                    {
                        To = _accessOrderAuditHisList.FirstOrDefault(f => f.auditSn == 1).Mail,
                        Subject = $"MES 權限申請單 - 簽核完成",
                        Content = "<br /> Dear Sir <br /><br />" +
                        "您有<a style='text-decoration:underline'>已簽核完成</a><a style='font-weight:900'>MES 權限申請單</a>， <br /><br />" +
                        $"單號連結：<a href='http://10.54.215.210/CarUX/Demand/MES/Detail/{_mesOrder.orderSn}' target='_blank'>" + _mesOrder.orderNo + "</a>"
                    });
                }
                else if (_updMESOrder.statusId == DemandStatusEnum.Completed)
                {
                    // 通知申請人
                    _mailServer.Send(new MailEntity
                    {
                        To = _accountDomainService.GetAccountInfo(new List<int> { _mesOrder.applicantAccountSn }).FirstOrDefault().Mail,
                        Subject = $"MES 權限申請單 - 設定完成",
                        Content = "<br /> Dear Sir <br /><br />" +
                        "您有<a style='text-decoration:underline'>已完成</a><a style='font-weight:900'>MES 權限申請單</a>， <br /><br />" +
                        $"單號連結：<a href='http://10.54.215.210/CarUX/Demand/MES/Detail/{_mesOrder.orderSn}' target='_blank'>" + _mesOrder.orderNo + "</a>"
                    });
                }

                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion

        #region Private
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

        private List<MESPermissionAuditHistoryDao> GetMESPermAuditFlow(DateTime nowTime, UserEntity userEntity)
        {
            // 取得申請人上級
            var _auditFlow = _accountDomainService.GetAuditFlowInfo(userEntity);

            List<MESPermissionAuditHistoryDao> _mesOrderAuditHistory = new List<MESPermissionAuditHistoryDao>
                {
                    new MESPermissionAuditHistoryDao
                    {
                        auditSn = 1,
                        auditAccountSn = 6,
                        auditAccountName = "郭瑋婷",
                        statusId = DemandStatusEnum.Signing,
                        receivedTime = nowTime,
                        Mail = "WEITING.GUO@INNOLUX.COM"
                    },
                    new MESPermissionAuditHistoryDao
                    {
                        auditSn = 2,
                        auditAccountSn = _auditFlow.SectionAccSn,
                        auditAccountName = _auditFlow.SectionName,
                        statusId = DemandStatusEnum.Signing,
                        Mail = _auditFlow.SectionMail
                    },
                    new MESPermissionAuditHistoryDao
                    {
                        auditSn = 3,
                        auditAccountSn = 42,
                        auditAccountName = "陳繹印",
                        statusId = DemandStatusEnum.Signing,
                        Mail = "MORRISE.CHEN@INNOLUX.COM"
                    }
                };

            return _mesOrderAuditHistory;
        }
        #endregion

    }
}
