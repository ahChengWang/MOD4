using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MOD4.Web.DomainService;
using MOD4.Web.Enum;
using MOD4.Web.Models;
using MOD4.Web.ViewModel;
using System;
using System.Linq;
using Utility.Helper;

namespace MOD4.Web.Controllers
{
    [Authorize]
    public class BulletinController : BaseController
    {
        private readonly ILogger<ExtensionController> _logger;
        private readonly IOptionDomainService _optionDomainService;

        public BulletinController(IHttpContextAccessor httpContextAccessor,
            IAccountDomainService accountDomainService,
            IOptionDomainService optionDomainService,
            ILogger<ExtensionController> logger)
            : base(httpContextAccessor, accountDomainService)
        {
            _optionDomainService = optionDomainService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var _userCurrentPagePermission = GetUserInfo().UserMenuPermissionList.FirstOrDefault(f => f.MenuSn == MenuEnum.MPS);
            ViewBag.UserPermission = new UserPermissionViewModel
            {
                AccountSn = _userCurrentPagePermission.AccountSn,
                MenuSn = _userCurrentPagePermission.MenuSn,
                AccountPermission = _userCurrentPagePermission.AccountPermission
            };

            ViewBag.SectionOption = _optionDomainService.GetAllSections().Select(s => new
            {
                Id = s.Id,
                Value = s.Value
            });

            return View();
        }

        [HttpPost]
        public IActionResult Create(BulletinCreateViewModel createVM)
        {
            try
            {
                return Json("");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        //[HttpGet]
        //public IActionResult MPSDownload()
        //{
        //    try
        //    {
        //        var _dwnlRes = _extensionDomainService.Download();

        //        if (_dwnlRes.Item1)
        //            return PhysicalFile(_dwnlRes.Item2, System.Net.Mime.MediaTypeNames.Application.Octet, _dwnlRes.Item3);
        //        else
        //            return RedirectToAction("Error", "Home", new ErrorViewModel { Message = "下載異常" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
        //    }
        //}
    }
}
