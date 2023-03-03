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
                        DTX = detail.DTX.ToString("0.#####"),
                        USL = detail.USL.ToString("0.#####"),
                        Target = detail.Target.ToString("0.#####"),
                        LSL = detail.LSL.ToString("0.#####"),
                        UCL1 = detail.UCL1.ToString("0.#####"),
                        CL1 = detail.CL1.ToString("0.#####"),
                        LCL1 = detail.LCL1.ToString("0.#####"),
                        OOC1 = detail.OOC1,
                        OOS = detail.OOS,
                        DTRM = detail.DTRM.ToString("0.#####"),
                        UCL2 = detail.UCL2.ToString("0.#####"),
                        CL2 = detail.CL2.ToString("0.#####"),
                        LCL2 = detail.LCL2.ToString("0.#####"),
                        OOC2 = detail.OOC2,
                        OOR1 = detail.OOR1,
                        OOR2 = detail.OOR2
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

                UserEntity _userInfo = GetUserInfo();
                var _userCurrentPagePermission = _userInfo.UserMenuPermissionList.FirstOrDefault(f => f.MenuSn == MenuEnum.SPCParaSetting);

                var _res = _targetSettingList.Select(setting => new SPCSettingViewModel
                {
                    Sn = setting.sn,
                    ProductId = setting.PECD,
                    OnchType = setting.ONCHTYPE,
                    DataGroup = setting.DataGroup,
                    Node = setting.PROC_ID,
                    Chartgrade = setting.CHARTGRADE,
                    USPEC = setting.USPEC.ToString("0.#####"),
                    LSPEC = setting.LSPEC.ToString("0.#####"),
                    UCL1 = setting.UCL1.ToString("0.#####"),
                    CL1 = setting.CL1.ToString("0.#####"),
                    LCL1 = setting.LCL1.ToString("0.#####"),
                    UCL2 = setting.UCL2.ToString("0.#####"),
                    CL2 = setting.CL2.ToString("0.#####"),
                    LCL2 = setting.LCL2.ToString("0.#####"),
                }).ToList();

                return PartialView("_PartialSetting", new SPCSettingMainViewModel
                {
                    SettingList = _res,
                    UserPermission = new UserPermissionViewModel
                    {
                        AccountSn = _userCurrentPagePermission.AccountSn,
                        MenuSn = _userCurrentPagePermission.MenuSn,
                        AccountPermission = _userCurrentPagePermission.AccountPermission
                    }
                });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }


        [HttpGet]
        public IActionResult Edit([FromQuery] int sn)
        {
            try
            {
                var _targetSettingList = _spcReportDomainService.GetSettingEdit(sn);

                return PartialView("_PartialSettingEdit", new SPCSettingEditViewModel()
                {
                    Sn = _targetSettingList.sn,
                    ProductId = _targetSettingList.PECD,
                    OnchType = _targetSettingList.ONCHTYPE,
                    DataGroup = _targetSettingList.DataGroup,
                    Node = _targetSettingList.PROC_ID,
                    Chartgrade = _targetSettingList.CHARTGRADE,
                    USPEC = _targetSettingList.USPEC.ToString("0.#####"),
                    LSPEC = _targetSettingList.LSPEC.ToString("0.#####"),
                    Last3MonCPK = _targetSettingList.Last3MonCPK.ToString("0.#####"),
                    Last2MonCPK = _targetSettingList.Last2MonCPK.ToString("0.#####"),
                    LastMonCPK = _targetSettingList.LastMonCPK.ToString("0.#####"),
                    LastMonCL1 = _targetSettingList.LastMonCL1.ToString("0.#####"),
                    LastMonUCL1 = _targetSettingList.LastMonUCL1.ToString("0.#####"),
                    LastMonLCL1 = _targetSettingList.LastMonLCL1.ToString("0.#####"),
                    LastMonCL2 = _targetSettingList.LastMonCL2.ToString("0.#####"),
                    LastMonUCL2 = _targetSettingList.LastMonUCL2.ToString("0.#####"),
                    LastMonLCL2 = _targetSettingList.LastMonLCL2.ToString("0.#####"),
                    UCL1 = _targetSettingList.UCL1.ToString("0.#####"),
                    CL1 = _targetSettingList.CL1.ToString("0.#####"),
                    LCL1 = _targetSettingList.LCL1.ToString("0.#####"),
                    UCL2 = _targetSettingList.UCL2.ToString("0.#####"),
                    CL2 = _targetSettingList.CL2.ToString("0.#####"),
                    LCL2 = _targetSettingList.LCL2.ToString("0.#####"),
                    NewCL1 = _targetSettingList.NewCL1,
                    NewUCL1 = _targetSettingList.NewUCL1,
                    NewLCL1 = _targetSettingList.NewLCL1,
                    NewCL2 = _targetSettingList.NewCL2,
                    NewUCL2 = _targetSettingList.NewUCL2,
                    NewLCL2 = _targetSettingList.NewLCL2,
                    Memo = _targetSettingList.Memo
                });
            }
            catch (Exception ex)
            {
                return Json($"錯誤：{ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult Edit([FromForm] SPCSettingEditViewModel updateSPCSettingEditVM)
        {
            try
            {

                var _updResult = _spcReportDomainService.UpdateSPCSetting(new SPCChartSettingEntity
                {
                    sn = updateSPCSettingEditVM.Sn,
                    NewUCL1 = updateSPCSettingEditVM.NewUCL1,
                    NewCL1 = updateSPCSettingEditVM.NewCL1,
                    NewLCL1 = updateSPCSettingEditVM.NewLCL1,
                    NewUCL2 = updateSPCSettingEditVM.NewUCL2,
                    NewCL2 = updateSPCSettingEditVM.NewCL2,
                    NewLCL2 = updateSPCSettingEditVM.NewLCL2,
                    Memo = updateSPCSettingEditVM.Memo
                }
                , GetUserInfo());

                return Json(_updResult);

            }
            catch (Exception ex)
            {
                return Json($"錯誤：{ex.Message}");
            }
        }
        #endregion
    }
}
