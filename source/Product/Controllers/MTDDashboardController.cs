using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MOD4.Web.DomainService;
using MOD4.Web.Models;
using MOD4.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

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
                return View(new List<MTDDashboardViewModel>
                {
                    new MTDDashboardViewModel
                    {
                        Process = "BOND",
                        Plan = 1200,
                        Actual = 999,
                        DownTime = "237.2",
                        DownPercent = "16.5%",
                        UPPercent = "83.5%",
                        RUNPercent = "95.1%",
                        UPHPercent = "99.4%",
                        OEEPercent = "79.0%",
                        MTDDetail = new List<MTDDashboardDetailViewModel>
                        {
                            new MTDDashboardDetailViewModel
                            {
                                Date = "4/10",
                                Equipment = "AOLB2010",
                                BigProduct = "VCS",
                                PlanProduct = "GDD340IA0090S",
                                Output = "150",
                                DayPlan = "330",
                                RangPlan = "70",
                                RangDiff = "80",
                                MonthPlan = "4,999",
                                MTDPlan = "2,234",
                                MTDActual = "4,012",
                                MTDDiff = "1,778",
                                EqAbnormal = "DOWN SHOOTING 123",
                                RepaireTime = "66",
                                Status = "已排除"
                            },
                            new MTDDashboardDetailViewModel
                            {
                                Date = "4/10",
                                Equipment = "AOLB2010",
                                BigProduct = "PADI",
                                PlanProduct = "GDD313BZ0200S",
                                Output = "150",
                                DayPlan = "330",
                                RangPlan = "70",
                                RangDiff = "80",
                                MonthPlan = "4,999",
                                MTDPlan = "2,234",
                                MTDActual = "4,012",
                                MTDDiff = "1,778",
                                EqAbnormal = "DOWN SHOOTING 123",
                                RepaireTime = "66",
                                Status = "已排除"
                            },
                            new MTDDashboardDetailViewModel
                            {
                                Date = "4/10",
                                Equipment = "AOLB2010",
                                BigProduct = "PADI",
                                PlanProduct = "GDD313IA0010S",
                                Output = "150",
                                DayPlan = "330",
                                RangPlan = "70",
                                RangDiff = "80",
                                MonthPlan = "4,999",
                                MTDPlan = "2,234",
                                MTDActual = "4,012",
                                MTDDiff = "1,778",
                                EqAbnormal = "test",
                                RepaireTime = "99",
                                Status = "ING"
                            }
                        }
                    },
                    new MTDDashboardViewModel
                    {
                        Process = "FOG",
                        Plan = 1100,
                        Actual = 600,
                        DownTime = "211.2",
                        DownPercent = "16.5%",
                        UPPercent = "73.5%",
                        RUNPercent = "75.1%",
                        UPHPercent = "79.4%",
                        OEEPercent = "69.0%",
                        MTDDetail = new List<MTDDashboardDetailViewModel>
                        {
                            new MTDDashboardDetailViewModel
                            {
                                Date = "4/10",
                                Equipment = "AFOG2010",
                                BigProduct = "VCS",
                                PlanProduct = "GDD340IA0090S",
                                Output = "150",
                                DayPlan = "330",
                                RangPlan = "70",
                                RangDiff = "80",
                                MonthPlan = "4,999",
                                MTDPlan = "2,234",
                                MTDActual = "4,012",
                                MTDDiff = "1,778",
                                EqAbnormal = "",
                                RepaireTime = "",
                                Status = ""
                            }
                        }
                    },
                    new MTDDashboardViewModel
                    {
                        Process = "LAM",
                        Plan = 1900,
                        Actual = 1000,
                        DownTime = "100.2",
                        DownPercent = "56.5%",
                        UPPercent = "80.5%",
                        RUNPercent = "91.1%",
                        UPHPercent = "90.4%",
                        OEEPercent = "66.0%",
                        MTDDetail = new List<MTDDashboardDetailViewModel>
                        {
                            new MTDDashboardDetailViewModel
                            {
                                Date = "4/10",
                                Equipment = "CLAM2010",
                                BigProduct = "VCS",
                                PlanProduct = "GDD340IA0090S",
                                Output = "700",
                                DayPlan = "800",
                                RangPlan = "90",
                                RangDiff = "0",
                                MonthPlan = "4,000",
                                MTDPlan = "1,000",
                                MTDActual = "1,000",
                                MTDDiff = "1,008",
                                EqAbnormal = "",
                                RepaireTime = "",
                                Status = ""
                            },
                            new MTDDashboardDetailViewModel
                            {
                                Date = "4/10",
                                Equipment = "CLAM2010",
                                BigProduct = "VCS",
                                PlanProduct = "5306M3340160M",
                                Output = "200",
                                DayPlan = "400",
                                RangPlan = "111",
                                RangDiff = "99",
                                MonthPlan = "3,001",
                                MTDPlan = "1,001",
                                MTDActual = "1,100",
                                MTDDiff = "1,408",
                                EqAbnormal = "",
                                RepaireTime = "",
                                Status = ""
                            },
                            new MTDDashboardDetailViewModel
                            {
                                Date = "4/10",
                                Equipment = "CLAM2010",
                                BigProduct = "PADI",
                                PlanProduct = "GDD313IA0010S",
                                Output = "1000",
                                DayPlan = "600",
                                RangPlan = "300",
                                RangDiff = "200",
                                MonthPlan = "999",
                                MTDPlan = "1,999",
                                MTDActual = "1,111",
                                MTDDiff = "1,789",
                                EqAbnormal = "",
                                RepaireTime = "",
                                Status = ""
                            }
                        }
                    }
                });
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
        public IActionResult Search([FromQuery] string date, decimal time)
        {
            try
            {

                return Json("");
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
                var _response = _mtdDashboardDomainService.Search();

                List<ManufactureViewModel> _reponse = new List<ManufactureViewModel>();

                _response.ForEach(mtd =>
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
        public IActionResult ManufactureSearch([FromQuery] string dateRange)
        {
            try
            {
                var _result = _mtdDashboardDomainService.Search(dateRange);

                List<ManufactureViewModel> _response = new List<ManufactureViewModel>();

                _result.ForEach(mtd =>
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

                return PartialView("_PartialTable", _response);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpPost("[controller]/Manufacture/Upload")]
        public IActionResult ManufactureUpload([FromForm] IFormFile updFile)
        {
            try
            {
                var _result = _mtdDashboardDomainService.Upload(updFile, GetUserInfo());

                return Json(_result);
            }
            catch (Exception ex)
            {
                return Json($"錯誤：{ex.Message}");
            }
        }
        #endregion
    }
}
