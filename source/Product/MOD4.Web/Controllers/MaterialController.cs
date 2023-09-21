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
    public class MaterialController : BaseController
    {
        private readonly ILogger<ExtensionController> _logger;
        private readonly IMaterialDomainService _materialDomainService;

        public MaterialController(IMaterialDomainService materialDomainService,
            IHttpContextAccessor httpContextAccessor,
            IAccountDomainService accountDomainService,
            ILogger<ExtensionController> logger)
            : base(httpContextAccessor, accountDomainService)
        {
            _materialDomainService = materialDomainService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var _res = _materialDomainService.GetSAPDropDownList().OrderBy(o => o.Order).ThenBy(tb => tb.MaterialNo);

            ViewBag.AllOptions = _res.Select(s => new
            {
                s.Order,
                s.Prod,
                s.MaterialNo,
                s.SAPNode
            }).ToList();

            ViewBag.OrderNoOptions = _res.GroupBy(g => g.Order).Select(s => s.Key).ToList();
            ViewBag.SapNodeOptions = _res.GroupBy(g => g.SAPNode).Select(s => s.Key).OrderBy(ob => ob).ToList();

            return View();
        }

        [HttpGet]
        public IActionResult SAPSearch([FromQuery] string workOrder, string prodNo, string sapNode, string matlNo)
        {
            try
            {
                var _result = _materialDomainService.GetSAPWorkOredr(workOrder, prodNo, sapNode, matlNo);

                return Json(new ResponseViewModel<List<SAPWorkOrderViewModel>>
                {
                    IsSuccess = true,
                    Data = _result.Select(res => new SAPWorkOrderViewModel
                    {
                        Order = res.Order,
                        Prod = res.Prod,
                        MaterialNo = res.MaterialNo,
                        SAPNode = res.SAPNode,
                        ProdQty = res.ProdQty,
                        StorageQty = res.StorageQty,
                        StartDate = res.StartDate.ToString("yyyy/MM/dd"),
                        FinishDate = res.FinishDate.ToString("yyyy/MM/dd"),
                        Unit = res.Unit,
                        ExptQty = res.ExptQty,
                        DisburseQty = res.DisburseQty,
                        ReturnQty = res.ReturnQty,
                        ActStorageQty = res.ActStorageQty,
                        ScrapQty = res.ScrapQty,
                        DiffQty = res.DiffQty,
                        DiffRate = res.DiffRate,
                        OverDisburse = res.OverDisburse,
                        DiffDisburse = res.DiffDisburse,
                        WOPremiumOut = res.WOPremiumOut,
                        CantNegative = res.CantNegative,
                        MatlShortName = res.MatlShortName,
                        OPIwoStatus = res.OPIwoStatus,
                        WOType = res.WOType,
                        UseNode = res.UseNode,
                        WOComment = res.WOComment,
                        MESScrap = res.MESScrap,
                        ICScrap = res.ICScrap,
                    }).ToList()
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
        public IActionResult WOCloseDownload([FromQuery] string workOrder, string prodNo, string sapNode, string matlNo)
        {
            try
            {
                var _res = _materialDomainService.GetSAPwoCloseDownload(workOrder, prodNo, sapNode, matlNo);

                if (_res.Item1)
                    return File(System.IO.File.OpenRead(_res.Item2), "application/octet-stream", _res.Item3);
                //return PhysicalFile(_res.Item2, System.Net.Mime.MediaTypeNames.Application.Octet, _res.Item3);
                else
                    return RedirectToAction("Error", "Home", new ErrorViewModel { Message = _res.Item3 });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        #region === SAP upload & Material setting ===

        [HttpGet("[controller]/Setting")]
        public IActionResult Setting()
        {
            try
            {
                var _result = _materialDomainService.GetMaterialSetting(MatlCodeTypeEnum.Code5);
                
                var _userPagePermission = GetUserInfo().UserMenuPermissionList.FirstOrDefault(f => f.MenuSn == MenuEnum.MaterialUpload);
                ViewBag.UserPermission = new UserPermissionViewModel
                {
                    AccountSn = _userPagePermission.AccountSn,
                    MenuSn = _userPagePermission.MenuSn,
                    AccountPermission = _userPagePermission.AccountPermission
                };

                return View(_result.CopyAToB<MaterialSettingViewModel>());
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        /// <summary>
        /// 上傳 SAP 軟體下載的原始檔
        /// </summary>
        /// <param name="uFile"></param>
        /// <returns></returns>
        [HttpPost("[controller]/UploadSAP")]
        public IActionResult UploadSAP(IFormFile uFile)
        {
            try
            {
                var _uplRes = _materialDomainService.UploadAndCalculate(uFile, GetUserInfo());

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
                        Msg = "無選擇檔案"
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
        /// 下載 SAP 管報
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet("[controller]/SAPMngRpt/{fileName}")]
        public IActionResult SAPMngRpt(string fileName)
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

        /// <summary>
        /// 耗損設定查詢
        /// </summary>
        /// <param name="codeTypeId">5碼 or 13碼</param>
        /// <returns></returns>
        [HttpGet("[controller]/MatlSearch/{codeTypeId}")]
        public IActionResult MatlSearch(MatlCodeTypeEnum codeTypeId)
        {
            try
            {
                var _result = _materialDomainService.GetMaterialSetting(codeTypeId);

                return Json(new ResponseViewModel<List<MaterialSettingViewModel>>
                {
                    IsSuccess = true,
                    Data = _result.CopyAToB<MaterialSettingViewModel>()
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
        /// 更新 5碼、13碼料 耗損率
        /// </summary>
        /// <param name="updVM"></param>
        /// <param name="codeTypeId"></param>
        /// <returns></returns>
        [HttpPut("[controller]/Setting")]
        public IActionResult MatlSetting(List<MaterialSettingViewModel> updVM, MatlCodeTypeEnum codeTypeId)
        {
            try
            {
                var _result = _materialDomainService.UpdateMaterialSetting(updVM.CopyAToB<MaterialSettingEntity>(), codeTypeId, GetUserInfo());

                return Json(new ResponseViewModel<string>
                {
                    IsSuccess = _result == "",
                    Msg = _result == "" ? "更新成功" : _result
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
        /// 上傳 5碼、13碼料 耗損率檔
        /// </summary>
        /// <param name="settingFile"></param>
        /// <param name="codeTypeId"></param>
        /// <returns></returns>
        [HttpPost("[controller]/MatlSettingUpload")]
        public IActionResult MatlSettingUpload([FromForm] IFormFile settingFile, MatlCodeTypeEnum codeTypeId)
        {
            try
            {
                var _uplRes = _materialDomainService.UploadCodeRate(settingFile, codeTypeId, GetUserInfo());

                return Json(new ResponseViewModel<string>
                {
                    IsSuccess = _uplRes.Item1,
                    Msg = _uplRes.Item2
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
        /// 下載當前 5碼、13碼料 耗損設定
        /// </summary>
        /// <param name="codeTypeId"></param>
        /// <returns></returns>
        [HttpGet("[controller]/MatlSettingDownload/{codeTypeId}")]
        public IActionResult MatlSettingDownload(MatlCodeTypeEnum codeTypeId)
        {
            try
            {
                var _res = _materialDomainService.MatlSettingDownload(codeTypeId);

                if (_res.Item1)
                    return File(System.IO.File.OpenRead(_res.Item2), "application/octet-stream", _res.Item3);
                //return PhysicalFile(_res.Item2, System.Net.Mime.MediaTypeNames.Application.Octet, _res.Item3);
                else
                    return RedirectToAction("Error", "Home", new ErrorViewModel { Message = _res.Item3 });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        #endregion
    }
}
