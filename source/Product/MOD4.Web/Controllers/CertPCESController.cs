﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MOD4.Web.DomainService;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Models;
using MOD4.Web.ViewModel;
using System;
using System.Collections.Generic;
using Utility.Helper;

namespace MOD4.Web.Controllers
{
    [Authorize]
    public class CertPCESController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPCESCertificationDomainService _certificationPCESDomainService;
        private readonly IOptionDomainService _optionDomainService;

        public CertPCESController(IPCESCertificationDomainService certificationPCESDomainService,
            IHttpContextAccessor httpContextAccessor,
            IOptionDomainService optionDomainService,
            IAccountDomainService accountDomainService,
            ILogger<HomeController> logger)
            : base(httpContextAccessor, accountDomainService)
        {
            _certificationPCESDomainService = certificationPCESDomainService;
            _optionDomainService = optionDomainService;
            _logger = logger;
        }

        public IActionResult Index()
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

        public IActionResult Search([FromQuery] string opr, string station, string prod, string status, string jobId)
        {
            try
            {
                var _pcesCertList = _certificationPCESDomainService.GetRecordPCES(opr, station, prod, status, jobId);

                return Json(new ResponseViewModel<List<PCESCertRecordViewModel>>
                {
                    IsSuccess = true,
                    Data = _pcesCertList.CopyAToB<PCESCertRecordViewModel>()
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseViewModel<string>
                {
                    IsSuccess = true,
                    Msg = ex.Message
                });
            }
        }

        [HttpGet("[controller]/Setting")]
        public IActionResult Setting()
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

        [HttpGet("[controller]/Setting/Search")]
        public IActionResult SettingSearch([FromQuery] string opr, string station, string prod, string status, string jobId)
        {
            try
            {
                var _pcesCertList = _certificationPCESDomainService.GetRecordPCES(opr, station, prod, null, jobId, status: status);

                return Json(new ResponseViewModel<List<PCESCertRecordViewModel>>
                {
                    IsSuccess = true,
                    Data = _pcesCertList.CopyAToB<PCESCertRecordViewModel>()
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseViewModel<string>
                {
                    IsSuccess = true,
                    Msg = ex.Message
                });
            }
        }

        [HttpPost("[controller]/Setting")]
        public IActionResult Setting(PCESCertRecordViewModel updateVM)
        {
            try
            {
                var _updRes = _certificationPCESDomainService.UpdateCertSkill(updateVM.CopyAToB<PCESCertRecordEntity>(), GetUserInfo());

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

        [HttpPut("[controller]/UpdatePCES")]
        public IActionResult UpdatePCES()
        {
            try
            {
                var _updRes = _certificationPCESDomainService.UpdatePCESRecord();

                return Json(new ResponseViewModel<string>
                {
                    IsSuccess = _updRes.Item1,
                    Msg = _updRes.Item2
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
