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
    public class SPCReportController : BaseController
    {
        private readonly ILogger<SPCReportController> _logger;
        private readonly IOptionDomainService _optionDomainService;
        private readonly ISPCReportDomainService _spcReportDomainService;

        public SPCReportController(ISPCReportDomainService spcReportDomainService,
            IHttpContextAccessor httpContextAccessor,
            IAccountDomainService accountDomainService,
            IOptionDomainService optionDomainService,
            ILogger<SPCReportController> logger)
            : base(httpContextAccessor, accountDomainService)
        {
            _spcReportDomainService = spcReportDomainService;
            _optionDomainService = optionDomainService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                var _allOptions = _optionDomainService.GetSPCChartCategoryOptions();

                ViewBag.Floor = new SelectList(_allOptions.FirstOrDefault(f => f.Item1 == "floor").Item2, "Value", "Value");
                ViewBag.ChartGrade = new SelectList(_allOptions.FirstOrDefault(f => f.Item1 == "chartgrade").Item2, "Value", "Value");

                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel
                {
                    Message = ex.Message
                });
            }
        }

        public IActionResult Search([FromQuery] int floor, string chartgrade, string dateRange, string eqpId, string prodId, string dataGroup)
        {
            try
            {
                var _resilt = _spcReportDomainService.Search(floor, chartgrade, dateRange, eqpId, prodId, dataGroup);

                var _response = _resilt.Select(res => new SPCMainViewModel
                {
                    EquiomentId = res.EquipmentId,
                    ProductId = res.ProductId,
                    DataGroup = res.DataGroup,
                    Count = res.Count,
                    OOSCount = res.OOSCount,
                    OOCCount = res.OOCCount,
                    OORCount = res.OORCount,
                }).ToList();

                return PartialView("_PartialDetail", _response);
            }
            catch (Exception ex)
            {
                return Json(new { IsException = true, Msg = ex.Message });
            }
        }

        public IActionResult GetMainOptions([FromQuery] int floor, string chartgrade)
        {
            try
            {
                var _allOptions = _optionDomainService.GetSPCMainChartOptions(floor, chartgrade);

                var _response = _allOptions.CopyAToB<SPCChartSettingViewModel>().ToList();

                return Json(_response);
            }
            catch (Exception ex)
            {
                return Json(new { IsException = true, Msg = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Detail([FromQuery] int floor, string chartgrade, string dateRange, string eqpId, string prodId, string dataGroup)
        {
            try
            {
                var _resilt = _spcReportDomainService.Detail(floor, chartgrade, dateRange, eqpId, prodId, dataGroup);

                var _response = new SPCOnlineChartViewModel
                {
                    ChartId = _resilt.ChartId,
                    TypeStr = _resilt.TypeStr,
                    TestItem = _resilt.TestItem,
                    XBarBar = _resilt.XBarBar,
                    Sigma = _resilt.Sigma,
                    Ca = _resilt.Ca,
                    Cp = _resilt.Cp,
                    Cpk = _resilt.Cpk,
                    Sample = _resilt.Sample,
                    n = _resilt.n,
                    RMBar = _resilt.RMBar,
                    PpkBar = _resilt.PpkBar,
                    PpkSigma = _resilt.PpkSigma,
                    Pp = _resilt.Pp,
                    Ppk = _resilt.Ppk,
                    SPCDetail = _resilt.DetailList.Select(detail => new SPCDataViewModel
                    {
                        MeasureDateStr = detail.MeasureDateStr,
                        MeasureTimeStr = detail.MeasureTimeStr,
                        SHTId = detail.SHTId,
                        ProductId = detail.ProductId,
                        DataGroup = detail.DataGroup,
                        DTX = detail.DTX.ToString("0.###"),
                        USL = detail.USL.ToString("0.###"),
                        Target = detail.Target.ToString("0.###"),
                        LSL = detail.LSL.ToString("0.###"),
                        UCL1 = detail.UCL1.ToString("0.###"),
                        CL1 = detail.CL1.ToString("0.###"),
                        LCL1 = detail.LCL1.ToString("0.###"),
                        OOC1 = detail.OOC1,
                        OOS = detail.OOS,
                        DTRM = detail.DTRM.ToString("0.###"),
                        UCL2 = detail.UCL2.ToString("0.###"),
                        CL2 = detail.CL2.ToString("0.###"),
                        LCL2 = detail.LCL2.ToString("0.###"),
                        OOC2 = detail.OOC2,
                    }).ToList()
                };

                return View(_response);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }



        #region ===== SPC setting =====

        [HttpGet("[controller]/Setting")]
        public IActionResult Setting()
        {
            try
            {
                var _allOptions = _optionDomainService.GetSPCChartCategoryOptions();

                ViewBag.Floor = new SelectList(_allOptions.FirstOrDefault(f => f.Item1 == "floor").Item2, "Value", "Value");
                ViewBag.ChartGrade = new SelectList(_allOptions.FirstOrDefault(f => f.Item1 == "chartgrade").Item2, "Value", "Value");

                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }


        [HttpGet("[controller]/Setting/Search")]
        public IActionResult SettingSearch([FromQuery] int floor, string chartgrade, string prodId)
        {
            try
            {
                var _targetSettingList = _spcReportDomainService.GetSettingList(0, floor, chartgrade, prodId);

                var _res = _targetSettingList.Select(setting => new SPCSettingViewModel
                {
                    ProductId = setting.PECD,
                    OnchType = setting.ONCHTYPE,
                    DataGroup = setting.DataGroup,
                    Node = setting.PROC_ID,
                    Chartgrade = setting.CHARTGRADE,
                    USPEC = setting.USPEC.ToString("0.###"),
                    LSPEC = setting.LSPEC.ToString("0.###"),
                    UCL1 = setting.UCL1.ToString("0.###"),
                    CL1 = setting.CL1.ToString("0.###"),
                    LCL1 = setting.LCL1.ToString("0.###"),
                    UCL2 = setting.UCL2.ToString("0.###"),
                    CL2 = setting.CL2.ToString("0.###"),
                    LCL2 = setting.LCL2.ToString("0.###"),
                }).ToList();

                return PartialView("_PartialSetting", _res);
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

                return Json("");

            }
            catch (Exception ex)
            {
                return Json($"錯誤：{ex.Message}");
            }
        }
        #endregion
    }
}
