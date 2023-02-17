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
            ViewBag.ProdOptions = _optionDomainService.GetLcmProdOptions();
            List<OptionEntity> _options = new List<OptionEntity>
            {
                new OptionEntity{Id = 0, Value = "BA_A_BSP011Lx_L" },
                new OptionEntity{Id = 1, Value = "BA_A_BSP011Ly_L" },
                new OptionEntity{Id = 2, Value = "DW_A_BSP011Ly_L" },
                new OptionEntity{Id = 3, Value = "BA_A_BSP016Rx_L" },
                new OptionEntity{Id = 4, Value = "BA_A_BSP016Ry_L" },
                new OptionEntity{Id = 5, Value = "DW_A_BSP016Ry_L" },
                new OptionEntity{Id = 5, Value = "BA_A_BSC020Ly_L" },
            };

            ViewBag.TestItemOptions = new SelectList(_options, "Value", "Value");

            return View();
        }

        public IActionResult Search([FromQuery] string dateRange, string eqpId, string prodId, string dataGroup)
        {
            try
            {
                var _resilt = _spcReportDomainService.Search(dateRange, eqpId, prodId, dataGroup);

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

        [HttpGet]
        public IActionResult Detail([FromQuery] string dateRange, string eqpId, string prodId, string dataGroup)
        {
            ViewBag.ProdOptions = _optionDomainService.GetLcmProdOptions();

            var _resilt = _spcReportDomainService.Detail(dateRange, eqpId, prodId, dataGroup);

            List<OptionEntity> _options = new List<OptionEntity>
            {
                new OptionEntity{Id = 0, Value = "BA_A_BSP011Lx_L" },
                new OptionEntity{Id = 1, Value = "BA_A_BSP011Ly_L" },
                new OptionEntity{Id = 2, Value = "DW_A_BSP011Ly_L" },
                new OptionEntity{Id = 3, Value = "BA_A_BSP016Rx_L" },
                new OptionEntity{Id = 4, Value = "BA_A_BSP016Ry_L" },
                new OptionEntity{Id = 5, Value = "DW_A_BSP016Ry_L" },
            };

            ViewBag.TestItemOptions = new SelectList(_options, "Value", "Value");

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
                SPCDetail = _resilt.DetailList.CopyAToB<SPCDataViewModel>()
            };

            return Json(_response);
        }

    }
}
