using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MOD4.Web.DomainService;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using MOD4.Web.Models;
using MOD4.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility.Helper;

namespace MOD4.Web.Controllers
{
    [Authorize]
    public class AccountManagementController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IOptionDomainService _optionDomainService;
        private readonly IAccountDomainService _accountDomainService;
        private readonly string _shaKey = string.Empty;

        public AccountManagementController(IHttpContextAccessor httpContextAccessor,
            IAccountDomainService accountDomainService,
            IOptionDomainService optionDomainService,
            IConfiguration connectionString,
            ILogger<HomeController> logger)
            : base(httpContextAccessor, accountDomainService)
        {
            _optionDomainService = optionDomainService;
            _accountDomainService = accountDomainService;
            _shaKey = connectionString.GetSection("SHAKey").Value;
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
                ViewBag.LevelOption = new SelectList(_optionDomainService.GetLevelOptionList().CopyAToB<OptionViewModel>(), "Id", "Value");
                ViewBag.DepartmentOption = new SelectList(_optionDomainService.GetDepartmentOptionList(0, 1).CopyAToB<OptionViewModel>(), "Id", "Value");

                var _result = _accountDomainService.GetAccountAndMenuInfo(accountSn);

                AccountEditViewModel _response = new AccountEditViewModel
                {
                    Sn = _result.sn,
                    Account = _result.Account,
                    Password = _result.Password,
                    Name = _result.Name,
                    JobId = _result.JobId,
                    ApiKey = _result.ApiKey,
                    MODId = _result.MODId,
                    DepartmentId = _result.DepartmentId,
                    SectionId = _result.SectionId,
                    LevelId = _result.Level_id,
                    Mail = _result.Mail,
                    MenuPermissionList = _result.MenuPermissionList.Select(menu =>
                    new MenuPermissionViewModel
                    {
                        IsMenuActive = menu.IsMenuActive,
                        MenuId = menu.MenuId,
                        Menu = menu.Menu,
                        MenuActionList = menu.ActionList.Select(s => new MenuActionViewModel
                        {
                            IsActionActive = s.IsActionActive,
                            ActionId = s.ActionId,
                            Action = s.Action
                        }).ToList()
                    }).ToList()
                };

                return View(_response);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = $"{ex.Message}\n{ex.StackTrace}" });
            }
        }

        [HttpPost]
        public IActionResult Edit([FromForm] AccountEditViewModel updateModel)
        {
            try
            {
                string _response = _accountDomainService.Update(new AccountCreateEntity
                {
                    sn = updateModel.Sn,
                    Account = updateModel.Account,
                    Password = updateModel.Password,
                    Name = updateModel.Name,
                    JobId = updateModel.JobId,
                    MODId = updateModel.MODId,
                    DepartmentId = updateModel.DepartmentId,
                    SectionId = updateModel.SectionId,
                    Level_id = updateModel.LevelId,
                    Mail = updateModel.Mail,
                    ApiKey = updateModel.ApiKey,
                    MenuPermissionList = updateModel.MenuPermissionList.Where(w => w.IsMenuActive)
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
                    DepartmentId = createModel.DepartmentId,
                    SectionId = createModel.SectionId,
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

        [HttpGet]
        public IActionResult SyncDLEmp()
        {
            try
            {
                string _response = _accountDomainService.SyncDLEmp(_shaKey);

                return Json(new ResponseViewModel<string>
                {
                    IsSuccess = string.IsNullOrEmpty(_response),
                    Msg = _response
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseViewModel<string>
                {
                    IsSuccess = false,
                    Msg = ex.Message
                });
            }
        }
    }
}
