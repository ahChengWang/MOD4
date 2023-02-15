using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class SPCReportController : BaseController
    {
        private readonly ILogger<SPCReportController> _logger;
        private readonly IOptionDomainService _optionDomainService;
        private readonly IExtensionDomainService _extensionDomainService;

        public SPCReportController(IExtensionDomainService extensionDomainService,
            IHttpContextAccessor httpContextAccessor,
            IAccountDomainService accountDomainService,
            IOptionDomainService optionDomainService,
            ILogger<SPCReportController> logger)
            : base(httpContextAccessor, accountDomainService)
        {
            _extensionDomainService = extensionDomainService;
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
            };

            ViewBag.TestItemOptions = new SelectList(_options, "Value", "Value");

            return View();
        }

        public IActionResult Search()
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
            };

            ViewBag.TestItemOptions = new SelectList(_options, "Value", "Value");

            return Json(new List<SPCDataViewModel> {
                new SPCDataViewModel{ 
                    MeasureDate = "2023-02-09",
                    MeasureTime = "00:01:00",
                    SHTId = "T31732U7NZ06",
                    ProductId = "GDD340IA0050S",
                    DataGroup = "BA_A_BSP011Lx_L",
                    DTX = 0.129,
                    DTRM = 0.019
                },
                new SPCDataViewModel{
                    MeasureDate = "2023-02-09",
                    MeasureTime = "00:02:00",
                    SHTId = "T31732U7NZ06",
                    ProductId = "GDD340IA0050S",
                    DataGroup = "BA_A_BSP011Ly_L",
                    DTX = 0.169,
                    DTRM = 0.059
                },
                new SPCDataViewModel{
                    MeasureDate = "2023-02-09",
                    MeasureTime = "00:03:00",
                    SHTId = "T31732U7NZ06",
                    ProductId = "GDD340IA0050S",
                    DataGroup = "DW_A_BSP011Ly_L",
                    DTX = 1.509,
                    DTRM = 1.399
                },
                new SPCDataViewModel{
                    MeasureDate = "2023-02-09",
                    MeasureTime = "00:04:00",
                    SHTId = "T31732U7NZ06",
                    ProductId = "GDD340IA0050S",
                    DataGroup = "BA_A_BSP016Rx_L",
                    DTX = 0.018,
                    DTRM = -0.028
                },
                new SPCDataViewModel{
                    MeasureDate = "2023-02-09",
                    MeasureTime = "00:05:00",
                    SHTId = "T31732U7NZ06",
                    ProductId = "GDD340IA0050S",
                    DataGroup = "BA_A_BSP016Ry_L",
                    DTX = 0.12,
                    DTRM = -0.12
                },
            });
        }

        [HttpPost]
        public IActionResult Upload([FromForm] ReportUploadViewModel uploadVM)
        {
            try
            {
                string _uplRes = _extensionDomainService.Upload(uploadVM.JobId, uploadVM.ApplyAreaId, uploadVM.ItemId, uploadVM.File, GetUserInfo());
                if (_uplRes == "")
                    return Json("");
                else
                    return Json(_uplRes);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult Download([FromQuery] string jobId, ApplyAreaEnum applyAreaId, int itemId)
        {
            try
            {
                var _uplRes = _extensionDomainService.Download(jobId, applyAreaId, itemId, GetUserInfo());

                if (_uplRes.Item2 == "")
                    return File(_uplRes.Item1, System.Net.Mime.MediaTypeNames.Application.Zip, $"{jobId}_{applyAreaId.GetDescription()}.zip");
                else
                    return RedirectToAction("Error", "Home", new ErrorViewModel { Message = _uplRes.Item2 });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }
    }
}
