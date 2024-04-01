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
    public class ExtensionController : BaseController
    {
        private readonly ILogger<ExtensionController> _logger;
        private readonly IOptionDomainService _optionDomainService;
        private readonly IExtensionDomainService _extensionDomainService;

        public ExtensionController(IExtensionDomainService extensionDomainService,
            IHttpContextAccessor httpContextAccessor,
            IAccountDomainService accountDomainService,
            IOptionDomainService optionDomainService,
            ILogger<ExtensionController> logger)
            : base(httpContextAccessor, accountDomainService)
        {
            _extensionDomainService = extensionDomainService;
            _optionDomainService = optionDomainService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                UserEntity _userInfo = GetUserInfo();
                _userInfo.UserMenuPermissionList = _userInfo.UserMenuPermissionList.Where(w => w.MenuSn == MenuEnum.Extension).ToList();
                ViewBag.UserInfo = _userInfo;
                ViewBag.DefectCatgOption = _optionDomainService.GetRWDefectCode();

                var _res = _extensionDomainService.GetLightingHisList();

                List<LightingLogViewModel> _response = _res.Select(main => new LightingLogViewModel
                {
                    LogDate = main.LogDate,
                    ProcessList = main.ProcessList.Select(detail => new LightingLogSubViewModel
                    {
                        Category = detail.Category,
                        CategoryId = detail.CategoryId,
                        ProcessCnt = detail.ProcessCnt
                    }).ToList()
                }).ToList();

                return View(_response);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpGet("[controller]/MonthLog/{yearMonthStr}")]
        public IActionResult MonthLog(string yearMonthStr)
        {
            try
            {
                var _res = _extensionDomainService.GetLightingHisList(yearMonth: yearMonthStr);

                List<LightingLogViewModel> _response = _res.Select(main => new LightingLogViewModel
                {
                    LogDate = main.LogDate,
                    ProcessList = main.ProcessList.Select(detail => new LightingLogSubViewModel
                    {
                        Category = detail.Category,
                        CategoryId = detail.CategoryId,
                        ProcessCnt = detail.ProcessCnt
                    }).ToList()
                }).ToList();

                return Json(new ResponseViewModel<List<LightingLogViewModel>>
                {
                    Data = _response
                });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult LightingPanelLog(string panelId)
        {
            try
            {
                var _res = _extensionDomainService.GetLightingLogById(panelId);

                List<LightingDayLogDetailViewModel> _response = _res.Select(main => new LightingDayLogDetailViewModel
                {
                    PanelDate = main.PanelDate,
                    Category = main.CategoryId.GetDescription(),
                }).ToList();

                return Json(new ResponseViewModel<List<LightingDayLogDetailViewModel>>
                {
                    Data = _response
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
        public IActionResult DayLightingLog(string panelDate, LightingCategoryEnum categoryId)
        {
            try
            {
                var _res = _extensionDomainService.GetLightingDayLogList(panelDate, categoryId);

                LightingDayLogViewModel _response = _res.GroupBy(g => new { g.PanelDate.Date, g.CategoryId })
                    .Select(main => new LightingDayLogViewModel
                    {
                        LogDate = main.Key.Date,
                        LightingCategoryId = main.Key.CategoryId,
                        Detail = main.Select(detail => new LightingDayLogDetailViewModel
                        {
                            PanelSn = detail.PanelSn,
                            CategoryId = detail.CategoryId,
                            Category = detail.CategoryId.GetDescription(),
                            StatusId = detail.StatusId,
                            DefectCatgId = detail.DefectCatgId,
                            DefectCode = detail.DefectCode,
                            PanelDate = detail.PanelDate,
                            PanelId = detail.PanelId,
                            CreateUser = detail.CreateUser,
                            UpdateUser = detail.UpdateUser
                        }).ToList()
                    }).First();

                return Json(new ResponseViewModel<LightingDayLogViewModel>
                {
                    Data = _response
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
        public IActionResult DayLightingLog(LightingDayLogViewModel insModel)
        {
            try
            {
                var _res = _extensionDomainService.CreateLightingLog(insModel.Detail.Select(s =>
                new LightingLogEntity
                {
                    PanelId = s.PanelId,
                    PanelDate = s.PanelDate,
                    CategoryId = s.CategoryId,
                    StatusId = s.StatusId,
                    DefectCatgId = s.DefectCatgId,
                    DefectCode = s.DefectCode,
                }).ToList(),
                GetUserInfo());

                return Json(new ResponseViewModel<string>
                {
                    IsSuccess = string.IsNullOrEmpty(_res),
                    Msg = _res.ToString()
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

        [HttpPut]
        public IActionResult DayLightingLogUpdate(LightingDayLogViewModel updModel)
        {
            try
            {
                var _res = _extensionDomainService.UpdateLightingLog(updModel.Detail.Select(s =>
                new LightingLogEntity
                {
                    PanelSn = s.PanelSn,
                    PanelId = s.PanelId,
                    PanelDate = s.PanelDate,
                    CategoryId = s.CategoryId,
                    StatusId = s.StatusId,
                    DefectCatgId = s.DefectCatgId,
                    DefectCode = s.DefectCode,
                }).ToList(),
                GetUserInfo());

                return Json(new ResponseViewModel<string>
                {
                    IsSuccess = string.IsNullOrEmpty(_res),
                    Msg = _res.ToString()
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

        /// <summary>
        /// 下載選取日期檔案
        /// </summary>
        /// <param name="codeTypeId"></param>
        /// <returns></returns>
        [HttpGet("[controller]/LightingLogDownload/{logDate}")]
        public IActionResult LightingLogDownload(DateTime logDate)
        {
            try
            {
                var _res = _extensionDomainService.DownloadLog(logDate);

                if (_res.Item1)
                    return File(System.IO.File.OpenRead(_res.Item2), "application/octet-stream", _res.Item3);
                else
                    return RedirectToAction("Error", "Home", new ErrorViewModel { Message = _res.Item3 });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Upload([FromForm] ReportUploadViewModel uploadVM)
        {
            try
            {
                //string _uplRes = _extensionDomainService.Upload(uploadVM.JobId, uploadVM.ApplyAreaId, uploadVM.ItemId, uploadVM.File, GetUserInfo());
                string _uplRes = _extensionDomainService.MPSUpload(uploadVM.File, GetUserInfo());

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


        #region === MPS ===

        [HttpGet("[controller]/MPS")]
        public IActionResult MPS()
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

        [HttpPost("[controller]/MPSUpload")]
        public IActionResult MPSUpload([FromForm] ReportUploadViewModel uploadVM)
        {
            try
            {
                string _uplRes = _extensionDomainService.MPSUpload(uploadVM.File, GetUserInfo());

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

        [HttpGet("[controller]/MPSDownload")]
        public IActionResult MPSDownload()
        {
            try
            {
                var _dwnlRes = _extensionDomainService.Download();

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
        #endregion
    }
}
