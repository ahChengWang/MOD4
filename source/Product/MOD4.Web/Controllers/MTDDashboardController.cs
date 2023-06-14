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
                var _resilt = _mtdDashboardDomainService.DashboardSearch(isMass: true);

                List<MTDDashboardMainViewModel> _response = _resilt.Select(mtd => new MTDDashboardMainViewModel
                {
                    Process = mtd.Process,
                    Plan = mtd.MTDSubList.Sum(sub => sub.Plan).ToString("#,0"),
                    Actual = mtd.MTDSubList.Sum(sub => sub.Actual).ToString("#,0"),
                    Diff = mtd.MTDSubList.Sum(sub => sub.Diff).ToString("#,0"),
                    ProcessList = mtd.MTDSubList.Select(s => new MTDDashboardViewModel
                    {
                        Process = s.Process,
                        Plan = s.Plan.ToString("#,0"),
                        Actual = s.Actual.ToString("#,0"),
                        Diff = s.Diff.ToString("#,0"),
                        DownTime = s.DownTime,
                        DownPercent = s.DownPercent,
                        UPPercent = s.UPPercent,
                        RUNPercent = s.RUNPercent,
                        UPHPercent = s.UPHPercent,
                        OEEPercent = s.OEEPercent,
                        MTDDetail = s.MTDDetail.Select(detail => new MTDDashboardDetailViewModel
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
        public IActionResult Search([FromQuery] string date, decimal time, int floor, int owner)
        {
            try
            {
                var _resilt = _mtdDashboardDomainService.DashboardSearch(floor, date, time, owner == 1);

                List<MTDDashboardMainViewModel> _response = _resilt.Select(mtd => new MTDDashboardMainViewModel
                {
                    Process = mtd.Process,
                    Plan = mtd.MTDSubList.Sum(sub => sub.Plan).ToString("#,0"),
                    Actual = mtd.MTDSubList.Sum(sub => sub.Actual).ToString("#,0"),
                    Diff = mtd.MTDSubList.Sum(sub => sub.Diff).ToString("#,0"),
                    ProcessList = mtd.MTDSubList.Select(s => new MTDDashboardViewModel
                    {
                        Process = s.Process,
                        Plan = s.Plan.ToString("#,0"),
                        Actual = s.Actual.ToString("#,0"),
                        Diff = s.Diff.ToString("#,0"),
                        DownTime = s.DownTime,
                        DownPercent = s.DownPercent,
                        UPPercent = s.UPPercent,
                        RUNPercent = s.RUNPercent,
                        UPHPercent = s.UPHPercent,
                        OEEPercent = s.OEEPercent,
                        MTDDetail = s.MTDDetail.Select(detail => new MTDDashboardDetailViewModel
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
                    }).ToList()
                }).ToList();

                return PartialView("_PartialDashboard", _response);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("[controller]/MTDINT0")]
        public IActionResult MTDINT0()
        {
            try
            {
                var _resilt = _mtdDashboardDomainService.DashboardSearch(isMass: false);

                List<MTDDashboardMainViewModel> _response = _resilt.Select(mtd => new MTDDashboardMainViewModel
                {
                    Process = mtd.Process,
                    Plan = mtd.MTDSubList.Sum(sub => sub.Plan).ToString("#,0"),
                    Actual = mtd.MTDSubList.Sum(sub => sub.Actual).ToString("#,0"),
                    Diff = mtd.MTDSubList.Sum(sub => sub.Diff).ToString("#,0"),
                    ProcessList = mtd.MTDSubList.Select(s => new MTDDashboardViewModel
                    {
                        Process = s.Process,
                        Plan = s.Plan.ToString("#,0"),
                        Actual = s.Actual.ToString("#,0"),
                        Diff = s.Diff.ToString("#,0"),
                        DownTime = s.DownTime,
                        DownPercent = s.DownPercent,
                        UPPercent = s.UPPercent,
                        RUNPercent = s.RUNPercent,
                        UPHPercent = s.UPHPercent,
                        OEEPercent = s.OEEPercent,
                        MTDDetail = s.MTDDetail.Select(detail => new MTDDashboardDetailViewModel
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

        [HttpGet("[controller]/MTDINT0/Search")]
        public IActionResult MTDINT0Search([FromQuery] string date, decimal time, int floor, int owner)
        {
            try
            {
                var _resilt = _mtdDashboardDomainService.DashboardSearch(floor, date, time, owner == 1);

                List<MTDDashboardMainViewModel> _response = _resilt.Select(mtd => new MTDDashboardMainViewModel
                {
                    Process = mtd.Process,
                    Plan = mtd.MTDSubList.Sum(sub => sub.Plan).ToString("#,0"),
                    Actual = mtd.MTDSubList.Sum(sub => sub.Actual).ToString("#,0"),
                    Diff = mtd.MTDSubList.Sum(sub => sub.Diff).ToString("#,0"),
                    ProcessList = mtd.MTDSubList.Select(s => new MTDDashboardViewModel
                    {
                        Process = s.Process,
                        Plan = s.Plan.ToString("#,0"),
                        Actual = s.Actual.ToString("#,0"),
                        Diff = s.Diff.ToString("#,0"),
                        DownTime = s.DownTime,
                        DownPercent = s.DownPercent,
                        UPPercent = s.UPPercent,
                        RUNPercent = s.RUNPercent,
                        UPHPercent = s.UPHPercent,
                        OEEPercent = s.OEEPercent,
                        MTDDetail = s.MTDDetail.Select(detail => new MTDDashboardDetailViewModel
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
                ViewBag.ProdOptions = _optionDomainService.GetLcmProdOptions();

                var _result = _mtdDashboardDomainService.Search();

                UserEntity _userInfo = GetUserInfo();
                var _userCurrentPagePermission = _userInfo.UserMenuPermissionList.FirstOrDefault(f => f.MenuSn == MenuEnum.Manufacture);
                ViewBag.UserPermission = new UserPermissionViewModel
                {
                    AccountSn = _userCurrentPagePermission.AccountSn,
                    MenuSn = _userCurrentPagePermission.MenuSn,
                    AccountPermission = _userCurrentPagePermission.AccountPermission
                };

                List<MftrScheduleViewModel> _reponse = _result.Select(res => new MftrScheduleViewModel
                {
                    Sn = res.Sn,
                    Process = res.Process,
                    Date = res.Date,
                    DateStart = res.DateStart,
                    Project = res.ProdDesc,
                    Product = res.ProdNo,
                    ProductId = res.LcmProdId,
                    Quantity = res.Qty,
                    IsMass = res.IsMass,
                }).ToList();

                return View(_reponse);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpGet("[controller]/Manufacture/Search/{mtdCategoryId}/{floor}")]
        public IActionResult ManufactureSearch(MTDCategoryEnum mtdCategoryId, int floor)
        {
            try
            {
                var _result = _mtdDashboardDomainService.Search(floor: floor, mtdCategoryId: mtdCategoryId);

                UserEntity _userInfo = GetUserInfo();
                var _userCurrentPagePermission = _userInfo.UserMenuPermissionList.FirstOrDefault(f => f.MenuSn == MenuEnum.Manufacture);
                ViewBag.UserPermission = new UserPermissionViewModel
                {
                    AccountSn = _userCurrentPagePermission.AccountSn,
                    MenuSn = _userCurrentPagePermission.MenuSn,
                    AccountPermission = _userCurrentPagePermission.AccountPermission
                };

                List<MftrScheduleViewModel> _reponse = _result.Select(res => new MftrScheduleViewModel
                {
                    Sn = res.Sn,
                    Process = res.Process,
                    Date = res.Date,
                    DateStart = res.DateStart,
                    Project = res.ProdDesc,
                    Product = res.ProdNo,
                    ProductId = res.LcmProdId,
                    Quantity = res.Qty,
                    IsMass = res.IsMass,
                }).ToList();

                if (_reponse.Any())
                    return Json(new ResponseViewModel<List<MftrScheduleViewModel>>
                    {
                        IsSuccess = true,
                        Data = _reponse
                    });
                else
                    return Json(new ResponseViewModel<object>
                    {
                        IsSuccess = false,
                        Msg = "查無排程"
                    });

            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
        }

        [HttpPost("[controller]/Manufacture/Create")]
        public IActionResult MftrCreate(MftrScheduleViewModel createMftrVM)
        {
            try
            {
                var _createRes = _mtdDashboardDomainService.Create(new MftrScheduleEntity
                {
                    Date = createMftrVM.Date,
                    IsMass = createMftrVM.IsMass,
                    LcmProdId = createMftrVM.ProductId,
                    Qty = createMftrVM.Quantity,
                    MTDCategoryId = createMftrVM.MTDCategoryId,
                    Floor = createMftrVM.Floor
                },
                GetUserInfo());

                return Json(new ResponseViewModel<int>
                {
                    IsSuccess = _createRes.Item1 == "",
                    Data = _createRes.Item2,
                    Msg = _createRes.Item1 == "" ? "" : _createRes.Item1
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseViewModel<string>
                {
                    IsSuccess = false,
                    Msg = ex.Message
                });
            }
        }

        [HttpPost("[controller]/Manufacture/BatchCreate")]
        public IActionResult MftrBatchCreate(MftrScheduleViewModel createMftrVM)
        {
            try
            {
                var _createRes = _mtdDashboardDomainService.Create(new MftrScheduleEntity
                {
                    Date = createMftrVM.Date,
                    IsMass = createMftrVM.IsMass,
                    LcmProdId = createMftrVM.ProductId,
                    Qty = createMftrVM.Quantity,
                    MTDCategoryId = createMftrVM.MTDCategoryId,
                    Floor = createMftrVM.Floor
                },
                GetUserInfo());

                return Json(new ResponseViewModel<int>
                {
                    IsSuccess = _createRes.Item1 == "",
                    Data = _createRes.Item2,
                    Msg = _createRes.Item1 == "" ? "" : _createRes.Item1
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseViewModel<string>
                {
                    IsSuccess = false,
                    Msg = ex.Message
                });
            }
        }


        [HttpPost("[controller]/Manufacture/Update")]
        public IActionResult MftrUpdate(MftrScheduleViewModel updateMftrVM)
        {
            try
            {
                var _updateRes = _mtdDashboardDomainService.Update(new MftrScheduleEntity
                {
                    Sn = updateMftrVM.Sn,
                    Date = updateMftrVM.Date,
                    IsMass = updateMftrVM.IsMass,
                    LcmProdId = updateMftrVM.ProductId,
                    Qty = updateMftrVM.Quantity,
                    IsDrop = updateMftrVM.IsDrop
                },
                GetUserInfo());

                return Json(new ResponseViewModel<int>
                {
                    IsSuccess = _updateRes == "",
                    Msg = _updateRes == "" ? "" : _updateRes
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseViewModel<string>
                {
                    IsSuccess = false,
                    Msg = ex.Message
                });
            }
        }

        [HttpGet("[controller]/Manufacture/LatestUpdate/{floor}/{owner}")]
        public IActionResult ManufactureLatestUpdate(int floor, int owner)
        {
            try
            {
                var _result = _mtdDashboardDomainService.GetLatestUpdate(floor, owner);

                return Json(_result);
            }
            catch (Exception ex)
            {
                return Json($"錯誤：{ex.Message}");
            }
        }

        [HttpPost("[controller]/Manufacture/Upload")]
        public IActionResult ManufactureUpload([FromForm] IFormFile updFile, int floor, int owner)
        {
            try
            {
                var _result = _mtdDashboardDomainService.Upload(updFile, floor, owner, GetUserInfo());

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
        public IActionResult MTBFMTTRSearch([FromQuery] string beginDate, string endDate, string equipment, int floor)
        {
            try
            {
                var _resultList = _mtdDashboardDomainService.GetMTBFMTTRList(beginDate, endDate, equipment, floor);

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
                    MTTRTarget = Convert.ToDecimal(settingVM.MTTRTarget),
                    Floor = settingVM.Floor
                }, GetUserInfo());

                if (_result != "")
                    return Json(new ResponseViewModel<string>
                    {
                        IsSuccess = false,
                        Msg = _result
                    });
                else
                    return Json(new ResponseViewModel<string>
                    {
                        IsSuccess = true,
                        Data = _result
                    });
            }
            catch (Exception ex)
            {
                return Json(new ResponseViewModel<string>
                {
                    IsSuccess = false,
                    Msg = ex.Message
                });
            }
        }
        #endregion
    }
}
