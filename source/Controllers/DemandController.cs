using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MOD4.Web.DomainService;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using MOD4.Web.Models;
using MOD4.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.Controllers
{
    [Authorize]
    public class DemandController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDemandDomainService _demandDomainService;
        private readonly IOptionDomainService _optionDomainService;

        public DemandController(IDemandDomainService demandDomainService,
            IHttpContextAccessor httpContextAccessor,
            IOptionDomainService optionDomainService,
            IAccountDomainService accountDomainService,
            ILogger<HomeController> logger)
            : base(httpContextAccessor, accountDomainService)
        {
            _demandDomainService = demandDomainService;
            _optionDomainService = optionDomainService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                UserEntity _userInfo = GetUserInfo();
                var _userCurrentPagePermission = _userInfo.UserMenuPermissionList.FirstOrDefault(f => f.MenuSn == MenuEnum.Demand);
                ViewBag.UserPermission = new UserPermissionViewModel
                {
                    AccountSn = _userCurrentPagePermission.AccountSn,
                    MenuSn = _userCurrentPagePermission.MenuSn,
                    AccountPermission = _userCurrentPagePermission.AccountPermission
                };

                var _demands = _demandDomainService.GetDemands(_userInfo, dateStart: DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd"), dateEnd: DateTime.Now.ToString("yyyy-MM-dd"));

                List<DemanMainViewModel> _response = _demands.Select(s => new DemanMainViewModel
                {
                    OrderSn = s.OrderSn,
                    OrderId = s.OrderNo,
                    DemandCategory = s.CategoryId.GetDescription(),
                    DemandCategoryId = s.CategoryId,
                    DemandStatus = s.StatusId.GetDescription(),
                    DemandStatusId = s.StatusId,
                    Subject = s.Subject,
                    Applicant = s.Applicant,
                    JobNo = s.JobNo,
                    CreateDate = s.CreateTime.ToString("yyyy-MM-dd"),
                    UserEditable = s.UserEditable,
                    RoleId = _userInfo.RoleId
                }).ToList();

                return View(_response);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Search([FromQuery] string startDate, string endDate, string category, string status, string kw)
        {
            try
            {
                UserEntity _userInfo = GetUserInfo();
                var _userCurrentPagePermission = _userInfo.UserMenuPermissionList.FirstOrDefault(f => f.MenuSn == MenuEnum.Demand);
                ViewBag.UserPermission = new UserPermissionViewModel
                {
                    AccountSn = _userCurrentPagePermission.AccountSn,
                    MenuSn = _userCurrentPagePermission.MenuSn,
                    AccountPermission = _userCurrentPagePermission.AccountPermission
                };

                var _demands = _demandDomainService.GetDemands(_userInfo, dateStart: startDate, dateEnd: endDate, categoryId: category, statusId: status, kw: kw);

                List<DemanMainViewModel> _response = _demands.Select(s => new DemanMainViewModel
                {
                    OrderSn = s.OrderSn,
                    OrderId = s.OrderNo,
                    DemandCategory = s.CategoryId.GetDescription(),
                    DemandCategoryId = s.CategoryId,
                    DemandStatus = s.StatusId.GetDescription(),
                    DemandStatusId = s.StatusId,
                    Subject = s.Subject,
                    Applicant = s.Applicant,
                    JobNo = s.JobNo,
                    CreateDate = s.CreateTime.ToString("yyyy-MM-dd"),
                    UserEditable = s.UserEditable,
                    RoleId = _userInfo.RoleId
                }).ToList();

                return PartialView("_PartialTable", _response);
            }
            catch (Exception ex)
            {
                return Json(new { Msg = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Edit([FromQuery] int sn, string orderId)
        {
            try
            {
                ViewBag.CategoryOptions = _optionDomainService.GetDemandCategoryOptionList();

                UserEntity _userInfo = GetUserInfo();
                var _userCurrentPagePermission = _userInfo.UserMenuPermissionList.FirstOrDefault(f => f.MenuSn == MenuEnum.Demand);
                ViewBag.UserPermission = new UserPermissionViewModel
                {
                    AccountSn = _userCurrentPagePermission.AccountSn,
                    MenuSn = _userCurrentPagePermission.MenuSn,
                    AccountPermission = _userCurrentPagePermission.AccountPermission
                };

                var _res = _demandDomainService.GetDemandDetail(sn, _userInfo, orderId);

                DemanEditViewModel _response = new DemanEditViewModel
                {
                    OrderSn = sn,
                    OrderId = _res.OrderNo,
                    CreateDate = _res.CreateTimeStr,
                    DemandCategoryId = _res.CategoryId,
                    DemandStatus = _res.Status,
                    DemandStatusId = _res.StatusId,
                    Subject = _res.Subject,
                    Content = _res.Content,
                    Applicant = _res.Applicant,
                    JobNo = _res.JobNo,
                    UploadFile1 = _res.UploadFile1,
                    UploadFile2 = _res.UploadFile2,
                    UploadFile3 = _res.UploadFile3,
                    CompleteUploadFile1 = _res.CompleteUploadFile1,
                    CompleteUploadFile2 = _res.CompleteUploadFile2,
                    CompleteUploadFile3 = _res.CompleteUploadFile3,
                    RejectReason = _res.RejectReason,
                    Remark = _res.Remark
                };

                return View(_response);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }

        }

        [HttpPost]
        public IActionResult Edit([FromForm] DemanEditViewModel updModel)
        {
            try
            {
                var _res = _demandDomainService.UpdateDemand(new DemandEntity
                {
                    OrderSn = updModel.OrderSn,
                    OrderNo = updModel.OrderId,
                    StatusId = updModel.DemandStatusId,
                    CategoryId = updModel.DemandCategoryId,
                    Subject = updModel.Subject,
                    Content = updModel.Content,
                    Applicant = updModel.Applicant,
                    JobNo = updModel.JobNo,
                    UploadFileList = updModel.UploadFile,
                    RejectReason = updModel.RejectReason,
                    Remark = updModel.Remark
                },
                GetUserInfo());

                return Json(new { IsSuccess = _res.Item1, msg = _res.Item2 });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, msg = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Detail([FromQuery] int sn)
        {
            ViewBag.CategoryOptions = _optionDomainService.GetDemandCategoryOptionList();

            var _res = _demandDomainService.GetDemandDetail(sn, null);

            DemanEditViewModel _response = new DemanEditViewModel
            {
                OrderSn = sn,
                OrderId = _res.OrderNo,
                CreateDate = _res.CreateTimeStr,
                DemandCategoryId = _res.CategoryId,
                DemandStatus = _res.Status,
                DemandStatusId = _res.StatusId,
                Subject = _res.Subject,
                Content = _res.Content,
                Applicant = _res.Applicant,
                JobNo = _res.JobNo,
                UploadFile1 = _res.UploadFile1,
                UploadFile2 = _res.UploadFile2,
                UploadFile3 = _res.UploadFile3,
                RejectReason = _res.RejectReason,
                Remark = _res.Remark,
                CompleteUploadFile1 = _res.CompleteUploadFile1,
                CompleteUploadFile2 = _res.CompleteUploadFile2,
                CompleteUploadFile3 = _res.CompleteUploadFile3
            };

            return PartialView("_PartialDetail", _response);
        }

        [HttpGet]
        public IActionResult Download([FromQuery] int orderSn, int typeId, int fileNo)
        {
            try
            {
                var _res = _demandDomainService.GetDownFileStr(orderSn, typeId, fileNo);

                if (_res.Item1 == null)
                {
                    return RedirectToAction("Error", "Home", new ErrorViewModel { Message = _res.Item2 });
                }

                return File(_res.Item1, "application/octet-stream", _res.Item2);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            UserEntity _userEntity = GetUserInfo();

            ViewBag.CategoryOptions = _optionDomainService.GetDemandCategoryOptionList();
            ViewBag.AccountName = _userEntity.Name;
            ViewBag.JobId = _userEntity.JobId;
            return View();
        }

        [HttpPost]
        public IActionResult Create(DemanCreateViewModel createModel)
        {
            try
            {
                var _res = _demandDomainService.InsertDemand(new DemandEntity
                {
                    CategoryId = createModel.DemandCategoryId,
                    Subject = createModel.Subject,
                    Content = createModel.Content,
                    Applicant = createModel.Applicant,
                    JobNo = createModel.JobNo,
                    UploadFileList = createModel.UploadFile
                }, GetUserInfo());

                return Json(new { IsSuccess = _res.Item1, msg = _res.Item2 });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, msg = ex.Message });
            }
        }


        private (bool, string) Edit(DemanEditViewModel updModel, DemandStatusEnum newStatusId)
        {
            try
            {
                var _res = _demandDomainService.UpdateDemand(new DemandEntity
                {
                    OrderSn = updModel.OrderSn,
                    OrderNo = updModel.OrderId,
                    CategoryId = updModel.DemandCategoryId,
                    Subject = updModel.Subject,
                    Content = updModel.Content,
                    Applicant = updModel.Applicant,
                    JobNo = updModel.JobNo,
                    UploadFileList = updModel.UploadFile,
                    RejectReason = updModel.RejectReason,
                    Remark = updModel.Remark
                },
                GetUserInfo());

                return _res;
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
