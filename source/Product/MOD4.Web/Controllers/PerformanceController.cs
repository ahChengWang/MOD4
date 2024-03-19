using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MOD4.Web.DomainService;
using MOD4.Web.DomainService.Entity;
using Utility.Helper;
using MOD4.Web.Models;
using MOD4.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using MOD4.Web.Enum;

namespace MOD4.Web.Controllers
{
    [Authorize]
    public class PerformanceController : BaseController
    {
        private readonly ILogger<PerformanceController> _logger;
        private readonly IPerformanceDomainService _performanceDomainService;
        private readonly ITargetSettingDomainService _targetSettingDomainService;
        private readonly IOptionDomainService _optionDomainService;

        public PerformanceController(IHttpContextAccessor httpContextAccessor,
            IPerformanceDomainService performanceDomainService,
            ITargetSettingDomainService targetSettingDomainService,
            IOptionDomainService optionDomainService,
            IAccountDomainService accountDomainService,
            ILogger<PerformanceController> logger)
            : base(httpContextAccessor, accountDomainService)
        {
            _logger = logger;
            _performanceDomainService = performanceDomainService;
            _targetSettingDomainService = targetSettingDomainService;
            _optionDomainService = optionDomainService;
        }

        public IActionResult Index()
        {
            try
            {
                ViewData["ProdName"] = "GDD340IA0090S-34VCS";
                ViewBag.ProdOptions = _optionDomainService.GetLcmProdDescOptions();
                ViewBag.NodeOptions = _optionDomainService.GetNodeList();

                var resule = _performanceDomainService.GetList();

                return View(resule);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        public IActionResult Search(string mfgDay, string shift, string node, int owner, string prodList)
        {
            try
            {
                var resule = _performanceDomainService.GetList(mfgDay, prodList, shift, node, owner);

                if (!resule.Any())
                {
                    return Json("查無資料");
                }

                return PartialView(shift == "A" ? "_PartialEqTableA" : "_PartialEqTableB", resule);
            }
            catch (Exception ex)
            {
                return Json($"錯誤：{ex.Message}");
            }

        }


        #region ===== target setting =====

        [HttpGet("[controller]/Setting")]
        public IActionResult Setting()
        {
            try
            {
                //_targetSettingDomainService.Migration();

                ViewBag.ProdOptions = _optionDomainService.GetLcmProdDescOptions();

                var _targetSettingList = _targetSettingDomainService.GetList(new List<int> { 1206 });

                ViewData["ProdName"] = "GDD340IA0090S - 34VCS";
                ViewData["NodeTab"] = _targetSettingList.GroupBy(gb => gb.Node).Select(s => s.Key).ToList();

                var _res = _targetSettingList.CopyAToB<TargetSettingDetailModel>();

                return View(new TargetSettingViewModel
                {
                    SettingDetailList = _res
                });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }


        [HttpGet("[controller]/Setting/Search")]
        public IActionResult TargetSearch([FromQuery] int prodSn)
        {
            try
            {
                var _targetSettingList = _targetSettingDomainService.GetList(prodSn: new List<int> { prodSn });

                ViewData["NodeTab"] = _targetSettingList.GroupBy(gb => gb.Node).Select(s => s.Key).ToList();

                var _res = _targetSettingList.CopyAToB<TargetSettingDetailModel>();

                return PartialView("_PartialSettingTable", new TargetSettingViewModel
                {
                    SettingDetailList = _res
                });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }


        [HttpPost]
        public IActionResult Setting(TargetSettingViewModel updateMode)
        {
            try
            {
                var _res = updateMode.SettingDetailList.CopyAToB<TargetSettingEntity>();

                var _updateResult = _targetSettingDomainService.Update(updateMode.ProdSn, _res, GetUserInfo());

                if (_updateResult != "")
                    return Json(_updateResult);

                return Json("");

            }
            catch (Exception ex)
            {
                return Json($"錯誤：{ex.Message}");
            }
        }
        #endregion

        #region ===== efficiency dashboard =====

        [HttpGet("[controller]/Efficiency")]
        public IActionResult Efficiency()
        {
            try
            {
                var _result = _performanceDomainService.GetDailyEfficiencyList("", 3);

                List<EfficiencyViewModel> _respone = _result.Select(r => new EfficiencyViewModel
                {
                    Floor = r.Floor,
                    Process = r.Process,
                    TTLPassQty = r.TTLPassQty,
                    EfficiencyInlineTTL = r.EfficiencyInlineTTL,
                    EfficiencyInlineOfflineTTL = r.EfficiencyInlineOfflineTTL,
                    TTLcount = r.TTLcount,
                    InfoList = r.InfoList.Select(detail => new EfficiencyInfoViewModel
                    {
                        ProdNo = detail.ProdNo,
                        Shift = detail.Shift,
                        PassQty = detail.PassQty,
                        EfficiencyInline = detail.EfficiencyInline,
                        EfficiencyInlineOffline = detail.EfficiencyInlineOffline,
                        StartTime = detail.StartTime,
                        EndTime = detail.EndTime,
                        MedianTT = detail.MedianTT
                    }).ToList()
                }).ToList();

                return View(_respone);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpGet("[controller]/Efficiency/Search/{floor}/{date}")]
        public IActionResult EfficiencySearch(int floor, string date)
        {
            try
            {
                var _result = _performanceDomainService.GetDailyEfficiencyList(date, floor);

                List<EfficiencyViewModel> _respone = _result.Select(r => new EfficiencyViewModel
                {
                    Floor = r.Floor,
                    Process = r.Process,
                    TTLPassQty = r.TTLPassQty,
                    EfficiencyInlineTTL = r.EfficiencyInlineTTL,
                    EfficiencyInlineOfflineTTL = r.EfficiencyInlineOfflineTTL,
                    TTLcount = r.TTLcount,
                    InfoList = r.InfoList.Select(detail => new EfficiencyInfoViewModel
                    {
                        ProdNo = detail.ProdNo,
                        Shift = detail.Shift,
                        PassQty = detail.PassQty,
                        EfficiencyInline = detail.EfficiencyInline,
                        EfficiencyInlineOffline = detail.EfficiencyInlineOffline,
                        StartTime = detail.StartTime,
                        EndTime = detail.EndTime,
                        MedianTT = detail.MedianTT
                    }).ToList()
                }).ToList();

                return Json(new ResponseViewModel<List<EfficiencyViewModel>>
                {
                    IsSuccess = true,
                    Data = _respone
                });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpGet("[controller]/EffSetting")]
        public IActionResult EffSetting()
        {
            try
            {
                ViewBag.NodeList = _optionDomainService.GetNodeList(0).CopyAToB<OptionViewModel>();

                return View(GetEffSetting(2));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpGet("[controller]/EffSetting/Search/{floor}")]
        public IActionResult EffSettingSearch(int floor)
        {
            try
            {
                return Json(new ResponseViewModel<List<EfficiencySettingViewModel>>
                {
                    IsSuccess = true,
                    Data = GetEffSetting(floor)
                });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpPost("[controller]/EffSetting")]
        public IActionResult EffSetting(List<EfficiencySettingViewModel> editModelList)
        {
            try
            {
                List<EfficiencySettingEntity> _entitys = new List<EfficiencySettingEntity>();

                foreach (var model in editModelList)
                {
                    _entitys.AddRange(model.DetailList.Select(s => new EfficiencySettingEntity
                    {
                        LcmProdSn = model.ProdSn,
                        Shift = model.Shift,
                        Floor = model.Floor,
                        ProcessId = s.ProcessId,
                        Node = s.Node,
                        WT = s.WT,
                        InlineEmps = s.InlineEmps,
                        OfflineEmps = s.OfflineEmps
                    }));
                }

                var _result = _performanceDomainService.UpdateEfficiencySetting(_entitys, GetUserInfo());

                return Json(new ResponseViewModel<List<string>>
                {
                    IsSuccess = _result == "",
                    Msg = _result
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseViewModel<List<string>>
                {
                    IsSuccess = false,
                    Msg = ex.Message
                });
            }
        }

        private List<EfficiencySettingViewModel> GetEffSetting(int floor)
        {
            try
            {
                var _result = _performanceDomainService.GetEfficiencySetting(floor);

                List<EfficiencySettingViewModel> _response = _result.GroupBy(g => new { g.LcmProdSn, g.ProdNo, g.Shift })
                    .Select(setting => new EfficiencySettingViewModel
                    {
                        ProdSn = setting.Key.LcmProdSn,
                        ProdNo = setting.Key.ProdNo,
                        Shift = setting.Key.Shift,
                        ShiftDesc = setting.Key.Shift == "A" ? "日" : "夜",
                        DetailList = setting.Select(detail => new EfficiencySettingDetailViewModel
                        {
                            Process = detail.Process,
                            Node = detail.Node,
                            WT = detail.WT,
                            InlineEmps = detail.InlineEmps,
                            OfflineEmps = detail.OfflineEmps
                        }).ToList()
                    }).OrderBy(o => o.ProdNo).ToList();

                return _response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region ===== take back work time =====

        [HttpGet("[controller]/TakeBackWT")]
        public IActionResult TakeBackWT()
        {
            try
            {
                var _result = _performanceDomainService.GetTBWTList(null, 0);

                var _userCurrentPagePermission = GetUserInfo().UserMenuPermissionList.FirstOrDefault(f => f.MenuSn == MenuEnum.ReclaimWT);
                ViewBag.UserPermission = new UserPermissionViewModel
                {
                    AccountSn = _userCurrentPagePermission.AccountSn,
                    MenuSn = _userCurrentPagePermission.MenuSn,
                    AccountPermission = _userCurrentPagePermission.AccountPermission
                };

                var _response = _result.Select(s => new TakeBackWTViewModel
                {
                    Sn = s.Sn,
                    Date = s.Date,
                    WTCategoryId = (WTCategoryEnum)s.WTCategoryId,
                    TakeBackBonding = s.TakeBackBonding,
                    TakeBackFOG = s.TakeBackFOG,
                    TakeBackLAM = s.TakeBackLAM,
                    TakeBackASSY = s.TakeBackASSY,
                    TakeBackCDP = s.TakeBackCDP,
                    TakeBackPercent = s.TakeBackPercent,
                    TotalTakeBack = s.TotalTakeBack,
                    DetailList = s.DetailList.Select(detail => new TakeBackWTProdViewModel
                    {
                        TakeBackWTSn = detail.TakeBackWTSn,
                        ProcessId = detail.ProcessId,
                        EqId = detail.EqId,
                        ProdId = detail.ProdId,
                        Prod = detail.Prod,
                        IEStandard = detail.IEStandard,
                        IETT = detail.IETT,
                        IEWT = detail.IEWT,
                        PassQty = detail.PassQty,
                        TakeBackTime = detail.TakeBackTime
                    }).ToList(),
                    AttendanceList = s.AttendanceList.Select(atten => new TakeBackAttendanceViewModel
                    {
                        TakeBackWTSn = atten.TakeBackWTSn,
                        Country = atten.Country,
                        CountryId = atten.CountryId,
                        ShouldPresentCnt = atten.ShouldPresentCnt,
                        OverTimeCnt = atten.OverTimeCnt,
                        AcceptSupCnt = atten.AcceptSupCnt,
                        HaveDayOffCnt = atten.HaveDayOffCnt,
                        OffCnt = atten.OffCnt,
                        Support = atten.Support,
                        PresentCnt = atten.PresentCnt,
                        TotalWorkTime = atten.TotalWorkTime
                    }).ToList()
                });

                ViewBag.ProdOptions = JsonConvert.SerializeObject(_optionDomainService.GetLcmProdOptions());
                //ViewBag.BondEq = _allEqList2F.Where(w => w.AREA == "BONDING").Select(s => new OptionViewModel
                //{
                //    Value = s.EQUIP_NBR
                //});
                //ViewBag.FOGEq = _allEqList2F.Where(w => w.AREA == "LAM-FOG").Select(s => new OptionViewModel
                //{
                //    Value = s.EQUIP_NBR
                //});
                //ViewBag.LAMEq = _allEqList2F.Where(w => w.AREA == "LAM").Select(s => new OptionViewModel
                //{
                //    Value = s.EQUIP_NBR
                //});

                return View(_response);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpGet("[controller]/TakeBackWT/{pickDate}/{catg}")]
        public IActionResult TakeBackWT(DateTime pickDate, string catg)
        {
            try
            {
                var _result = _performanceDomainService.GetTBWTList(pickDate, (WTCategoryEnum)System.Enum.Parse(typeof(WTCategoryEnum), catg, true));

                var _response = _result.Select(s => new TakeBackWTViewModel
                {
                    Sn = s.Sn,
                    Date = s.Date,
                    WTCategoryId = (WTCategoryEnum)s.WTCategoryId,
                    TakeBackBonding = s.TakeBackBonding,
                    TakeBackFOG = s.TakeBackFOG,
                    TakeBackLAM = s.TakeBackLAM,
                    TakeBackASSY = s.TakeBackASSY,
                    TakeBackCDP = s.TakeBackCDP,
                    TakeBackPercent = s.TakeBackPercent,
                    TotalTakeBack = s.TotalTakeBack,
                    DetailList = s.DetailList.Select(detail => new TakeBackWTProdViewModel
                    {
                        TakeBackWTSn = detail.TakeBackWTSn,
                        ProcessId = detail.ProcessId,
                        EqId = detail.EqId,
                        ProdId = detail.ProdId,
                        Prod = detail.Prod,
                        IEStandard = detail.IEStandard,
                        IETT = detail.IETT,
                        IEWT = detail.IEWT,
                        PassQty = detail.PassQty,
                        TakeBackTime = detail.TakeBackTime
                    }).ToList(),
                    AttendanceList = s.AttendanceList.Select(atten => new TakeBackAttendanceViewModel
                    {
                        TakeBackWTSn = atten.TakeBackWTSn,
                        Country = atten.Country,
                        CountryId = atten.CountryId,
                        ShouldPresentCnt = atten.ShouldPresentCnt,
                        OverTimeCnt = atten.OverTimeCnt,
                        AcceptSupCnt = atten.AcceptSupCnt,
                        HaveDayOffCnt = atten.HaveDayOffCnt,
                        OffCnt = atten.OffCnt,
                        Support = atten.Support,
                        PresentCnt = atten.PresentCnt,
                        TotalWorkTime = atten.TotalWorkTime
                    }).ToList()
                });

                return Json(new ResponseViewModel<TakeBackWTViewModel>
                {
                    IsSuccess = true,
                    Data = _response.FirstOrDefault()
                });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpPost("[controller]/TakeBackWT")]
        public IActionResult TakeBackWT(TakeBackWTEditViewModel editVM)
        {
            try
            {
                var _result = _performanceDomainService.UpdateTakeBackWT(new TakeBackWTEntity
                {
                    Sn = editVM.Sn,
                    Date = editVM.Date,
                    WTCategoryId = editVM.WTCategoryId,
                    DetailList = editVM.DetailList.Select(detail => new TakeBackWTProdEntity
                    {
                        TakeBackWTSn = detail.TakeBackWTSn,
                        ProcessId = detail.ProcessId,
                        EqId = detail.EqId,
                        ProdId = detail.ProdId,
                    }).ToList(),
                    AttendanceList = editVM.AttendanceList.Select(atte => new TakeBackAttendanceEntity
                    {
                        TakeBackWTSn = atte.TakeBackWTSn,
                        CountryId = atte.CountryId,
                        ShouldPresentCnt = atte.ShouldPresentCnt,
                        OverTimeCnt = atte.OverTimeCnt,
                        AcceptSupCnt = atte.AcceptSupCnt,
                        Support = atte.Support,
                        HaveDayOffCnt = atte.HaveDayOffCnt,
                        OffCnt = atte.OffCnt,
                        PresentCnt = atte.PresentCnt
                    }).ToList()
                },
                GetUserInfo());

                if (string.IsNullOrEmpty(_result.Item1))
                    return Json(new ResponseViewModel<TakeBackWTViewModel>
                    {
                        IsSuccess = true,
                        Data = new TakeBackWTViewModel
                        {
                            Sn = _result.Item2.Sn,
                            Date = _result.Item2.Date,
                            WTCategoryId = (WTCategoryEnum)_result.Item2.WTCategoryId,
                            TakeBackBonding = _result.Item2.TakeBackBonding,
                            TakeBackFOG = _result.Item2.TakeBackFOG,
                            TakeBackLAM = _result.Item2.TakeBackLAM,
                            TakeBackASSY = _result.Item2.TakeBackASSY,
                            TakeBackCDP = _result.Item2.TakeBackCDP,
                            TakeBackPercent = _result.Item2.TakeBackPercent,
                            TotalTakeBack = _result.Item2.TotalTakeBack,
                            DetailList = _result.Item2.DetailList.Select(detail => new TakeBackWTProdViewModel
                            {
                                TakeBackWTSn = detail.TakeBackWTSn,
                                ProcessId = detail.ProcessId,
                                EqId = detail.EqId,
                                ProdId = detail.ProdId,
                                Prod = detail.Prod,
                                IEStandard = detail.IEStandard,
                                IETT = detail.IETT,
                                IEWT = detail.IEWT,
                                PassQty = detail.PassQty,
                                TakeBackTime = detail.TakeBackTime
                            }).ToList(),
                            AttendanceList = _result.Item2.AttendanceList.Select(atten => new TakeBackAttendanceViewModel
                            {
                                TakeBackWTSn = atten.TakeBackWTSn,
                                Country = atten.Country,
                                CountryId = atten.CountryId,
                                ShouldPresentCnt = atten.ShouldPresentCnt,
                                OverTimeCnt = atten.OverTimeCnt,
                                AcceptSupCnt = atten.AcceptSupCnt,
                                HaveDayOffCnt = atten.HaveDayOffCnt,
                                OffCnt = atten.OffCnt,
                                Support = atten.Support,
                                PresentCnt = atten.PresentCnt,
                                TotalWorkTime = atten.TotalWorkTime
                            }).ToList()
                        }
                    });
                else
                    return Json(new ResponseViewModel<string>
                    {
                        IsSuccess = false,
                        Msg = _result.Item1
                    });

            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }


        [HttpGet("[controller]/TakeBackKanban/{srcDate}/{wtCatg}")]
        public IActionResult TakeBackKanban(DateTime srcDate, int wtCatg)
        {
            try
            {
                var _result = _performanceDomainService.GetWTKanBan(srcDate, wtCatg);

                return Json(new ResponseViewModel<TakeBackKanbanViewModel>
                {
                    Data = new TakeBackKanbanViewModel
                    {
                        Date = _result.Date,
                        TakeBackDaily = _result.TotalTakeBack.ToString("0.00"),
                        TakeBackDailyPercent = _result.TakeBackPercent.ToString("0.00") + "%",
                        DetailList = _result.DetailList.Select(detail => new TakeBackInfoViewModel
                        {
                            Date = detail.Date,
                            ShortDate = detail.ShortDateStr,
                            TakeBackPercnet = detail.TakeBackPercent
                        }).ToList()
                    }
                });

            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        #endregion

    }
}
