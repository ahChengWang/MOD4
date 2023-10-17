using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class BulletinController : BaseController
    {
        private readonly ILogger<ExtensionController> _logger;
        private readonly IOptionDomainService _optionDomainService;
        private readonly IBulletinDomainService _bulletinDomainService;

        public BulletinController(IHttpContextAccessor httpContextAccessor,
            IAccountDomainService accountDomainService,
            IOptionDomainService optionDomainService,
            IBulletinDomainService bulletinDomainService,
            ILogger<ExtensionController> logger)
            : base(httpContextAccessor, accountDomainService)
        {
            _bulletinDomainService = bulletinDomainService;
            _optionDomainService = optionDomainService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                var _userCurrentPagePermission = GetUserInfo().UserMenuPermissionList.FirstOrDefault(f => f.MenuSn == MenuEnum.Bulletin);
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

                var _resp = _bulletinDomainService.GetBulletinList(GetUserInfo());

                return View(_resp.CopyAToB<BulletinViewModel>());
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Search(string dateRange, string readStatus)
        {
            try
            {
                var _userCurrentPagePermission = GetUserInfo().UserMenuPermissionList.FirstOrDefault(f => f.MenuSn == MenuEnum.Bulletin);
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

                var _resp = _bulletinDomainService.GetBulletinByConditions(GetUserInfo(), dateRange, readStatus);

                return Json(new ResponseViewModel<List<BulletinViewModel>>
                {
                    IsSuccess = true,
                    Data = _resp.CopyAToB<BulletinViewModel>()
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

        [HttpPost]
        public IActionResult Create(BulletinCreateViewModel createVM)
        {
            try
            {
                var _res = _bulletinDomainService.Create(createVM.CopyAToB<BulletinCreateEntity>(), GetUserInfo());

                if (string.IsNullOrEmpty(_res))
                    return Json(new ResponseViewModel<string>
                    {
                        Msg = ""
                    });
                else
                    return Json(new ResponseViewModel<string>
                    {
                        IsSuccess = false,
                        Msg = _res
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

        [HttpGet]
        public IActionResult Detail([FromQuery] int bulletinSn)
        {
            try
            {
                var _resp = _bulletinDomainService.GetBulletinDetail(bulletinSn);

                //return Json(new ResponseViewModel<BulletinViewModel>
                //{
                //    IsSuccess = false,
                //    Msg = "error 查無此公告"
                //});

                return Json(new ResponseViewModel<BulletinViewModel>
                {
                    IsSuccess = _resp != null,
                    Data = _resp.CopyAToB<BulletinViewModel>(),
                    Msg = _resp == null ? "error 查無此公告" : ""
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseViewModel<BulletinViewModel>
                {
                    IsSuccess = false,
                    Msg = ex.Message
                });
            }
        }

        [HttpPut]
        public IActionResult UpdateDetail([FromQuery] int bulletinSn)
        {
            try
            {
                var _updRes = _bulletinDomainService.UpdateDetail(bulletinSn, GetUserInfo());

                return Json(new ResponseViewModel<string>
                {
                    IsSuccess = string.IsNullOrEmpty(_updRes),
                    Msg = _updRes
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

        [HttpGet]
        public IActionResult DownloadFile([FromQuery] int bulletinSn)
        {
            try
            {
                var _dwnlRes = _bulletinDomainService.Download(bulletinSn, GetUserInfo());

                if (_dwnlRes.Item1)
                    return PhysicalFile(_dwnlRes.Item2, System.Net.Mime.MediaTypeNames.Application.Octet, _dwnlRes.Item3);
                else
                    return RedirectToAction("Error", "Home", new ErrorViewModel { Message = "下載異常" });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult DownloadReadInfoFile([FromQuery] int bulletinSn)
        {
            try
            {
                var _dwnlRes = _bulletinDomainService.DownloadReadInfoFile(bulletinSn);

                if (_dwnlRes.Item1)
                    return File(System.IO.File.OpenRead(_dwnlRes.Item2), "application/octet-stream", _dwnlRes.Item3);
                else
                    return RedirectToAction("Error", "Home", new ErrorViewModel { Message = "下載異常" });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }
    }
}
