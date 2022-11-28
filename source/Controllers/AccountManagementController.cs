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
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MOD4.Web.Controllers
{
    [Authorize]
    public class AccountManagementController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IOptionDomainService _optionDomainService;
        private readonly IAccountDomainService _accountDomainService;

        public AccountManagementController(IHttpContextAccessor httpContextAccessor,
            IAccountDomainService accountDomainService,
            IOptionDomainService optionDomainService,
            ILogger<HomeController> logger)
            : base(httpContextAccessor, accountDomainService)
        {
            _optionDomainService = optionDomainService;
            _accountDomainService = accountDomainService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                List<AccountViewModel> _response = _accountDomainService.GetAccountDepartmentList().Select(account =>
                new AccountViewModel
                {
                    Sn = account.sn,
                    Account = account.Account,
                    Name = account.Name,
                    LevelId = account.Level_id.GetDescription(),
                    JobId = account.JobId,
                    Department = account.DepartmentName
                }).ToList();

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
        public IActionResult Edit([FromQuery] int accountSn)
        {
            try
            {
                _accountDomainService.GetAccountAndMenuInfo(accountSn);

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

            List<MenuPermissionViewModel> _menuList = _optionDomainService.GetCreatePermissionList();

            var _response = new AccountEditViewModel
            {
                MenuPermissionList = _menuList
            };

            return View(_response);
        }

        [HttpPost]
        public IActionResult Create(AccountEditViewModel createModel)
        {
            try
            {
                string _response = _accountDomainService.Create(new AccountCreateEntity
                {
                    Account = createModel.Account,
                    Password = createModel.Password,
                    Name = createModel.Name,
                    JobId = createModel.JobId,
                    MODId = createModel.MODId,
                    SectionId = createModel.SectionId,
                    DepartmentId = createModel.DepartmentId,
                    Level_id = createModel.LevelId,
                    Mail = createModel.Mail,
                    ApiKey = createModel.ApiKey,
                    MenuPermissionList = createModel.MenuPermissionList.Where(w => w.IsMenuActive)
                        .Select(s => new AccountMenuInfoEntity
                        {
                            MenuSn = s.MenuId,
                            AccountPermission = s.MenuActionList.Where(action => action.IsActionActive).Sum(sum => (int)sum.ActionId)
                        }).ToList()
                });

                if (_response == "")
                    return Json(new { IsSuccess = true, msg = "" });
                else
                    return Json(new { IsSuccess = false, msg = _response });
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
