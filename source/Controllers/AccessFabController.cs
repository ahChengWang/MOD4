using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using MOD4.Web.DomainService;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using MOD4.Web.Helper;
using MOD4.Web.Models;
using MOD4.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.Controllers
{
    [Authorize]
    public class AccessFabController : BaseController
    {
        private readonly IAccessFabDomainService _accessFabDomainService;
        private readonly IOptionDomainService _optionDomainService;
        private readonly ILogger<HomeController> _logger;

        public AccessFabController(IHttpContextAccessor httpContextAccessor,
            IAccessFabDomainService accessFabDomainService,
            IOptionDomainService optionDomainService,
            IAccountDomainService accountDomainService,
            ILogger<HomeController> logger)
            : base(httpContextAccessor, accountDomainService)
        {
            _accessFabDomainService = accessFabDomainService;
            _logger = logger;
            _optionDomainService = optionDomainService;
        }

        /// <summary>
        /// 管制口 - 首頁
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            try
            {
                // 主頁搜尋下拉選項
                var _optionList = _optionDomainService.GetAccessFabOptions();

                // 訂單狀態
                ViewBag.StatusList = new SelectList(_optionList.FirstOrDefault(f => f.Item1 == "statusList").Item2, "Id", "Value");
                // 入廠性質
                ViewBag.FabInTypeList = new SelectList(_optionList.FirstOrDefault(f => f.Item1 == "fabInTypeList").Item2, "Id", "Value");
                // 入廠對象
                ViewBag.FabInCategoryList = new SelectList(_optionList.FirstOrDefault(f => f.Item1 == "fabInCategoryList").Item2, "Id", "Value");

                UserEntity _userInfo = GetUserInfo();
                // user 當前權限
                var _userCurrentPagePermission = _userInfo.UserMenuPermissionList.FirstOrDefault(f => f.MenuSn == MenuEnum.AccessFab);
                ViewBag.UserPermission = new UserPermissionViewModel
                {
                    AccountSn = _userCurrentPagePermission.AccountSn,
                    MenuSn = _userCurrentPagePermission.MenuSn,
                    AccountPermission = _userCurrentPagePermission.AccountPermission
                };

                List<AccessFabOrderEntity> _result = _accessFabDomainService.GetList(_userInfo, new AccessFabSelectOptionEntity());

                List<AccessFabMainViewModel> _resAccessList = _result.Select(s => new AccessFabMainViewModel
                {
                    OrderSn = s.OrderSn,
                    OrderNo = s.OrderNo,
                    Applicant = s.Applicant,
                    Date = s.CreateTimeStr,
                    FabInCategory = s.Category,
                    FabInType = s.FabInType,
                    Status = s.Status,
                    StatusId = s.StatusId,
                    AuditAccount = s.AuditAccountName,
                    Content = s.Content,
                    Url = s.Url
                }).ToList();

                return View(_resAccessList);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Search([FromQuery] string startDate, string endDate, int statusId, int fabInTypeId, string orderNo, string applicant)
        {
            try
            {
                var _userInfo = GetUserInfo();

                List<AccessFabOrderEntity> _result = _accessFabDomainService.GetList(_userInfo,
                    new AccessFabSelectOptionEntity
                    {
                        StartDate = startDate,
                        EndDate = endDate,
                        StatusId = statusId,
                        FabInTypeId = fabInTypeId,
                        OrderNo = orderNo,
                        Applicant = applicant
                    });

                List<AccessFabMainViewModel> _resAccessList = _result.Select(s => new AccessFabMainViewModel
                {
                    OrderSn = s.OrderSn,
                    OrderNo = s.OrderNo,
                    Applicant = s.Applicant,
                    Date = s.CreateTimeStr,
                    FabInCategory = s.Category,
                    FabInType = s.FabInType,
                    Status = s.Status,
                    StatusId = s.StatusId,
                    AuditAccount = s.AuditAccountName,
                    Content = s.Content,
                    Url = s.Url
                }).ToList();

                return PartialView("_PartialTable", _resAccessList);
            }
            catch (Exception ex)
            {
                return Json(new { Msg = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Edit([FromQuery] int orderSn)
        {
            try
            {
                var _optionList = _optionDomainService.GetAccessFabOptions();

                ViewBag.StatusList = new SelectList(_optionList.FirstOrDefault(f => f.Item1 == "statusList").Item2, "Id", "Value");
                ViewBag.FabInTypeList = new SelectList(_optionList.FirstOrDefault(f => f.Item1 == "fabInTypeList").Item2, "Id", "Value");
                ViewBag.FabInCategoryList = new SelectList(_optionList.FirstOrDefault(f => f.Item1 == "fabInCategoryList").Item2, "Id", "Value");
                ViewBag.AccountName = GetUserInfo().Name;

                AccessFabOrderEntity _accessFabOrderData = _accessFabDomainService.GetDetail(orderSn);

                var _res = new AccessFabDetailPageViewModel
                {
                    OrderSn = orderSn,
                    FabInTypeId = _accessFabOrderData.FabInTypeId,
                    FabInOtherType = _accessFabOrderData.FabInOtherType,
                    Applicant = _accessFabOrderData.Applicant,
                    ApplicantMVPN = _accessFabOrderData.ApplicantMVPN,
                    FabInCategoryId = _accessFabOrderData.CategoryId,
                    FillOutPerson = _accessFabOrderData.FillOutPerson,
                    AccompanyingPerson = _accessFabOrderData.AccompanyingPerson,
                    AccompanyingPersonMVPN = _accessFabOrderData.AccompanyingPersonMVPN,
                    Content = _accessFabOrderData.Content,
                    Route = _accessFabOrderData.Route,
                    JobId = _accessFabOrderData.JobId,
                    AccessFabOrderNo = _accessFabOrderData.OrderNo,
                    OrderStatus = _accessFabOrderData.StatusId.GetDescription(),
                    FabInDate = _accessFabOrderData.FabInDate,
                    FabOutDate = _accessFabOrderData.FabOutDate,
                    CreateTimeStr = _accessFabOrderData.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    AccessPersonList = _accessFabOrderData.DetailList.Select(detail => new AccessFabPersonsViewModel
                    {
                        DetailSn = detail.Sn,
                        CompanyName = detail.CompanyName,
                        Name = detail.Name,
                        GuestPhone = detail.GuestPhone,
                        ClotheSize = detail.ClotheSize,
                        ShoesSize = detail.ShoesSize
                    }).ToList(),
                    AuditHistory = _accessFabOrderData.AuditFlow.Select(his => new AccessFabAuditHistoryViewModel
                    {
                        AuditSn = his.AuditSn,
                        AuditAccountName = his.AuditAccountName,
                        StatusId = his.StatusId,
                        Status = his.StatusId.GetDescription(),
                        ReceivedTimeStr = his.ReceivedTimeStr,
                        AuditTimeStr = his.AuditTimeStr,
                        AuditRemark = his.AuditRemark,
                        Duration = his.Duration
                    }).ToList()
                };

                return View(_res);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Edit([FromForm] AccessFabDetailPageViewModel accessFabOrderEditModel)
        {
            try
            {
                string _editRes = _accessFabDomainService.Edit(new AccessFabOrderEntity
                {
                    OrderSn = accessFabOrderEditModel.OrderSn,
                    FabInTypeId = accessFabOrderEditModel.FabInTypeId,
                    FabInOtherType = accessFabOrderEditModel.FabInOtherType,
                    FillOutPerson = accessFabOrderEditModel.FillOutPerson,
                    Applicant = accessFabOrderEditModel.Applicant,
                    ApplicantMVPN = accessFabOrderEditModel.ApplicantMVPN,
                    CategoryId = accessFabOrderEditModel.FabInCategoryId,
                    AccompanyingPerson = accessFabOrderEditModel.AccompanyingPerson,
                    AccompanyingPersonMVPN = accessFabOrderEditModel.AccompanyingPersonMVPN,
                    Content = accessFabOrderEditModel.Content,
                    Route = accessFabOrderEditModel.Route,
                    JobId = accessFabOrderEditModel.JobId,
                    FabInDate = accessFabOrderEditModel.FabInDate,
                    FabOutDate = accessFabOrderEditModel.FabOutDate,
                    DetailList = accessFabOrderEditModel.AccessPersonList.Select(detail => new AccessFabOrderDetailEntity
                    {
                        Sn = detail.DetailSn,
                        CompanyName = detail.CompanyName,
                        Name = detail.Name,
                        GuestPhone = detail.GuestPhone,
                        ClotheSize = detail.ClotheSize,
                        ShoesSize = detail.ShoesSize
                    }).ToList()
                }, GetUserInfo());

                if (_editRes == "")
                    return Json(new { IsSuccess = true, Msg = "" });
                else
                    return Json(new { IsSuccess = false, Msg = _editRes });

            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Detail([FromQuery] int orderSn)
        {
            try
            {
                var _optionList = _optionDomainService.GetAccessFabOptions();

                ViewBag.StatusList = new SelectList(_optionList.FirstOrDefault(f => f.Item1 == "statusList").Item2, "Id", "Value");
                ViewBag.FabInTypeList = new SelectList(_optionList.FirstOrDefault(f => f.Item1 == "fabInTypeList").Item2, "Id", "Value");
                ViewBag.FabInCategoryList = new SelectList(_optionList.FirstOrDefault(f => f.Item1 == "fabInCategoryList").Item2, "Id", "Value");

                AccessFabOrderEntity _accessFabOrderData = _accessFabDomainService.GetDetail(orderSn);

                var _res = new AccessFabDetailPageViewModel
                {
                    FabInTypeId = _accessFabOrderData.FabInTypeId,
                    FabInOtherType = _accessFabOrderData.FabInOtherType,
                    Applicant = _accessFabOrderData.Applicant,
                    ApplicantMVPN = _accessFabOrderData.ApplicantMVPN,
                    FabInCategoryId = _accessFabOrderData.CategoryId,
                    FillOutPerson = _accessFabOrderData.FillOutPerson,
                    AccompanyingPerson = _accessFabOrderData.AccompanyingPerson,
                    AccompanyingPersonMVPN = _accessFabOrderData.AccompanyingPersonMVPN,
                    Content = _accessFabOrderData.Content,
                    Route = _accessFabOrderData.Route,
                    JobId = _accessFabOrderData.JobId,
                    AccessFabOrderNo = _accessFabOrderData.OrderNo,
                    OrderStatusId = _accessFabOrderData.StatusId,
                    OrderStatus = _accessFabOrderData.StatusId.GetDescription(),
                    FabInDate = _accessFabOrderData.FabInDate,
                    FabOutDate = _accessFabOrderData.FabOutDate,
                    CreateTimeStr = _accessFabOrderData.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    AccessPersonList = _accessFabOrderData.DetailList.Select(detail => new AccessFabPersonsViewModel
                    {
                        CompanyName = detail.CompanyName,
                        Name = detail.Name,
                        GuestPhone = detail.GuestPhone,
                        ClotheSize = detail.ClotheSize,
                        ShoesSize = detail.ShoesSize
                    }).ToList(),
                    AuditHistory = _accessFabOrderData.AuditFlow.Select(his => new AccessFabAuditHistoryViewModel
                    {
                        AuditSn = his.AuditSn,
                        AuditAccountName = his.AuditAccountName,
                        StatusId = his.StatusId,
                        Status = his.StatusId.GetDescription(),
                        ReceivedTimeStr = his.ReceivedTimeStr,
                        AuditTimeStr = his.AuditTimeStr,
                        AuditRemark = his.AuditRemark,
                        Duration = his.Duration
                    }).ToList()
                };

                return View(_res);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Download([FromQuery] int orderSn, int typeId, int fileNo)
        {
            try
            {

                return File("", "application/octet-stream", "");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            try
            {
                var _optionList = _optionDomainService.GetAccessFabOptions();

                ViewBag.FabInTypeList = new SelectList(_optionList.FirstOrDefault(f => f.Item1 == "fabInTypeList").Item2, "Id", "Value");
                ViewBag.FabInCategoryList = new SelectList(_optionList.FirstOrDefault(f => f.Item1 == "fabInCategoryList").Item2, "Id", "Value");
                ViewBag.AccountName = GetUserInfo().Name;
                ViewBag.JobId = GetUserInfo().JobId;

                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Create([FromForm] AccessFabCreateViewModel createModel)
        {
            try
            {
                string _result = _accessFabDomainService.Create(new AccessFabOrderEntity
                {
                    FabInTypeId = createModel.FabInTypeId,
                    FabInOtherType = createModel.FabInOtherType,
                    CategoryId = createModel.FabInCategoryId,
                    FillOutPerson = createModel.FillOutPerson,
                    Applicant = createModel.Applicant,
                    ApplicantMVPN = createModel.ApplicantMVPN,
                    JobId = createModel.JobId,
                    AccompanyingPerson = createModel.AccompanyingPerson,
                    AccompanyingPersonMVPN = createModel.AccompanyingPersonMVPN,
                    Content = createModel.Content,
                    Route = createModel.Route,
                    FabInDate = createModel.FabInDate,
                    FabOutDate = createModel.FabOutDate,
                    DetailList = createModel.Details.CopyAToB<AccessFabOrderDetailEntity>()
                }, GetUserInfo());

                if (_result == "")
                    return Json(new { IsSuccess = true, Msg = "" });
                else
                    return Json(new { IsSuccess = false, Msg = _result });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }

        [HttpDelete]
        public IActionResult Delete([FromQuery] int orderSn)
        {
            try
            {
                string _result = _accessFabDomainService.Delete(orderSn, GetUserInfo());

                if (_result == "")
                    return Json(new { IsSuccess = true, Msg = "" });
                else
                    return Json(new { IsSuccess = false, Msg = _result });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }


        #region ===== Audit =====

        [HttpGet("[controller]/Audit")]
        public IActionResult Audit()
        {
            try
            {
                var _optionList = _optionDomainService.GetAccessFabOptions();

                ViewBag.StatusList = new SelectList(_optionList.FirstOrDefault(f => f.Item1 == "statusList").Item2, "Id", "Value");
                ViewBag.FabInTypeList = new SelectList(_optionList.FirstOrDefault(f => f.Item1 == "fabInTypeList").Item2, "Id", "Value");
                ViewBag.FabInCategoryList = new SelectList(_optionList.FirstOrDefault(f => f.Item1 == "fabInCategoryList").Item2, "Id", "Value");

                List<AccessFabOrderEntity> _result = _accessFabDomainService.GetAuditList(
                    GetUserInfo(), 
                    new AccessFabSelectOptionEntity() 
                    { 
                        IsDefaultPage = true
                    });

                List<AccessFabMainViewModel> _resAccessList = _result.Select(s => new AccessFabMainViewModel
                {
                    OrderSn = s.OrderSn,
                    OrderNo = s.OrderNo,
                    Applicant = s.Applicant,
                    FabInDate = s.FabInDateStr,
                    Date = s.CreateTimeStr,
                    FabInCategory = s.Category,
                    //FabInType = s.FabInType,
                    GustNames = s.GustNames,
                    Status = s.Status,
                    StatusId = s.StatusId,
                    Content = s.Content,
                    Url = s.Url
                }).ToList();

                return View(_resAccessList);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }

        }


        [HttpGet("[controller]/Audit/Search")]
        public IActionResult AuditSearch([FromQuery] string startFabInDate, string endFabInDate, int fabInTypeId, string guestName, string applicant)
        {
            try
            {
                var _userInfo = GetUserInfo();

                List<AccessFabOrderEntity> _result = _accessFabDomainService.GetAuditList(_userInfo,
                    new AccessFabSelectOptionEntity
                    {
                        StartFabInDate = startFabInDate,
                        EndFabInDate = endFabInDate,
                        FabInTypeId = fabInTypeId,
                        Applicant = applicant,
                        GuestName = guestName,
                        IsDefaultPage = false
                    });

                List<AccessFabMainViewModel> _resAccessList = _result.Select(s => new AccessFabMainViewModel
                {
                    OrderSn = s.OrderSn,
                    OrderNo = s.OrderNo,
                    Applicant = s.Applicant,
                    FabInDate = s.FabInDateStr,
                    Date = s.CreateTimeStr,
                    FabInCategory = s.Category,
                    //FabInType = s.FabInType,
                    //Status = s.Status,
                    //StatusId = s.StatusId,
                    GustNames = s.GustNames,
                    Status = s.Status,
                    StatusId = s.StatusId,
                    Content = s.Content,
                    Url = s.Url
                }).ToList();

                return PartialView("_PartialTable", _resAccessList);
            }
            catch (Exception ex)
            {
                return Json(new { Msg = ex.Message });
            }
        }

        [HttpGet("[controller]/Audit/Detail/{orderSn}")]
        public IActionResult AuditDetail(int orderSn)
        {
            try
            {
                var _optionList = _optionDomainService.GetAccessFabOptions();

                ViewBag.StatusList = new SelectList(_optionList.FirstOrDefault(f => f.Item1 == "statusList").Item2, "Id", "Value");
                ViewBag.FabInTypeList = new SelectList(_optionList.FirstOrDefault(f => f.Item1 == "fabInTypeList").Item2, "Id", "Value");
                ViewBag.FabInCategoryList = new SelectList(_optionList.FirstOrDefault(f => f.Item1 == "fabInCategoryList").Item2, "Id", "Value");

                string _verifyResult = _accessFabDomainService.VerifyAuditStatus(orderSn, GetUserInfo());
                if (_verifyResult != "")
                    return RedirectToAction("Error", "Home", new ErrorViewModel { Message = "訂單「已簽核」, 請回簽核主頁" });

                UserEntity _userInfo = GetUserInfo();
                var _userCurrentPagePermission = _userInfo.UserMenuPermissionList.FirstOrDefault(f => f.MenuSn == MenuEnum.AccessFabAudit);
                ViewBag.UserPermission = new UserPermissionViewModel
                {
                    AccountSn = _userCurrentPagePermission.AccountSn,
                    MenuSn = _userCurrentPagePermission.MenuSn,
                    AccountPermission = _userCurrentPagePermission.AccountPermission
                };

                AccessFabOrderEntity _accessFabOrderData = _accessFabDomainService.GetDetail(orderSn);

                var _res = new AccessFabDetailPageViewModel
                {
                    OrderSn = orderSn,
                    FabInTypeId = _accessFabOrderData.FabInTypeId,
                    FabInOtherType = _accessFabOrderData.FabInOtherType,
                    FillOutPerson = _accessFabOrderData.FillOutPerson,
                    Applicant = _accessFabOrderData.Applicant,
                    ApplicantMVPN = _accessFabOrderData.ApplicantMVPN,
                    FabInCategoryId = _accessFabOrderData.CategoryId,
                    AccompanyingPerson = _accessFabOrderData.AccompanyingPerson,
                    AccompanyingPersonMVPN = _accessFabOrderData.AccompanyingPersonMVPN,
                    Content = _accessFabOrderData.Content,
                    Route = _accessFabOrderData.Route,
                    JobId = _accessFabOrderData.JobId,
                    AccessFabOrderNo = _accessFabOrderData.OrderNo,
                    OrderStatusId = _accessFabOrderData.StatusId,
                    OrderStatus = _accessFabOrderData.StatusId.GetDescription(),
                    FabInDate = _accessFabOrderData.FabInDate,
                    FabOutDate = _accessFabOrderData.FabOutDate,
                    CreateTimeStr = _accessFabOrderData.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    AccessPersonList = _accessFabOrderData.DetailList.Select(detail => new AccessFabPersonsViewModel
                    {
                        CompanyName = detail.CompanyName,
                        Name = detail.Name,
                        GuestPhone = detail.GuestPhone,
                        ClotheSize = detail.ClotheSize,
                        ShoesSize = detail.ShoesSize
                    }).ToList(),
                    AuditHistory = _accessFabOrderData.AuditFlow.Select(his => new AccessFabAuditHistoryViewModel
                    {
                        AuditSn = his.AuditSn,
                        AuditAccountName = his.AuditAccountName,
                        StatusId = his.StatusId,
                        Status = his.StatusId.GetDescription(),
                        ReceivedTimeStr = his.ReceivedTimeStr,
                        AuditTimeStr = his.AuditTimeStr,
                        AuditRemark = his.AuditRemark,
                        Duration = his.Duration
                    }).ToList()
                };

                return View(_res);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpPost("[controller]/Audit/Update")]
        public IActionResult AuditUpdate([FromForm] AccessFabDetailPageViewModel approveViewModel)
        {
            try
            {
                string _result = _accessFabDomainService.Audit(new AccessFabOrderEntity
                {
                    OrderSn = approveViewModel.OrderSn,
                    StatusId = approveViewModel.OrderStatusId,
                    Remark = approveViewModel.Remark
                }
                , GetUserInfo());

                if (_result == "")
                    return Json(new { IsSuccess = true, Msg = "" });
                else
                    return Json(new { IsSuccess = false, Msg = _result });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }

        [HttpGet("[controller]/Audit/Verify/{orderSn}")]
        public IActionResult Verify(int orderSn)
        {
            try
            {
                string _verifyResult = _accessFabDomainService.VerifyAuditStatus(orderSn, GetUserInfo());

                if (_verifyResult == "")
                    return Json(new { IsSuccess = true, Msg = "" });
                else
                    return Json(new { IsSuccess = false, Msg = _verifyResult });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }
        #endregion
    }
}
