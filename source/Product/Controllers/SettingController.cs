using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MOD4.Web.Models;
using MOD4.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MOD4.Web.Controllers
{
    [Authorize]
    public class SettingController : Controller
    {
        private readonly ILogger<SettingController> _logger;

        public SettingController(ILogger<SettingController> logger)
        {
            _logger = logger;
        }

        public IActionResult LineEquipment()
        {
            ViewData["Line"] = new List<string> { "ACOG2010", "AOLB2010" };

            return View(new LineViewModel
            {
                LineDetailList = new List<LineDetailModel> {
                    new LineDetailModel {
                        sn = 1109,
                        Operation = "1050",
                        Station = "ACOG",
                        ToolId = "ACOG2010",
                        Line = "ACOG2010",
                        TargetLine = "",
                        Area = "BONDING",
                        EnableImport = "N",
                        EnableDashboard = "N"
                    },
                    new LineDetailModel {
                        sn = 1110,
                        Operation = "1100",
                        Station = "AOLB",
                        ToolId = "AOLB2010",
                        Line = "AOLB2010",
                        TargetLine = "",
                        Area = "BONDING",
                        EnableImport = "N",
                        EnableDashboard = "N"
                    }
                }
            });
        }

        [HttpGet]
        public IActionResult Edit(int sn)
        {
            return View();
        }

        [HttpPost]
        public IActionResult Edit(LineViewModel updLineModel)
        {
            return View();
        }


        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("_PartialAddModal", new LineDetailModel { });
        }
    }
}
