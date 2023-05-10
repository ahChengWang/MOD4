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
using Utility.Helper;

namespace MOD4.Web.Controllers
{
    [Authorize]
    public class MTDDashboardController : BaseController
    {
        private readonly ILogger<SPCReportController> _logger;
        private readonly IOptionDomainService _optionDomainService;
        private readonly IMTDDashboardDomainService _mtdDashboardDomainService;

        public MTDDashboardController(IMTDDashboardDomainService mtdDashboardDomainService,
            IHttpContextAccessor httpContextAccessor,
            IAccountDomainService accountDomainService,
            IOptionDomainService optionDomainService,
            ILogger<SPCReportController> logger)
            : base(httpContextAccessor, accountDomainService)
        {
            _mtdDashboardDomainService = mtdDashboardDomainService;
            _optionDomainService = optionDomainService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                var _resilt = _mtdDashboardDomainService.DashboardSearch();

                List<MTDDashboardViewModel> _response = _resilt.Select(mtd => new MTDDashboardViewModel
                {
                    Process = mtd.Process,
                    Plan = mtd.Plan,
                    Actual = mtd.Actual,
                    Diff = mtd.Diff,
                    DownTime = mtd.DownTime,
                    DownPercent = mtd.DownPercent,
                    UPPercent = mtd.UPPercent,
                    RUNPercent = mtd.RUNPercent,
                    UPHPercent = mtd.UPHPercent,
                    OEEPercent = mtd.OEEPercent,
                    MTDDetail = mtd.MTDDetail.Select(detail => new MTDDashboardDetailViewModel
                    {
                        Date = detail.Date,
                        Equipment = detail.Equipment,
                        BigProduct = detail.BigProduct,
                        PlanProduct = detail.PlanProduct,
                        Output = detail.Output,
                        DayPlan = detail.DayPlan,
                        RangPlan = detail.RangPlan,
                        RangDiff = detail.RangDiff,
                        MonthPlan = detail.MonthPlan,
                        MTDPlan = detail.MTDPlan,
                        MTDActual = detail.MTDActual,
                        MTDDiff = detail.MTDDiff,
                        EqAbnormal = "",
                        RepaireTime = "",
                        Status = ""
                    }).ToList()
                }).ToList();

                return View(_response);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel
                {
                    Message = ex.Message
                });
            }
        }

        [HttpGet]
        public IActionResult Search([FromQuery] string date, decimal time, int floor)
        {
            try
            {
                var _resilt = _mtdDashboardDomainService.DashboardSearch(floor, date, time);

                List<MTDDashboardViewModel> _response = _resilt.Select(mtd => new MTDDashboardViewModel
                {
                    Process = mtd.Process,
                    Plan = mtd.Plan,
                    Actual = mtd.Actual,
                    Diff = mtd.Diff,
                    DownTime = mtd.DownTime,
                    DownPercent = mtd.DownPercent,
                    UPPercent = mtd.UPPercent,
                    RUNPercent = mtd.RUNPercent,
                    UPHPercent = mtd.UPHPercent,
                    OEEPercent = mtd.OEEPercent,
                    MTDDetail = mtd.MTDDetail.Select(detail => new MTDDashboardDetailViewModel
                    {
                        Date = detail.Date,
                        Equipment = detail.Equipment,
                        BigProduct = detail.BigProduct,
                        PlanProduct = detail.PlanProduct,
                        Output = detail.Output,
                        DayPlan = detail.DayPlan,
                        RangPlan = detail.RangPlan,
                        RangDiff = detail.RangDiff,
                        MonthPlan = detail.MonthPlan,
                        MTDPlan = detail.MTDPlan,
                        MTDActual = detail.MTDActual,
                        MTDDiff = detail.MTDDiff,
                        EqAbnormal = detail.EqAbnormal,
                        RepaireTime = detail.RepaireTime,
                        Status = detail.Status
                    }).ToList()
                }).ToList();

                return PartialView("_PartialDashboard", _response);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #region ===== Manufacture schedule =====

        [HttpGet("[controller]/Manufacture")]
        public IActionResult Manufacture()
        {
            try
            {
                var _result = _mtdDashboardDomainService.Search();

                ViewBag.UpdateInfo = _result.latestUpdInfo;

                UserEntity _userInfo = GetUserInfo();
                var _userCurrentPagePermission = _userInfo.UserMenuPermissionList.FirstOrDefault(f => f.MenuSn == MenuEnum.Manufacture);
                ViewBag.UserPermission = new UserPermissionViewModel
                {
                    AccountSn = _userCurrentPagePermission.AccountSn,
                    MenuSn = _userCurrentPagePermission.MenuSn,
                    AccountPermission = _userCurrentPagePermission.AccountPermission
                };

                ViewBag.UpdateInfo = _result.latestUpdInfo;

                List<ManufactureViewModel> _reponse = new List<ManufactureViewModel>();

                _result.manufactureSchedules.ForEach(mtd =>
                {
                    _reponse.Add(new ManufactureViewModel
                    {
                        Process = mtd.Process,
                        Category = mtd.Category,
                        MonthPlan = mtd.MonthPlan,
                        ProductName = mtd.ProductName,
                        PlanDetail = mtd.PlanDetail.Select(s => new ManufactureDetailViewModel
                        {
                            Date = s.Date,
                            Quantity = s.Quantity,
                            IsToday = s.IsToday
                        }).ToList()
                    });
                });

                return View(_reponse);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpGet("[controller]/Manufacture/Search")]
        public IActionResult ManufactureSearch([FromQuery] string dateRange, int floor)
        {
            try
            {
                var _result = _mtdDashboardDomainService.Search(dateRange, floor);

                UserEntity _userInfo = GetUserInfo();
                var _userCurrentPagePermission = _userInfo.UserMenuPermissionList.FirstOrDefault(f => f.MenuSn == MenuEnum.Manufacture);
                ViewBag.UserPermission = new UserPermissionViewModel
                {
                    AccountSn = _userCurrentPagePermission.AccountSn,
                    MenuSn = _userCurrentPagePermission.MenuSn,
                    AccountPermission = _userCurrentPagePermission.AccountPermission
                };

                List<ManufactureViewModel> _response = new List<ManufactureViewModel>();

                _result.manufactureSchedules.ForEach(mtd =>
                {
                    _response.Add(new ManufactureViewModel
                    {
                        Process = mtd.Process,
                        Category = mtd.Category,
                        MonthPlan = mtd.MonthPlan,
                        ProductName = mtd.ProductName,
                        PlanDetail = mtd.PlanDetail.Select(s => new ManufactureDetailViewModel
                        {
                            Date = s.Date,
                            Quantity = s.Quantity,
                            IsToday = s.IsToday
                        }).ToList()
                    });
                });

                if (_response.Any())
                {
                    ViewBag.UpdateInfo = _result.latestUpdInfo;

                    return PartialView("_PartialTable", _response);
                }
                else
                    return Json(new { message = "查無排程" });

            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
        }

        [HttpPost("[controller]/Manufacture/Upload")]
        public IActionResult ManufactureUpload([FromForm] IFormFile updFile, int floor)
        {
            try
            {
                var _result = _mtdDashboardDomainService.Upload(updFile, floor, GetUserInfo());

                return Json(_result);
            }
            catch (Exception ex)
            {
                return Json($"錯誤：{ex.Message}");
            }
        }

        #endregion

        #region === MTBF 、 MTTR ===

        [HttpGet("[controller]/MTBFMTTR")]
        public IActionResult MTBFMTTR()
        {
            try
            {
                ViewBag.EqIDMappingOption = _optionDomainService.GetEqIDAreaList();

                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpGet("[controller]/MTBFMTTR/Search")]
        public IActionResult MTBFMTTRSearch([FromQuery] string beginDate, string endDate, string equipment)
        {
            try
            {
                var _resultList = _mtdDashboardDomainService.GetMTBFMTTRList(beginDate, endDate, equipment);

                if (_resultList == null)
                    return Json(new { IsSuccess = false, Msg = "No Data" });

                MTBFMTTRDashboardViewModel _response = new MTBFMTTRDashboardViewModel
                {
                    MTBFTarget = _resultList.MTBFTarget,
                    MTBFActual = _resultList.MTBFActual,
                    MTTRTarget = _resultList.MTTRTarget,
                    MTTRActual = _resultList.MTTRActual,
                    MTTRDetail = _resultList.MTTRDetail.Select(mttr => new MTTRDetailViewModel
                    {
                        DownCode = mttr.DownCode,
                        AvgTime = mttr.AvgTime
                    }).ToList(),
                    EqpInfoDetail = _resultList.EqpInfoDetail.Select(eq => new MTBFMTTREqInfoViewModel
                    {
                        Code = eq.ToolStatus,
                        CodeDesc = eq.StatusCdsc,
                        Comments = eq.Comment,
                        Operator = eq.UserId,
                        StartTime = eq.LmTime.ToString("yyyy/MM/dd HH:mm:ss"),
                        RepairTime = eq.RepairTime
                    }).CopyAToB<MTBFMTTREqInfoViewModel>()
                };

                return Json(new { IsSuccess = true, Data = _response });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }

        [HttpGet("[controller]/MTBFMTTR/Setting")]
        public IActionResult MTBFMTTRSetting()
        {
            try
            {
                ViewBag.EqIDMappingOption = _optionDomainService.GetEqIDAreaList();

                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpPost("[controller]/MTBFMTTR/Setting")]
        public IActionResult MTBFMTTRSetting([FromForm] MTBFMTTRTargetSettingViewModel settingVM)
        {
            try
            {
                var _result = _mtdDashboardDomainService.UpdateMTBFMTTRSetting(new EqMappingEntity
                {
                    EQUIP_NBR = settingVM.EquipNo,
                    MTBFTarget = Convert.ToDecimal(settingVM.MTBFTarget),
                    MTTRTarget = Convert.ToDecimal(settingVM.MTTRTarget)
                }, GetUserInfo());

                if (_result != "")
                    return Json(new ResponseViewModel 
                    {
                        IsSuccess = false,
                        Msg = _result
                    });
                else
                    return Json(new ResponseViewModel
                    {
                        IsSuccess = true,
                        Data = _result
                    });
            }
            catch (Exception ex)
            {
                return Json(new ResponseViewModel
                {
                    IsSuccess = false,
                    Msg = ex.Message
                });
            }
        }
        #endregion
    }
}
