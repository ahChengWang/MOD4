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
    public class AccountManagementController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IOptionDomainService _optionDomainService;

        public AccountManagementController(IHttpContextAccessor httpContextAccessor,
            IAccountDomainService accountDomainService,
            IOptionDomainService optionDomainService,
            ILogger<HomeController> logger)
            : base(httpContextAccessor, accountDomainService)
        {
            _optionDomainService = optionDomainService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                List<AccountViewModel> _response = new List<AccountViewModel>
                {
                    new AccountViewModel
                    {
                        Sn = 1,
                        Account = "flower.lin",
                        Name = "林意翔",
                        LevelId = "工程師",
                        JobId = "22000626",
                        Department = "支援二課"
                    },
                    new AccountViewModel
                    {
                        Sn = 2,
                        Account = "weiting.guo",
                        Name = "郭偉庭",
                        LevelId = "工程師",
                        JobId = "16013232",
                        Department = "支援一課"
                    },
                    new AccountViewModel
                    {
                        Sn = 3,
                        Account = "karen01.wang",
                        Name = "王麗雅",
                        LevelId = "工程師",
                        JobId = "10098491",
                        Department = "支援二課"
                    },
                    new AccountViewModel
                    {
                        Sn = 4,
                        Account = "morrise.chen",
                        Name = "陳驛印",
                        LevelId = "課長",
                        JobId = "10016031",
                        Department = "支援二課"
                    }
                };

                return View(_response);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Search([FromQuery] string startDate, string endDate, string category, string status)
        {
            try
            {
                UserEntity _userInfo = GetUserInfo();
                var _userCurrentPagePermission = _userInfo.UserMenuPermissionList.FirstOrDefault(f => f.MenuSn == MenuEnum.Demand);
                ViewBag.UserPermission = new UserPermissionViewModel
                {
                    AccountSn = _userCurrentPagePermission.AccountSn,
                    MenuSn = _userCurrentPagePermission.MenuSn,
                    AccountPermission = _userCurrentPagePermission.AccountPermission
                };

                return PartialView("_PartialTable", "");
            }
            catch (Exception ex)
            {
                return Json(new { Msg = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Edit([FromQuery] int sn, string orderId)
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

        [HttpGet]
        public IActionResult Detail([FromQuery] int sn)
        {
            return PartialView("_PartialDetail", "");
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.LevelOption = new SelectList(_optionDomainService.GetLevelOptionList().CopyAToB<OptionViewModel>(), "Id", "Value");
            ViewBag.DepartmentOption = new SelectList(_optionDomainService.GetDepartmentOptionList(0, 1).CopyAToB<OptionViewModel>(), "Id", "Value");
            ViewBag.MenuOption = new SelectList(_optionDomainService.GetMenuOptionList().CopyAToB<OptionViewModel>(), "Id", "Value");

            return View();
        }

        [HttpPost]
        public IActionResult Create(DemanCreateViewModel createModel)
        {
            try
            {
                return Json(new { IsSuccess = true, msg = "" });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, msg = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetDeptOption([FromQuery] int parentDeptId, int levelId)
        {
            return Json(_optionDomainService.GetDepartmentOptionList(parentDeptId, levelId)
                .Select(s => new OptionViewModel
                {
                    Id = s.Id,
                    RelatedId = s.SubId,
                    Value = s.Value
                }));
        }

        private (bool, string) Edit(DemanEditViewModel updModel, DemandStatusEnum newStatusId)
        {
            try
            {
                return (true, "");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
