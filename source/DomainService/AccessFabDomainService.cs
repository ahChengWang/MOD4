using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using MOD4.Web.Repostory;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace MOD4.Web.DomainService
{
    public class AccessFabDomainService : BaseDomainService, IAccessFabDomainService
    {
        private readonly IAccessFabOrderRepository _accessFabOrderRepository;
        private readonly IAccountDomainService _accountDomainService;
        private readonly IAccessFabOrderDetailRepository _accessFabOrderDetailRepository;
        private readonly IAccessFabOrderAuditHistoryRepository _accessFabOrderAuditHistoryRepository;

        public AccessFabDomainService(IAccessFabOrderRepository accessFabOrderRepository,
            IAccountDomainService accountDomainService,
            IAccessFabOrderDetailRepository accessFabOrderDetailRepository,
            IAccessFabOrderAuditHistoryRepository accessFabOrderAuditHistoryRepository)
        {
            _accessFabOrderRepository = accessFabOrderRepository;
            _accountDomainService = accountDomainService;
            _accessFabOrderDetailRepository = accessFabOrderDetailRepository;
            _accessFabOrderAuditHistoryRepository = accessFabOrderAuditHistoryRepository;
        }

        public List<AccessFabOrderEntity> GetList(UserEntity userEntity, AccessFabSelectOptionEntity selectOption)
        {
            try
            {
                selectOption.ApplicantAccountSn = userEntity.sn;
                List<AccessFabOrderEntity> _accessFabOrderList = GetAccessFabOrderList(selectOption);
                List<AccountInfoEntity> _accInfoList = _accountDomainService.GetAccountInfo(_accessFabOrderList.Select(s => s.AuditAccountSn).ToList());
                _accessFabOrderList.ForEach(fe => fe.AuditAccountName = _accInfoList.FirstOrDefault(f => f.sn == fe.AuditAccountSn).Name);

                return _accessFabOrderList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AccessFabOrderEntity GetDetail(int accessFabOrderSn)
        {
            try
            {
                AccessFabOrderDao _accessOrder = _accessFabOrderRepository.SelectList(orderSn: accessFabOrderSn).FirstOrDefault();
                List<AccessFabOrderDetailDao> _accessOrderDetail = _accessFabOrderDetailRepository.SelectList(_accessOrder.OrderSn);
                List<AccessFabOrderAuditHistoryDao> _accessOrderAuditHisList = _accessFabOrderAuditHistoryRepository.SelectList(_accessOrder.OrderSn);

                return new AccessFabOrderEntity
                {
                    OrderSn = accessFabOrderSn,
                    FabInTypeId = _accessOrder.FabInTypeId,
                    FabInType = _accessOrder.FabInTypeId.GetDescription(),
                    FabInOtherType = _accessOrder.FabInOtherType,
                    Applicant = _accessOrder.Applicant,
                    ApplicantMVPN = _accessOrder.ApplicantMVPN,
                    CategoryId = _accessOrder.CategoryId,
                    Category = _accessOrder.CategoryId.GetDescription(),
                    AccompanyingPerson = _accessOrder.AccompanyingPerson,
                    Content = _accessOrder.Content,
                    Route = _accessOrder.Route,
                    JobId = _accessOrder.JobId,
                    OrderNo = _accessOrder.OrderNo,
                    StatusId = _accessOrder.StatusId,
                    Status = _accessOrder.StatusId.GetDescription(),
                    FabInDate = _accessOrder.FabInDate,
                    FabOutDate = _accessOrder.FabOutDate,
                    CreateTimeStr = _accessOrder.CreateTime.ToString("yyyy/MM/dd HH:mm:ss"),
                    DetailList = _accessOrderDetail.Select(detail => new AccessFabOrderDetailEntity
                    {
                        Sn = detail.Sn,
                        CompanyName = detail.CompanyName,
                        Name = detail.Name,
                        GuestPhone = detail.GuestPhone
                    }).ToList(),
                    AuditFlow = _accessOrderAuditHisList.Select(his => new AccessFabOrderAuditHistoryEntity
                    {
                        AuditSn = his.AuditSn,
                        AuditAccountName = his.AuditAccountName,
                        StatusId = his.StatusId,
                        Status = his.StatusId.GetDescription(),
                        ReceivedTimeStr = his.ReceivedTime?.ToString("yyyy/MM/dd HH:mm:ss") ?? "",
                        AuditTimeStr = his.AuditTime?.ToString("yyyy/MM/dd HH:mm:ss") ?? "",
                        AuditRemark = his.AuditRemark,
                        Duration = his.ReceivedTime != null
                            ? ((DateTime)(his.AuditTime ?? DateTime.Now)).Subtract((DateTime)his.ReceivedTime).TotalHours.ToString("0.00")
                            : "0"
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string Create(AccessFabOrderEntity orderEntity, UserEntity userEntity)
        {
            try
            {
                DateTime _nowTime = DateTime.Now;
                string _createRes = "";

                var _verifyResult = Verify(orderEntity);

                if (!_verifyResult.status)
                    return _verifyResult.msg;
                if (userEntity.DeptSn == 0)
                    return "尚未設定部門, 請聯絡系統管理員";

                var _auditFlow = _accountDomainService.GetAuditFlowInfo(userEntity);

                AccessFabOrderDao _insAccessFabOrderDao = new AccessFabOrderDao()
                {
                    OrderNo = "CG" + _nowTime.ToString("yyMMddHHmmssff"),
                    FabInTypeId = orderEntity.FabInTypeId,
                    FabInOtherType = orderEntity.FabInOtherType ?? "",
                    CategoryId = orderEntity.CategoryId,
                    StatusId = FabInOutStatusEnum.Processing,
                    Applicant = orderEntity.Applicant,
                    JobId = orderEntity.JobId,
                    ApplicantMVPN = orderEntity.ApplicantMVPN,
                    FabInDate = orderEntity.FabInDate,
                    FabOutDate = orderEntity.FabOutDate,
                    Content = orderEntity.Content,
                    Route = orderEntity.Route,
                    AccompanyingPerson = orderEntity.AccompanyingPerson,
                    ApplicantAccountSn = userEntity.sn,
                    CreateUser = userEntity.Name,
                    CreateTime = _nowTime,
                    UpdateUser = "",
                    UpdateTime = _nowTime
                };

                List<AccessFabOrderDetailDao> _insAccessFabOrderDetailListDao = new List<AccessFabOrderDetailDao>();
                foreach (AccessFabOrderDetailEntity detail in orderEntity.DetailList)
                {
                    _insAccessFabOrderDetailListDao.Add(new AccessFabOrderDetailDao()
                    {
                        CompanyName = detail.CompanyName,
                        Name = detail.Name,
                        GuestPhone = detail.GuestPhone
                    });
                }

                List<AccessFabOrderAuditHistoryDao> _insAccessFabOrderAuditHisDao = ProcessFabOrderAuditFlow(_auditFlow, userEntity.Level_id, _nowTime);

                _insAccessFabOrderDao.AuditAccountSn = _insAccessFabOrderAuditHisDao.FirstOrDefault(f => f.AuditSn == 1).AuditAccountSn;

                using (var scope = new TransactionScope())
                {
                    bool _insFabOrderRes = false;
                    bool _insFabOrderDetailRes = false;
                    bool _insFabOrderHisRes = false;

                    _insFabOrderRes = _accessFabOrderRepository.Insert(_insAccessFabOrderDao) == 1;
                    var _latestOrderSn = _accessFabOrderRepository.SelectList(orderNo: _insAccessFabOrderDao.OrderNo).FirstOrDefault().OrderSn;

                    // 新增入廠人員明細
                    _insAccessFabOrderDetailListDao.ForEach(fe => fe.AccessFabOrderSn = _latestOrderSn);
                    _insFabOrderDetailRes = _accessFabOrderDetailRepository.Insert(_insAccessFabOrderDetailListDao) == _insAccessFabOrderDetailListDao.Count;

                    // 新增申請單簽核
                    _insAccessFabOrderAuditHisDao.ForEach(fe => fe.AccessFabOrderSn = _latestOrderSn);
                    _insFabOrderHisRes = _accessFabOrderAuditHistoryRepository.Insert(_insAccessFabOrderAuditHisDao) == _insAccessFabOrderAuditHisDao.Count;

                    if (_insFabOrderRes && _insFabOrderDetailRes && _insFabOrderHisRes)
                        scope.Complete();
                    else
                        _createRes = "新增申請單異常";
                }

                // 發送待簽核 mail
                if (_createRes == "")
                {
                    _mailServer.Send(new MailEntity
                    {
                        To = _insAccessFabOrderAuditHisDao.FirstOrDefault(f => f.AuditSn == 1).Mail,
                        Subject = "管制口進出申請單 - 待簽核通知",
                        Content = "<br /> Dear Sir <br /><br />" +
                        "您有<a style='text-decoration:underline'>待簽核</a><a style='font-weight:900'>管制口進出請單</a>， <br /><br />" +
                        "簽核單號連結：<a href='http://10.54.215.210/MOD4/AccessFab/Audit' target='_blank'>" + _insAccessFabOrderDao.OrderNo + "</a>"
                    });
                }

                return _createRes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string Edit(AccessFabOrderEntity orderEntity, UserEntity userEntity)
        {
            try
            {
                DateTime _nowTime = DateTime.Now;
                string _createRes = "";

                var _verifyResult = Verify(orderEntity);

                if (!_verifyResult.status)
                    return _verifyResult.msg;

                var _auditFlow = _accountDomainService.GetAuditFlowInfo(userEntity);

                var _oldAccessFabOrderDetail = _accessFabOrderDetailRepository.SelectList(orderEntity.OrderSn);

                AccessFabOrderDao _updAccessFabOrderDao = new AccessFabOrderDao()
                {
                    OrderSn = orderEntity.OrderSn,
                    FabInTypeId = orderEntity.FabInTypeId,
                    FabInOtherType = orderEntity.FabInOtherType ?? "",
                    CategoryId = orderEntity.CategoryId,
                    StatusId = FabInOutStatusEnum.Processing,
                    Applicant = orderEntity.Applicant,
                    JobId = orderEntity.JobId,
                    ApplicantMVPN = orderEntity.ApplicantMVPN,
                    FabInDate = orderEntity.FabInDate,
                    FabOutDate = orderEntity.FabOutDate,
                    Content = orderEntity.Content,
                    Route = orderEntity.Route,
                    AccompanyingPerson = orderEntity.AccompanyingPerson,
                    ApplicantAccountSn = userEntity.sn,
                    UpdateUser = userEntity.Name,
                    UpdateTime = _nowTime
                };

                List<AccessFabOrderDetailDao> _updAccessFabOrderDetailListDao = new List<AccessFabOrderDetailDao>();
                foreach (AccessFabOrderDetailEntity detail in orderEntity.DetailList)
                {
                    _updAccessFabOrderDetailListDao.Add(new AccessFabOrderDetailDao()
                    {
                        Sn = detail.Sn,
                        AccessFabOrderSn = detail.AccessFabOrderSn,
                        CompanyName = detail.CompanyName,
                        Name = detail.Name,
                        GuestPhone = detail.GuestPhone
                    });
                }

                List<AccessFabOrderDetailDao> _delAccessFabOrderDetail =
                    _oldAccessFabOrderDetail.Where(w => !_updAccessFabOrderDetailListDao.Select(del => del.Sn).Contains(w.Sn)).ToList();

                List<AccessFabOrderDetailDao> _updAccessFabOrderDetail =
                    _oldAccessFabOrderDetail.Where(w => _updAccessFabOrderDetailListDao.Select(del => del.Sn).Contains(w.Sn)).ToList();

                List<AccessFabOrderDetailDao> _insAccessFabOrderDetail = _updAccessFabOrderDetailListDao.Where(w => w.Sn == 0).ToList();

                List<AccessFabOrderAuditHistoryDao> _updAccessFabOrderAuditHisDao = ProcessFabOrderAuditFlow(_auditFlow, userEntity.Level_id, _nowTime);

                _updAccessFabOrderAuditHisDao.ForEach(fe => fe.AccessFabOrderSn = orderEntity.OrderSn);

                _updAccessFabOrderDao.AuditAccountSn = _updAccessFabOrderAuditHisDao.FirstOrDefault(f => f.AuditSn == 1).AuditAccountSn;

                using (var scope = new TransactionScope())
                {
                    bool _updFabOrderRes = false;
                    bool _updFabOrderDetailRes = false;
                    bool _updFabOrderHisRes = false;

                    _updFabOrderRes = _accessFabOrderRepository.Update(_updAccessFabOrderDao) == 1;
                    var _latestOrderSn = _accessFabOrderRepository.SelectList(orderNo: _updAccessFabOrderDao.OrderNo).FirstOrDefault().OrderSn;

                    // 新增入廠人員明細
                    _updFabOrderDetailRes = _accessFabOrderDetailRepository.Insert(_insAccessFabOrderDetail) == _insAccessFabOrderDetail.Count;
                    _updFabOrderDetailRes = _accessFabOrderDetailRepository.Delete(_delAccessFabOrderDetail.Select(s => s.Sn).ToList()) == _delAccessFabOrderDetail.Count;
                    _updFabOrderDetailRes = _accessFabOrderDetailRepository.Update(_updAccessFabOrderDetail) == _updAccessFabOrderDetail.Count;

                    // 更新申請單簽核流程
                    _updFabOrderHisRes = _accessFabOrderAuditHistoryRepository.Update(_updAccessFabOrderAuditHisDao) == _updAccessFabOrderAuditHisDao.Count;

                    if (_updFabOrderRes && _updFabOrderDetailRes && _updFabOrderHisRes)
                        scope.Complete();
                    else
                        _createRes = "新增申請單異常";
                }

                return _createRes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string Delete(int orderSn, UserEntity userEntity)
        {
            try
            {
                DateTime _nowTime = DateTime.Now;
                string _createRes = "";

                var _accessFabOrder = _accessFabOrderRepository.SelectList(orderSn: orderSn).FirstOrDefault();

                if (_accessFabOrder.StatusId != FabInOutStatusEnum.Rejected)
                    return "單據狀態異常";

                AccessFabOrderDao _updAccessFabOrderDao = new AccessFabOrderDao()
                {
                    OrderSn = orderSn,
                    StatusId = FabInOutStatusEnum.Cancel,
                    UpdateUser = userEntity.Name,
                    UpdateTime = _nowTime
                };

                using (var scope = new TransactionScope())
                {
                    bool _updFabOrderRes = false;

                    _updFabOrderRes = _accessFabOrderRepository.UpdateCancel(_updAccessFabOrderDao) == 1;
                    if (_updFabOrderRes)
                        scope.Complete();
                    else
                        _createRes = "作廢申請單異常";
                }

                return _createRes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<AccessFabOrderEntity> GetAuditList(UserEntity userEntity, AccessFabSelectOptionEntity selectOption)
        {
            // 取得該使用者待簽核申請單
            selectOption.AuditAccountSn = userEntity.sn;
            selectOption.StatusId = (int)FabInOutStatusEnum.Processing;
            var _accessFabOrderAuditList = GetAccessFabOrderList(selectOption);

            _accessFabOrderAuditList.ForEach(fe => fe.Url = $"./Audit/Detail/{fe.OrderSn}");

            return _accessFabOrderAuditList;
        }

        public string Audit(AccessFabOrderEntity orderEntity, UserEntity userEntity)
        {
            try
            {
                DateTime _nowTime = DateTime.Now;
                string _auditRes = "";

                var _auditFlow = _accountDomainService.GetAuditFlowInfo(userEntity);

                var _oldAccessFabOrder = _accessFabOrderRepository.SelectList(orderEntity.OrderSn).FirstOrDefault();
                var _accessFabOrderAuditHisList = _accessFabOrderAuditHistoryRepository.SelectList(orderEntity.OrderSn);

                // 檢核
                var _verifyResult = AuditVerify(orderEntity, _oldAccessFabOrder, _accessFabOrderAuditHisList, userEntity.sn);
                if (!_verifyResult.status)
                    return _verifyResult.msg;

                AccessFabOrderDao _updAccessFabOrderDao = new AccessFabOrderDao()
                {
                    OrderSn = orderEntity.OrderSn,
                    AuditAccountSn = _oldAccessFabOrder.AuditAccountSn,
                    UpdateUser = userEntity.Name,
                    UpdateTime = _nowTime
                };

                List<AccessFabOrderAuditHistoryDao> _updAccessFabOrderAuditHisDao = new List<AccessFabOrderAuditHistoryDao>();
                AccountInfoEntity _nextAuditAccountInfo = new AccountInfoEntity();

                // 更新目前簽核紀錄
                var _currentAuditFlow = _accessFabOrderAuditHisList.OrderBy(ob => ob.AuditSn).FirstOrDefault(f => f.AuditTime == null && f.AuditAccountSn == userEntity.sn);
                _currentAuditFlow.StatusId = orderEntity.StatusId;
                _currentAuditFlow.AuditTime = _nowTime;
                _currentAuditFlow.AuditRemark = orderEntity.Remark;
                _updAccessFabOrderAuditHisDao.Add(_currentAuditFlow);

                // 更新下一關簽核紀錄
                var _nextAuditFlow = _accessFabOrderAuditHisList.OrderBy(ob => ob.AuditSn).FirstOrDefault(f => f.AuditTime == null && f.AuditAccountSn != userEntity.sn);
                if (orderEntity.StatusId == FabInOutStatusEnum.Rejected)
                    _updAccessFabOrderDao.StatusId = orderEntity.StatusId;
                // 簽核已完成, 無下個簽核人員
                else if (orderEntity.StatusId != FabInOutStatusEnum.Rejected && _nextAuditFlow == null)
                    _updAccessFabOrderDao.StatusId = FabInOutStatusEnum.Completed;
                // 簽核未完成, 更新下個簽核人員資訊
                else if (orderEntity.StatusId != FabInOutStatusEnum.Rejected && _nextAuditFlow != null)
                {
                    _updAccessFabOrderDao.StatusId = FabInOutStatusEnum.Processing;
                    _updAccessFabOrderDao.AuditAccountSn = _nextAuditFlow.AuditAccountSn;
                    _nextAuditFlow.ReceivedTime = _nowTime;
                    _updAccessFabOrderAuditHisDao.Add(_nextAuditFlow);
                    _nextAuditAccountInfo.Mail = _accountDomainService.GetAccountInfo(new List<int> { _nextAuditFlow.AuditSn }).FirstOrDefault().Mail;
                }

                using (var scope = new TransactionScope())
                {
                    bool _updFabOrderRes = false;
                    bool _updFabOrderHisRes = false;

                    _updFabOrderRes = _accessFabOrderRepository.UpdateAudit(_updAccessFabOrderDao) == 1;

                    // 更新申請單簽核流程
                    _updFabOrderHisRes = _accessFabOrderAuditHistoryRepository.UpdateAudit(_updAccessFabOrderAuditHisDao) == _updAccessFabOrderAuditHisDao.Count;

                    if (_updFabOrderRes && _updFabOrderHisRes)
                        scope.Complete();
                    else
                        _auditRes = "更新申請單簽核異常";
                }

                // 剔退 通知申請人
                if (orderEntity.StatusId == FabInOutStatusEnum.Rejected && _auditRes == "")
                {
                    _mailServer.Send(new MailEntity
                    {
                        To = userEntity.Mail,
                        Subject = "管制口進出申請單 - 剔退待通知",
                        Content = "<br /> Dear Sir <br /><br />" +
                        "您有<a style='text-decoration:underline'>待重送</a><a style='font-weight:900'>管制口進出請單</a>， <br /><br />" +
                        "單號連結：<a href='http://10.54.215.210/MOD4/AccessFab/Audit' target='_blank'>" + _oldAccessFabOrder.OrderNo + "</a>"
                    });
                }
                // 發送待簽核 mail
                else if (_auditRes == "" && _nextAuditFlow != null)
                {
                    _mailServer.Send(new MailEntity
                    {
                        To = _nextAuditAccountInfo.Mail,
                        Subject = "管制口進出申請單 - 待簽核通知",
                        Content = "<br /> Dear Sir <br /><br />" +
                        "您有<a style='text-decoration:underline'>待簽核</a><a style='font-weight:900'>管制口進出請單</a>， <br /><br />" +
                        "單號連結：<a href='http://10.54.215.210/MOD4/AccessFab/Audit' target='_blank'>" + _oldAccessFabOrder.OrderNo + "</a>"
                    });
                }

                return _auditRes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string VerifyAuditStatus(int orderSn, UserEntity userEntity)
        {
            var _orderAccessFabOrder = _accessFabOrderRepository.SelectList(orderSn: orderSn).FirstOrDefault();

            if (_orderAccessFabOrder.StatusId != FabInOutStatusEnum.Processing ||
                (_orderAccessFabOrder.StatusId == FabInOutStatusEnum.Processing && _orderAccessFabOrder.AuditAccountSn != userEntity.sn))
                return "申請單已變更, 將重新整理頁面";

            return "";
        }

        private List<AccessFabOrderEntity> GetAccessFabOrderList(AccessFabSelectOptionEntity selectOption = null)
        {
            try
            {
                List<AccessFabOrderDao> accessOrderList = _accessFabOrderRepository.SelectList(
                    orderNo: selectOption?.OrderNo ?? "",
                    statusId: selectOption?.StatusId ?? 0,
                    fabInTypeId: selectOption?.FabInTypeId ?? 0,
                    applicant: selectOption?.Applicant ?? "",
                    applicantAccountSn: selectOption?.ApplicantAccountSn ?? 0,
                    auditAccountSn: selectOption?.AuditAccountSn ?? 0,
                    startTime: DateTime.TryParse(selectOption?.StartDate ?? "", out _) ? (DateTime?)DateTime.Parse(selectOption.StartDate) : null,
                    endTime: DateTime.TryParse(selectOption?.EndDate ?? "", out _) ? (DateTime?)(DateTime.Parse(selectOption.EndDate)).AddDays(1) : null);

                return accessOrderList.Select(s => new AccessFabOrderEntity()
                {
                    OrderSn = s.OrderSn,
                    OrderNo = s.OrderNo,
                    Applicant = s.Applicant,
                    CategoryId = s.CategoryId,
                    Category = s.CategoryId.GetDescription(),
                    FabInTypeId = s.FabInTypeId,
                    FabInType = s.FabInTypeId == FabInTypeEnum.Other ? s.FabInOtherType : s.FabInTypeId.GetDescription(),
                    StatusId = s.StatusId,
                    Status = s.StatusId.GetDescription(),
                    Content = s.Content,
                    AuditAccountSn = s.AuditAccountSn,
                    CreateTime = s.CreateTime,
                    CreateTimeStr = s.CreateTime.ToString("yyyy-MM-dd"),
                    Url = s.StatusId == FabInOutStatusEnum.Rejected ? $"./AccessFab/Edit?orderSn={s.OrderSn}" : $"./AccessFab/Detail?orderSn={s.OrderSn}",
                }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private (bool status, string msg) Verify(AccessFabOrderEntity entity)
        {
            if (entity.FabInTypeId == FabInTypeEnum.Other &&
                string.IsNullOrEmpty(entity.FabInOtherType.Trim()))
                return (false, "「其他」欄位必填");
            if (entity.FabInDate == null || entity.FabOutDate == null ||
                entity.FabInDate >= entity.FabOutDate)
                return (false, "「入廠」、「離廠」欄位有誤");

            return (true, "");
        }

        private (bool status, string msg) AuditVerify(AccessFabOrderEntity entity, AccessFabOrderDao oldEntity, List<AccessFabOrderAuditHistoryDao> _accessFabAuditHisList, int userAccSn)
        {
            AccessFabOrderAuditHistoryDao _currentFlowAuditData =
                _accessFabAuditHisList.FirstOrDefault(fe => fe.AuditAccountSn == userAccSn);

            if (oldEntity == null)
                return (false, "申請單異常");
            if (oldEntity.StatusId != FabInOutStatusEnum.Processing)
                return (false, "申請單狀態異常");
            if (oldEntity.AuditAccountSn != userAccSn)
                return (false, "請確認使用者是否為簽核人員");
            if (_currentFlowAuditData == null || _currentFlowAuditData.AuditTime != null)
                return (false, "申請單已簽核, 請重新整理頁面");

            return (true, "");
        }

        private List<AccessFabOrderAuditHistoryDao> ProcessFabOrderAuditFlow(AccessFabOrderFlowEntity auditFlow, JobLevelEnum userJobLevel, DateTime nowTime)
        {
            var _gateManager = _accountDomainService.GetAccInfoByDepartment(new UserEntity
            {
                DeptSn = 5
            });

            switch (userJobLevel)
            {
                case JobLevelEnum.SectionManager:
                    return new List<AccessFabOrderAuditHistoryDao>() {
                        new AccessFabOrderAuditHistoryDao
                        {
                            AuditSn = 1,
                            AuditAccountSn = _gateManager.sn,
                            //AuditAccountName = auditFlow.DeptName,
                            AuditAccountName = $"管制口主管-{_gateManager.Name}",
                            StatusId = FabInOutStatusEnum.Processing,
                            ReceivedTime = nowTime,
                            IsDel = false,
                            Mail = _gateManager.Mail
                        },
                        new AccessFabOrderAuditHistoryDao
                        {
                            AuditSn = 2,
                            AuditAccountSn = auditFlow.TopAccSn,
                            //AuditAccountName = auditFlow.TopName,
                            AuditAccountName = $"廠長-{auditFlow.TopName}",
                            StatusId = FabInOutStatusEnum.Processing,
                            IsDel = false,
                            Mail = auditFlow.TopMail
                        }
                    };
                case JobLevelEnum.Employee:
                    return new List<AccessFabOrderAuditHistoryDao>() {
                        new AccessFabOrderAuditHistoryDao
                        {
                            AuditSn = 1,
                            AuditAccountSn = auditFlow.DeptAccSn,
                            //AuditAccountName = auditFlow.SectionName,
                            AuditAccountName = $"部門主管-{auditFlow.DeptName}",
                            StatusId = FabInOutStatusEnum.Processing,
                            ReceivedTime = nowTime,
                            IsDel = false,
                            Mail = auditFlow.DeptMail
                        },
                        new AccessFabOrderAuditHistoryDao
                        {
                            AuditSn = 2,
                            AuditAccountSn = _gateManager.sn,
                            //AuditAccountName = auditFlow.DeptName,
                            AuditAccountName = $"管制口主管-{_gateManager.Name}",
                            StatusId = FabInOutStatusEnum.Processing,
                            IsDel = false,
                            Mail = _gateManager.Mail
                        },
                        new AccessFabOrderAuditHistoryDao
                        {
                            AuditSn = 3,
                            AuditAccountSn = auditFlow.TopAccSn,
                            //AuditAccountName = auditFlow.TopName,
                            AuditAccountName = $"廠長-{auditFlow.TopName}",
                            StatusId = FabInOutStatusEnum.Processing,
                            IsDel = false,
                            Mail = auditFlow.TopMail
                        }};
                default:
                    return new List<AccessFabOrderAuditHistoryDao>();
            }
        }
    }
}
