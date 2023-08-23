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
    public class MaterialController : BaseController
    {
        private readonly ILogger<ExtensionController> _logger;
        private readonly IMaterialDomainService _materialDomainService;
        private readonly IExtensionDomainService _extensionDomainService;

        public MaterialController(IMaterialDomainService materialDomainService,
            IHttpContextAccessor httpContextAccessor,
            IAccountDomainService accountDomainService,
            IOptionDomainService optionDomainService,
            ILogger<ExtensionController> logger)
            : base(httpContextAccessor, accountDomainService)
        {
            _materialDomainService = materialDomainService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        //public IActionResult Upload([FromForm] ReportUploadViewModel uploadVM)
        //{
        //    try
        //    {
        //        //string _uplRes = _extensionDomainService.Upload(uploadVM.JobId, uploadVM.ApplyAreaId, uploadVM.ItemId, uploadVM.File, GetUserInfo());
        //        string _uplRes = _extensionDomainService.MPSUpload(uploadVM.File, GetUserInfo());

        //        if (_uplRes == "")
        //            return Json("");
        //        else
        //            return Json(_uplRes);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(ex.Message);
        //    }
        //}

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


        #region === SAP upload ===

        [HttpGet("[controller]/MatlUpload")]
        public IActionResult MatlUpload()
        {
            return View();
        }

        [HttpPost("[controller]/MatlUpload")]
        public IActionResult MaterialUpload(IFormFile uFile)
        {
            try
            {
                var _uplRes = _materialDomainService.Upload(uFile, GetUserInfo());

                if (_uplRes.Item1)
                    return Json(new ResponseViewModel<string>
                    {
                        IsSuccess = true,
                        Data = _uplRes.Item3,
                        Msg = "上傳成功"
                    });
                else
                    return Json(new ResponseViewModel<string>
                    {
                        IsSuccess = false,
                        Msg = "SAP檔上傳處理異常"
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

        [HttpGet("[controller]/MatlDownload/{fileName}")]
        public IActionResult MPSDownload(string fileName)
        {
            try
            {
                if (!string.IsNullOrEmpty(fileName))
                    return File(System.IO.File.OpenRead($"..\\tempFileProcess\\{fileName}"), "application/octet-stream", fileName);
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
