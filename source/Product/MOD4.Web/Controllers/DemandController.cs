using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MOD4.Web.DomainService;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using Utility.Helper;
using MOD4.Web.Models;
using MOD4.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using CSRedis;

namespace MOD4.Web.Controllers
{
    [Authorize]
    public class DemandController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDemandDomainService _demandDomainService;
        private readonly IOptionDomainService _optionDomainService;

        public DemandController(IDemandDomainService demandDomainService,
            IHttpContextAccessor httpContextAccessor,
            IOptionDomainService optionDomainService,
            IAccountDomainService accountDomainService,
            ILogger<HomeController> logger)
            : base(httpContextAccessor, accountDomainService)
        {
            _demandDomainService = demandDomainService;
            _optionDomainService = optionDomainService;
            _logger = logger;
        }

        #region 需求單

        public IActionResult Index()
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

                var _demands = _demandDomainService.GetDemands(_userInfo, dateStart: DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd"), dateEnd: DateTime.Now.ToString("yyyy-MM-dd"));

                List<DemanMainViewModel> _response = _demands.Select(s => new DemanMainViewModel
                {
                    OrderSn = s.OrderSn,
                    OrderId = s.OrderNo,
                    DemandCategory = s.CategoryId.GetDescription(),
                    DemandCategoryId = s.CategoryId,
                    DemandStatus = s.StatusId.GetDescription(),
                    DemandStatusId = s.StatusId,
                    Subject = s.Subject,
                    Applicant = s.Applicant,
                    JobNo = s.JobNo,
                    CreateDate = s.CreateTime.ToString("yyyy-MM-dd"),
                    UserEditable = s.UserEditable,
                    //RoleId = _userInfo.RoleId
                }).ToList();

                return View(_response);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Search([FromQuery] string startDate, string endDate, string category, string status, string kw)
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

                var _demands = _demandDomainService.GetDemands(_userInfo, dateStart: startDate, dateEnd: endDate, categoryId: category, statusId: status, kw: kw);

                List<DemanMainViewModel> _response = _demands.Select(s => new DemanMainViewModel
                {
                    OrderSn = s.OrderSn,
                    OrderId = s.OrderNo,
                    DemandCategory = s.CategoryId.GetDescription(),
                    DemandCategoryId = s.CategoryId,
                    DemandStatus = s.StatusId.GetDescription(),
                    DemandStatusId = s.StatusId,
                    Subject = s.Subject,
                    Applicant = s.Applicant,
                    JobNo = s.JobNo,
                    CreateDate = s.CreateTime.ToString("yyyy-MM-dd"),
                    UserEditable = s.UserEditable,
                    //RoleId = _userInfo.RoleId
                }).ToList();

                return PartialView("_PartialTable", _response);
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
                ViewBag.CategoryOptions = _optionDomainService.GetDemandCategoryOptionList();

                UserEntity _userInfo = GetUserInfo();
                var _userCurrentPagePermission = _userInfo.UserMenuPermissionList.FirstOrDefault(f => f.MenuSn == MenuEnum.Demand);
                ViewBag.UserPermission = new UserPermissionViewModel
                {
                    AccountSn = _userCurrentPagePermission.AccountSn,
                    MenuSn = _userCurrentPagePermission.MenuSn,
                    AccountPermission = _userCurrentPagePermission.AccountPermission
                };

                var _res = _demandDomainService.GetDemandDetail(sn, _userInfo, orderId);

                DemanEditViewModel _response = new DemanEditViewModel
                {
                    OrderSn = sn,
                    OrderId = _res.OrderNo,
                    CreateDate = _res.CreateTimeStr,
                    DemandCategoryId = _res.CategoryId,
                    DemandStatus = _res.Status,
                    DemandStatusId = _res.StatusId,
                    Subject = _res.Subject,
                    Content = _res.Content,
                    Applicant = _res.Applicant,
                    JobNo = _res.JobNo,
                    UploadFile1 = _res.UploadFile1,
                    UploadFile2 = _res.UploadFile2,
                    UploadFile3 = _res.UploadFile3,
                    CompleteUploadFile1 = _res.CompleteUploadFile1,
                    CompleteUploadFile2 = _res.CompleteUploadFile2,
                    CompleteUploadFile3 = _res.CompleteUploadFile3,
                    RejectReason = _res.RejectReason,
                    Remark = _res.Remark
                };

                return View(_response);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }

        }

        [HttpPost]
        public IActionResult Edit([FromForm] DemanEditViewModel updModel)
        {
            try
            {
                var _res = _demandDomainService.UpdateDemand(new DemandEntity
                {
                    OrderSn = updModel.OrderSn,
                    OrderNo = updModel.OrderId,
                    StatusId = updModel.DemandStatusId,
                    CategoryId = updModel.DemandCategoryId,
                    Subject = updModel.Subject,
                    Content = updModel.Content,
                    Applicant = updModel.Applicant,
                    JobNo = updModel.JobNo,
                    UploadFileList = updModel.UploadFile,
                    RejectReason = updModel.RejectReason,
                    Remark = updModel.Remark,
                    IsNewProd = updModel.IsNewProd,
                    Floor = updModel.Floor,
                    ProdNo = updModel.ProdNo,
                    ProdDesc = updModel.ProdDesc
                },
                GetUserInfo());

                return Json(new { IsSuccess = _res.Item1, msg = _res.Item2 });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, msg = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Detail([FromQuery] int sn)
        {
            ViewBag.CategoryOptions = _optionDomainService.GetDemandCategoryOptionList();

            var _res = _demandDomainService.GetDemandDetail(sn, null);

            DemanEditViewModel _response = new DemanEditViewModel
            {
                OrderSn = sn,
                OrderId = _res.OrderNo,
                CreateDate = _res.CreateTimeStr,
                DemandCategoryId = _res.CategoryId,
                DemandStatus = _res.Status,
                DemandStatusId = _res.StatusId,
                Subject = _res.Subject,
                Content = _res.Content,
                Applicant = _res.Applicant,
                JobNo = _res.JobNo,
                UploadFile1 = _res.UploadFile1,
                UploadFile2 = _res.UploadFile2,
                UploadFile3 = _res.UploadFile3,
                RejectReason = _res.RejectReason,
                Remark = _res.Remark,
                CompleteUploadFile1 = _res.CompleteUploadFile1,
                CompleteUploadFile2 = _res.CompleteUploadFile2,
                CompleteUploadFile3 = _res.CompleteUploadFile3
            };

            return PartialView("_PartialDetail", _response);
        }

        [HttpGet]
        public IActionResult Download([FromQuery] int orderSn, int typeId, int fileNo)
        {
            try
            {
                var _res = _demandDomainService.GetDownFileStr(orderSn, typeId, fileNo);

                if (_res.Item1 == null)
                {
                    return RedirectToAction("Error", "Home", new ErrorViewModel { Message = _res.Item2 });
                }

                return File(_res.Item1, "application/octet-stream", _res.Item2);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            UserEntity _userEntity = GetUserInfo();

            ViewBag.CategoryOptions = _optionDomainService.GetDemandCategoryOptionList();
            ViewBag.AccountName = _userEntity.Name;
            ViewBag.JobId = _userEntity.JobId;
            return View();
        }

        [HttpPost]
        public IActionResult Create(DemanCreateViewModel createModel)
        {
            try
            {
                var _res = _demandDomainService.InsertDemand(new DemandEntity
                {
                    CategoryId = createModel.DemandCategoryId,
                    Subject = createModel.Subject,
                    Content = createModel.Content,
                    Applicant = createModel.Applicant,
                    JobNo = createModel.JobNo,
                    UploadFileList = createModel.UploadFile
                }, GetUserInfo());

                return Json(new { IsSuccess = _res.Item1, msg = _res.Item2 });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, msg = ex.Message });
            }
        }
        #endregion

        #region MES 權限申請

        [HttpGet("[controller]/MES")]
        public IActionResult MESPermission()
        {
            try
            {
                UserEntity _userInfo = GetUserInfo();
                var _userCurrentPagePermission = _userInfo.UserMenuPermissionList.FirstOrDefault(f => f.MenuSn == MenuEnum.MESPermission);
                ViewBag.UserPermission = new UserPermissionViewModel
                {
                    AccountSn = _userCurrentPagePermission.AccountSn,
                    MenuSn = _userCurrentPagePermission.MenuSn,
                    AccountPermission = _userCurrentPagePermission.AccountPermission
                };
                var _response = _demandDomainService.GetMESApplicantList(_userInfo)
                        .Select(mes => new MESPermissionMainViewModel
                        {
                            OrderSn = mes.OrderSn,
                            OrderNo = mes.OrderNo,
                            Status = mes.Status,
                            StatusId = (int)mes.StatusId,
                            Applicant = mes.Applicant,
                            MESOrderType = mes.MESOrderType,
                            JobId = mes.JobId,
                            ApplicantList = mes.ApplicantName,
                            AuditPerson = mes.AuditName,
                            CreateDate = mes.CreateTimeStr,
                            Reason = mes.ApplicantReason,
                            Url = mes.Url
                        }).ToList();

                return View(_response);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpGet("[controller]/MES/Search")]
        public IActionResult MESPermSearch([FromQuery] string dateStart, string dateEnd, string statusList, string keyWord, string orderType)
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
                var _response = _demandDomainService.GetMESApplicantList(_userInfo, dateStart: dateStart, dateEnd: dateEnd, statusId: statusList, kw: keyWord, orderType: orderType)
                        .Select(mes => new MESPermissionMainViewModel
                        {
                            OrderSn = mes.OrderSn,
                            OrderNo = mes.OrderNo,
                            Status = mes.Status,
                            StatusId = (int)mes.StatusId,
                            Applicant = mes.Applicant,
                            MESOrderType = mes.MESOrderType,
                            JobId = mes.JobId,
                            ApplicantList = mes.ApplicantName,
                            AuditPerson = mes.AuditName,
                            CreateDate = mes.CreateTimeStr,
                            Reason = mes.ApplicantReason,
                            Url = mes.Url
                        }).ToList();

                return PartialView("_PartialPermission", _response);
            }
            catch (Exception ex)
            {
                return Json(new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpGet("[controller]/MES/Create")]
        public IActionResult MESCreate()
        {
            try
            {
                var _mesPermission = _optionDomainService.GetMESPermission().CopyAToB<OptionViewModel>();

                UserEntity _userEntity = GetUserInfo();

                ViewBag.AccountName = _userEntity.Name;
                ViewBag.JobId = _userEntity.JobId;
                ViewBag.MESOrderType = new SelectList(_optionDomainService.GetMESType(), "Id", "Value");

                List<MESPermissionModel> _mesPermissionList = _mesPermission.Select(s => new MESPermissionModel
                {
                    MESPermissionId = s.Id,
                    MESPermission = s.Value,
                    IsEnable = false
                }).ToList();

                return View(new MESPermissionCreateViewModel
                {
                    PermissionList = _mesPermissionList,
                    ApplicantList = new List<MESApplicantModel>()
                });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpPost("[controller]/MES/Create")]
        public IActionResult MESCreate([FromForm] MESPermissionCreateViewModel createModel)
        {
            try
            {
                var _result = _demandDomainService.CreateMESApplicant(new MESPermissionEntity
                {
                    Department = createModel.Department,
                    SubUnit = createModel.SubUnit,
                    Applicant = createModel.Applicant,
                    JobId = createModel.JobId,
                    MESOrderTypeId = createModel.MESOrderTypeId,
                    Phone = createModel.Phone,
                    ApplicantReason = createModel.ApplicantReason,
                    PermissionInfo = createModel.PermissionList.Select(per => new MESPermissionDetailEntity
                    {
                        MESPermissionId = per.MESPermissionId,
                        IsEnable = per.IsEnable
                    }).ToList(),
                    OtherPermission = createModel.OtherPermission,
                    SamePermName = createModel.SameEmpName,
                    SamePermJobId = createModel.SameEmpJobId,
                    Applicants = createModel.ApplicantList.Select(emp => new MESApplicantEntity
                    {
                        ApplicantName = emp.ApplicantName,
                        ApplicantJobId = emp.ApplicantJobId
                    }).ToList(),
                    UploadFile = createModel.UploadFile
                },
                GetUserInfo());

                return Json(new { IsSuccess = _result.Item1, Msg = _result.Item2 });

            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpGet("[controller]/MES/Detail/{orderSn}")]
        public IActionResult MESDetail(int orderSn)
        {
            try
            {
                MESPermissionEntity _result = _demandDomainService.GetDetail(orderSn);

                MESPermissionDetailViewModel _response = new MESPermissionDetailViewModel
                {
                    OrderSn = _result.OrderSn,
                    OrderNo = _result.OrderNo,
                    StatusId = _result.StatusId,
                    Status = _result.Status,
                    Department = _result.Department,
                    SubUnit = _result.SubUnit,
                    MESOrderType = _result.MESOrderType,
                    Applicant = _result.Applicant,
                    JobId = _result.JobId,
                    Phone = _result.Phone,
                    ApplicantReason = _result.ApplicantReason,
                    PermissionList = _result.PermissionInfo.CopyAToB<MESPermissionModel>(),
                    ApplicantList = _result.Applicants.CopyAToB<MESApplicantModel>(),
                    OtherPermission = _result.OtherPermission,
                    SameEmpName = _result.SamePermName,
                    SameEmpJobId = _result.SamePermJobId,
                    UploadFileName = _result.UploadFileName,
                    CreateDate = _result.CreateTimeStr,
                    AuditHistory = _result.MESOrderAuditHistory.CopyAToB<MESPermissionAuditHistoryModel>()
                };

                return View(_response);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpGet("[controller]/MES/Update/{orderSn}")]
        public IActionResult MESUpdate(int orderSn)
        {
            try
            {
                MESPermissionEntity _result = _demandDomainService.GetDetail(orderSn);

                MESPermissionCreateViewModel _response = new MESPermissionCreateViewModel
                {
                    OrderSn = _result.OrderSn,
                    OrderNo = _result.OrderNo,
                    Department = _result.Department,
                    SubUnit = _result.SubUnit,
                    Applicant = _result.Applicant,
                    JobId = _result.JobId,
                    MESOrderTypeId = _result.MESOrderTypeId,
                    Phone = _result.Phone,
                    ApplicantReason = _result.ApplicantReason,
                    PermissionList = _result.PermissionInfo.CopyAToB<MESPermissionModel>(),
                    ApplicantList = _result.Applicants.CopyAToB<MESApplicantModel>(),
                    OtherPermission = _result.OtherPermission,
                    SameEmpName = _result.SamePermName,
                    SameEmpJobId = _result.SamePermJobId,
                    CreateDate = _result.CreateTimeStr,
                };

                return View(_response);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpPost("[controller]/MES/Update")]
        public IActionResult MESUpdate(MESPermissionCreateViewModel updateModel)
        {
            try
            {
                var _result = _demandDomainService.UpdateMES(new MESPermissionEntity
                {
                    OrderSn = updateModel.OrderSn,
                    StatusId = updateModel.StatusId,
                    Department = updateModel.Department,
                    SubUnit = updateModel.SubUnit,
                    MESOrderTypeId = updateModel.MESOrderTypeId,
                    Applicant = updateModel.Applicant,
                    JobId = updateModel.JobId,
                    Phone = updateModel.Phone,
                    ApplicantReason = updateModel.ApplicantReason,
                    PermissionInfo = updateModel.PermissionList.Select(per => new MESPermissionDetailEntity
                    {
                        MESPermissionId = per.MESPermissionId,
                        IsEnable = per.IsEnable
                    }).ToList(),
                    OtherPermission = updateModel.OtherPermission,
                    SamePermName = updateModel.SameEmpName,
                    SamePermJobId = updateModel.SameEmpJobId,
                    Applicants = updateModel.ApplicantList.Select(emp => new MESApplicantEntity
                    {
                        ApplicantName = emp.ApplicantName,
                        ApplicantJobId = emp.ApplicantJobId
                    }).ToList()
                }, GetUserInfo());

                return Json(new { IsSuccess = _result.Item1, Msg = _result.Item2 });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }

        [HttpGet("[controller]/MES/Audit/{orderSn}")]
        public IActionResult MESAudit(int orderSn)
        {
            try
            {
                UserEntity _userInfo = GetUserInfo();
                var _userCurrentPagePermission = _userInfo.UserMenuPermissionList.FirstOrDefault(f => f.MenuSn == MenuEnum.MESPermission);
                ViewBag.UserPermission = new UserPermissionViewModel
                {
                    AccountSn = _userCurrentPagePermission.AccountSn,
                    MenuSn = _userCurrentPagePermission.MenuSn,
                    AccountPermission = _userCurrentPagePermission.AccountPermission
                };
                ViewBag.MESOrderType = new SelectList(_optionDomainService.GetMESType(), "Id", "Value");

                MESPermissionEntity _result = _demandDomainService.GetAudit(orderSn, _userInfo);

                MESPermissionDetailViewModel _response = new MESPermissionDetailViewModel
                {
                    OrderSn = _result.OrderSn,
                    OrderNo = _result.OrderNo,
                    StatusId = _result.StatusId,
                    Status = _result.Status,
                    Department = _result.Department,
                    SubUnit = _result.SubUnit,
                    MESOrderType = _result.MESOrderType,
                    MESOrderTypeId = _result.MESOrderTypeId,
                    Applicant = _result.Applicant,
                    JobId = _result.JobId,
                    Phone = _result.Phone,
                    ApplicantReason = _result.ApplicantReason,
                    UploadFileName = _result.UploadFileName,
                    PermissionList = _result.PermissionInfo.CopyAToB<MESPermissionModel>(),
                    ApplicantList = _result.Applicants.CopyAToB<MESApplicantModel>(),
                    OtherPermission = _result.OtherPermission,
                    SameEmpName = _result.SamePermName,
                    SameEmpJobId = _result.SamePermJobId,
                    CreateDate = _result.CreateTimeStr,
                    AuditHistory = _result.MESOrderAuditHistory.CopyAToB<MESPermissionAuditHistoryModel>()
                };

                return View(_response);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpPost("[controller]/MES/Audit")]
        public IActionResult MESAudit([FromForm] MESPermissionDetailViewModel approveViewModel)
        {
            try
            {
                string _result = _demandDomainService.AuditMES(approveViewModel.Remark, 
                    new MESPermissionEntity
                    {
                        OrderSn = approveViewModel.OrderSn,
                        StatusId = approveViewModel.StatusId,
                        ApplicantReason = approveViewModel.ApplicantReason,
                        MESOrderTypeId = approveViewModel.MESOrderTypeId
                    }, GetUserInfo());

                if (_result == "")
                    return Json(new { IsSuccess = true, Msg = "" });
                else
                    return Json(new { IsSuccess = false, Msg = _result });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }

        [HttpGet("[controller]/MES/AddInfo")]
        public IActionResult MESApplicantInfo()
        {
            try
            {
                return PartialView("_PartialMESEmp", new MESApplicantModel());
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpGet("[controller]/MES/Download/{orderSn}")]
        public IActionResult MPSDownload(int orderSn)
        {
            try
            {
                var _dwnlRes = _demandDomainService.Download(orderSn);

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

        #region IE layout 申請單

        [HttpGet("[controller]/IELayout")]
        public IActionResult IELayout()
        {
            try
            {
                UserEntity _userInfo = GetUserInfo();
                var _userCurrentPagePermission = _userInfo.UserMenuPermissionList.FirstOrDefault(f => f.MenuSn == MenuEnum.IELayout);

                ViewBag.UserPermission = new UserPermissionViewModel
                {
                    AccountSn = _userCurrentPagePermission.AccountSn,
                    MenuSn = _userCurrentPagePermission.MenuSn,
                    AccountPermission = _userCurrentPagePermission.AccountPermission
                };

                var _applyRes = _demandDomainService.GetList(_userInfo);

                List<IELayoutViewModel> _response = _applyRes.Select(res => new IELayoutViewModel
                { 
                    OrderSn = res.OrderSn,
                    OrderNo = res.OrderNo,
                    Status = res.Status,
                    ApplicantName = res.ApplicantName,
                    AuditName = res.AuditName,
                    ApplyDate = res.ApplyDateStr,
                    CreateDate = res.CreateTimeStr,
                    IssueRemark = res.IssueRemark
                }).ToList();

                return View(_response);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpGet("[controller]/IELayout/Create")]
        public IActionResult IELayoutCreate()
        {
            try
            {
                ViewBag.FactoryFloorOptions = _optionDomainService.GetFactoryFloorOptions();
                ViewBag.ProcessAreaOptions = _optionDomainService.GetProcessAreaOptions();
                ViewBag.FormatTypeOptions = _optionDomainService.GetFormatTypeOptions();
                ViewBag.ReasonTypeOptions = _optionDomainService.GetReasonTypeOptions();
                ViewBag.LayerTypeOptions = _optionDomainService.GetLayerTypeOptions();

                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpPost("[controller]/IELayout/Create")]
        public IActionResult IELayoutCreate([FromForm] IELayoutCreateViewModel createModel)
        {
            try
            {

                var _result = _demandDomainService.Create(createModel.CopyAToB<IELayoutCreateEntity>(), GetUserInfo());

                return Json(new ResponseViewModel<string>
                {
                    IsSuccess = string.IsNullOrEmpty(_result),
                    Msg = _result
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

        [HttpGet("[controller]/IELayout/Detail/{sn}")]
        public IActionResult IELayoutDetail(int sn)
        {
            try
            {
                ViewBag.FactoryFloorOptions = _optionDomainService.GetFactoryFloorOptions();
                ViewBag.ProcessAreaOptions = _optionDomainService.GetProcessAreaOptions();
                ViewBag.FormatTypeOptions = _optionDomainService.GetFormatTypeOptions();
                ViewBag.ReasonTypeOptions = _optionDomainService.GetReasonTypeOptions();
                ViewBag.LayerTypeOptions = _optionDomainService.GetLayerTypeOptions();

                var _layoutApplyRes = _demandDomainService.GetLayoutApplyDetail(sn);

                return View(new IELayoutDetailViewModel
                {
                    LayoutOrderInfo = new IELayoutViewModel
                    {
                        OrderSn = _layoutApplyRes.LayoutOrderInfo.OrderSn,
                        OrderNo = _layoutApplyRes.LayoutOrderInfo.OrderNo,
                        ApplicantName = _layoutApplyRes.LayoutOrderInfo.ApplicantName,
                        Department = _layoutApplyRes.LayoutOrderInfo.Department,
                        Phone = _layoutApplyRes.LayoutOrderInfo.Phone,
                        Status = _layoutApplyRes.LayoutOrderInfo.Status,
                        ApplyDate = _layoutApplyRes.LayoutOrderInfo.ApplyDateStr,
                        CreateDate = _layoutApplyRes.LayoutOrderInfo.CreateTimeStr,
                        AuditName = _layoutApplyRes.LayoutOrderInfo.AuditName,
                        FactoryFloor = _layoutApplyRes.LayoutOrderInfo.FactoryFloor,
                        ProcessArea = _layoutApplyRes.LayoutOrderInfo.ProcessArea,
                        PartRemark = _layoutApplyRes.LayoutOrderInfo.PartRemark,
                        FormatType = _layoutApplyRes.LayoutOrderInfo.FormatType,
                        ReasonTypeId = _layoutApplyRes.LayoutOrderInfo.ReasonTypeId,
                        Reason = _layoutApplyRes.LayoutOrderInfo.Reason,
                        LayerTypeId = _layoutApplyRes.LayoutOrderInfo.LayerTypeId,
                        IssueRemark = _layoutApplyRes.LayoutOrderInfo.IssueRemark,
                    },
                    AuditHistory = _layoutApplyRes.AuditList.Select(his => new IELayoutAuditViewModel
                    {
                        AuditSn = his.AuditSn,
                        AuditAccountSn = his.AuditAccountSn,
                        AuditName = his.AuditName,
                        AuditStatus = his.AuditStatus,
                        AuditStatusId = his.AuditStatusId,
                        ReceivedTime = his.ReceivedTime,
                        AuditTime = his.AuditTime,
                        DiffTime = his.DiffTime,
                        Remark = his.Remark
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        #endregion

        private (bool, string) Edit(DemanEditViewModel updModel, DemandStatusEnum newStatusId)
        {
            try
            {
                var _res = _demandDomainService.UpdateDemand(new DemandEntity
                {
                    OrderSn = updModel.OrderSn,
                    OrderNo = updModel.OrderId,
                    CategoryId = updModel.DemandCategoryId,
                    Subject = updModel.Subject,
                    Content = updModel.Content,
                    Applicant = updModel.Applicant,
                    JobNo = updModel.JobNo,
                    UploadFileList = updModel.UploadFile,
                    RejectReason = updModel.RejectReason,
                    Remark = updModel.Remark
                },
                GetUserInfo());

                return _res;
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
