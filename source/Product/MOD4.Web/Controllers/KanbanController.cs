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
    public class KanbanController : BaseController
    {
        private readonly ILogger<ExtensionController> _logger;
        private readonly IOptionDomainService _optionDomainService;

        public KanbanController(IHttpContextAccessor httpContextAccessor,
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

            ViewBag.ApplyAreaOption = _optionDomainService.GetCertifiedAreaOptions().Select(s => new
            {
                Area = s.AreaId.GetDescription(),
                AreaId = s.AreaId,
                SubjectId = s.SubjectId,
                Subject = s.Subject
            });

            return View();
        }

        [HttpPost]
        public IActionResult MPSUpload([FromForm] ReportUploadViewModel uploadVM)
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
