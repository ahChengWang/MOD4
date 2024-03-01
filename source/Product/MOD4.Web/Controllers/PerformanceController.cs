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
                ViewBag.ProdOptions = _optionDomainService.GetLcmProdOptions();
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

                ViewBag.ProdOptions = _optionDomainService.GetLcmProdOptions();

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

        #region ===== efficiency dashboard =====

        [HttpGet("[controller]/ReclaimWT")]
        public IActionResult ReclaimWT()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        #endregion

    }
}
