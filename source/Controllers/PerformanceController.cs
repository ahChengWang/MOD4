using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MOD4.Web.DomainService;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Helper;
using MOD4.Web.Models;
using MOD4.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.Controllers
{
    [Authorize]
    public class PerformanceController : Controller
    {
        private readonly ILogger<PerformanceController> _logger;
        private readonly IPerformanceDomainService _performanceDomainService;
        private readonly ITargetSettingDomainService _targetSettingDomainService;
        private readonly IOptionDomainService _optionDomainService;

        public PerformanceController(ILogger<PerformanceController> logger,
            IPerformanceDomainService performanceDomainService,
            ITargetSettingDomainService targetSettingDomainService,
            IOptionDomainService optionDomainService)
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
                ViewBag.ProdOptions = _optionDomainService.GetLcmProdOptions();

                var resule = _performanceDomainService.GetList();

                return View(resule);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        public IActionResult Search(string mfgDay, string shift, string node, string prodList)
        {
            try
            {
                var resule = _performanceDomainService.GetList(mfgDay, prodList, shift, node);

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
                ViewBag.ProdOptions = _optionDomainService.GetLcmProdOptions();

                var _targetSettingList = _targetSettingDomainService.GetList();

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
                ViewBag.ProdOptions = _optionDomainService.GetLcmProdOptions();

                var _targetSettingList = _targetSettingDomainService.GetList();

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


        [HttpPost]
        public IActionResult Setting(TargetSettingViewModel updateMode)
        {
            try
            {
                var _res = updateMode.SettingDetailList.CopyAToB<TargetSettingEntity>();

                var _updateResult = _targetSettingDomainService.Update(_res);

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
    }
}
