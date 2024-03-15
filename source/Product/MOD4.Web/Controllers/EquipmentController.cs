using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MOD4.Web.DomainService;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using Utility.Helper;
using MOD4.Web.Models;
using MOD4.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.Controllers
{
    [Authorize]
    public class EquipmentController : BaseController
    {
        private readonly IEquipmentDomainService _equipmentDomainService;
        private readonly IOptionDomainService _optionDomainService;

        public EquipmentController(IEquipmentDomainService equipmentDomainService,
            IHttpContextAccessor httpContextAccessor,
            IOptionDomainService optionDomainService,
            IAccountDomainService accountDomainService)
            : base(httpContextAccessor, accountDomainService)
        {
            _equipmentDomainService = equipmentDomainService;
            _optionDomainService = optionDomainService;
        }


        [HttpGet]
        public IActionResult SearchUnrepairedEq([FromQuery] string date, string toolIdList)
        {
            try
            {
                var res = _equipmentDomainService.GetUnrepairedEqList(date, toolIdList);
                ViewBag.RoleId = GetUserInfo().Role;

                List<EquipmentDetailModel> response = res.Select(s =>
                {
                    return new EquipmentDetailModel()
                    {
                        sn = s.sn,
                        ToolId = s.ToolId,
                        ToolStatus = s.ToolStatus,
                        StatusCdsc = s.StatusCdsc,
                        UserId = s.UserId,
                        Comment = s.Comment,
                        LmTime = s.LmTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        MFGDay = s.MFGDay,
                        MFGHr = s.MFGHr
                    };
                }).ToList();

                return PartialView("_PartialUnrepaired", new EquipmentViewModel { UnrepairedEqList = response });
            }
            catch (Exception ex)
            {
                return Json(new { IsException = true, msg = $"錯誤：{ex.Message}" });
            }

        }

        [HttpGet]
        public IActionResult SearchRepairedEqHistory([FromQuery] string date, string toolIdList, string statusIdList, bool showAuto)
        {
            try
            {
                var res = _equipmentDomainService.GetRepairedEqList(date, toolIdList, statusIdList, showAuto: showAuto);
                ViewBag.RoleId = GetUserInfo().Role;

                //ViewBag.LineList = res.GroupBy(gb => gb.ToolId).Select(s => s.Key).ToList();

                List<EquipmentDetailModel> response = res.Select(s =>
                {
                    return new EquipmentDetailModel()
                    {
                        sn = s.sn,
                        ToolId = string.IsNullOrEmpty(s.ToolName) ? s.ToolId : $"{s.ToolId} ({s.ToolName})",
                        ToolStatus = s.ToolStatus,
                        StatusCdsc = s.StatusCdsc,
                        UserId = s.UserId,
                        Comment = s.Comment,
                        LmTime = s.LmTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        RepairedTime = s.RepairTime,
                        MFGDay = s.MFGDay,
                        MFGHr = s.MFGHr,
                        StatusId = s.StatusId
                    };
                }).ToList();

                return PartialView("_PartialEqHisotry", new EquipmentViewModel { RepairedEqInfoList = response });
            }
            catch (Exception ex)
            {
                return Json(new { IsException = true, msg = $"錯誤：{ex.Message}" });
            }
        }

        public IActionResult Index(string searchVal = "")
        {
            try
            {
                string[] searchConditions = null;
                if (searchVal != "")
                {
                    searchConditions = searchVal.Split(";");
                }

                ViewBag.ToolId = _equipmentDomainService.GetUnRepaireEqOptions();
                ViewBag.RepairedToolId = _equipmentDomainService.GetRepairedEqOptions(null);  //_equipmentDomainService.GetEqPageDropdown();
                ViewBag.RoleId = GetUserInfo().Role;

                List<EquipmentEntity> _repairedEqList = new List<EquipmentEntity>();
                int _pmPending = 0;
                int _engPending = 0;

                var _unrepairedList = _equipmentDomainService.GetUnrepairedEqList();

                if (searchConditions != null)
                    _repairedEqList = _equipmentDomainService.GetRepairedEqList(date: searchConditions[0], toolId: searchConditions[1], statusIdList: searchConditions[2]);
                else
                    _repairedEqList = _equipmentDomainService.GetRepairedEqList();

                (_pmPending, _engPending) = _equipmentDomainService.GetTodayRepairedEqPendingList();

                List<EquipmentDetailModel> _unrepairedRes = _unrepairedList.Select(s =>
                {
                    return new EquipmentDetailModel()
                    {
                        sn = s.sn,
                        ToolId = s.ToolId,
                        ToolStatus = s.ToolStatus,
                        StatusCdsc = s.StatusCdsc,
                        UserId = s.UserId,
                        Comment = s.Comment,
                        LmTime = s.LmTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        MFGDay = s.MFGDay,
                        MFGHr = s.MFGHr
                    };
                }).ToList();

                List<EquipmentDetailModel> _repairedEqRes = _repairedEqList.Select(s =>
                {
                    return new EquipmentDetailModel()
                    {
                        sn = s.sn,
                        ToolId = string.IsNullOrEmpty(s.ToolName) ? s.ToolId : $"{s.ToolId}({s.ToolName})",
                        ToolStatus = s.ToolStatus,
                        StatusCdsc = s.StatusCdsc,
                        UserId = s.UserId,
                        Comment = s.Comment,
                        LmTime = s.LmTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        RepairedTime = s.RepairTime,
                        MFGDay = s.MFGDay,
                        MFGHr = s.MFGHr,
                        StatusId = s.StatusId
                    };
                }).ToList();

                return View(new EquipmentViewModel
                {
                    UnrepairedEqList = _unrepairedRes,
                    RepairedEqInfoList = _repairedEqRes,
                    PMPending = _pmPending,
                    ENGPending = _engPending
                });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Edit(int sn, EqIssueStatusEnum statusId, string searchVal)
        {
            try
            {
                SelectList _shiftSelect = new SelectList(_optionDomainService.GetShiftOptionList().CopyAToB<OptionViewModel>(), "Id", "Value");
                SelectList _processSelect = new SelectList(_optionDomainService.GetEqProcessOptionByType(OptionTypeEnum.ProcessOption).CopyAToB<OptionViewModel>(), "Id", "Value");
                SelectList _prioritySelect = new SelectList(_optionDomainService.GetPriorityOptionList().CopyAToB<OptionViewModel>(), "Id", "Value");
                SelectList _eqEvenCodeSelect = new SelectList(_optionDomainService.GetEqEvenCodeOptionList().CopyAToB<OptionViewModel>(), "Id", "Value");

                var _res = _equipmentDomainService.GetEditEqpinfo(sn, GetUserInfo());

                EquipmentEditViewModel _resModel = new EquipmentEditViewModel
                {
                    sn = _res.sn,
                    ToolId = string.IsNullOrEmpty(_res.ToolName) ? _res.Equipment : $"{_res.Equipment} ({_res.ToolName})",
                    Code = _res.Code,
                    Codedesc = _res.CodeDesc,
                    Product = _res.Product,
                    ProductShortName = _res.ProductName,
                    //ModelName = "VCS 1234",
                    Comment = _res.Comments,
                    StartTime = _res.StartTime,
                    RepairedTime = _res.RepairedTime,
                    Shift = _res.Shift,
                    ProcessId = _res.ProcessId,
                    EqUnitId = _res.EqUnitId,
                    EqUnitPartId = _res.EqUnitPartId,
                    MntUser = _res.MntUser ?? "",
                    MntMinutes = _res.MntMinutes ?? "",
                    DefectQty = _res.DefectQty,
                    DefectRate = _res.DefectRate ?? "",
                    Engineer = _res.Engineer ?? "",
                    PriorityId = _res.PriorityId,
                    TypeId = _res.TypeId,
                    TypeDesc = _res.TypeDesc,
                    YId = _res.YId,
                    YDesc = _res.YDesc,
                    SubYId = _res.SubYId,
                    SubYDesc = _res.SubYDesc,
                    XId = _res.XId,
                    XDesc = _res.XDesc,
                    SubXId = _res.SubXId,
                    SubXDesc = _res.SubXDesc,
                    RId = _res.RId,
                    ENGTypeDesc = _res.ENGTypeDesc,
                    ENGTypeId = _res.ENGTypeId,
                    ENGYId = _res.ENGYId,
                    ENGYDesc = _res.ENGYDesc,
                    ENGSubYId = _res.ENGSubYId,
                    ENGSubYDesc = _res.ENGSubYDesc,
                    ENGXId = _res.ENGXId,
                    ENGXDesc = _res.ENGXDesc,
                    ENGSubXId = _res.ENGSubXId,
                    ENGSubXDesc = _res.ENGSubXDesc,
                    ENGRId = _res.ENGRId,
                    ENGRDesc = _res.ENGRDesc,
                    Memo = _res.Memo ?? "",
                    SearchVal = searchVal,
                    StatusId = _res.StatusId,
                    ProcessOptionList = _processSelect,
                    ShiftOptionList = _shiftSelect,
                    PriorityOptionList = _prioritySelect,
                    EvenCodeOptionList = _eqEvenCodeSelect,
                };

                if (statusId == EqIssueStatusEnum.PendingENG)
                {
                    _resModel.EqUnitOptionList =
                        new SelectList(_optionDomainService.GetEqProcessOptionByType(OptionTypeEnum.EqUnit, _res.ProcessId, 0).CopyAToB<OptionViewModel>(), "Id", "Value");
                    _resModel.EqUnitPartOptionList =
                        new SelectList(_optionDomainService.GetEqProcessOptionByType(OptionTypeEnum.EqUnitPart, _res.ProcessId, _res.EqUnitId).CopyAToB<OptionViewModel>(), "Id", "Value");
                    _resModel.EvenCodeYOptionList =
                        new SelectList(_optionDomainService.GetEqEvenCodeOptionList(_res.TypeId).CopyAToB<OptionViewModel>(), "Id", "Value");
                    _resModel.EvenCodeSubYOptionList =
                        new SelectList(_optionDomainService.GetEqEvenCodeOptionList(_res.TypeId, _res.YId).CopyAToB<OptionViewModel>(), "Id", "Value");
                    _resModel.EvenCodeXOptionList = 
                        new SelectList(_optionDomainService.GetEqEvenCodeOptionList(_res.TypeId, _res.YId, _res.SubYId).CopyAToB<OptionViewModel>(), "Id", "Value");
                    _resModel.EvenCodeSubXOptionList = 
                        new SelectList(_optionDomainService.GetEqEvenCodeOptionList(_res.TypeId, _res.YId, _res.SubYId, _res.XId).CopyAToB<OptionViewModel>(), "Id", "Value");
                    _resModel.EvenCodeROptionList = 
                        new SelectList(_optionDomainService.GetEqEvenCodeOptionList(_res.TypeId, _res.YId, _res.SubYId, _res.XId, _res.SubXId).CopyAToB<OptionViewModel>(), "Id", "Value");
                }

                return View(_resModel);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Edit([FromForm] EquipmentEditViewModel eidtModel)
        {
            try
            {
                var res = _equipmentDomainService.UpdateEqpinfo(new EquipmentEditEntity
                {
                    sn = eidtModel.sn,
                    Comments = eidtModel.Comment,
                    ProcessId = eidtModel.ProcessId,
                    EqUnitId = eidtModel.EqUnitId,
                    EqUnitPartId = eidtModel.EqUnitPartId,
                    Shift = eidtModel.Shift,
                    DefectQty = eidtModel.DefectQty,
                    DefectRate = eidtModel.DefectRate,
                    MntUser = eidtModel.MntUser,
                    MntMinutes = eidtModel.MntMinutes,
                    Engineer = eidtModel.Engineer,
                    PriorityId = eidtModel.PriorityId,
                    Memo = eidtModel.Memo,
                    TypeId = eidtModel.TypeId,
                    YId = eidtModel.YId,
                    SubYId = eidtModel.SubYId,
                    XId = eidtModel.XId,
                    SubXId = eidtModel.SubXId,
                    RId = eidtModel.RId,
                    ENGTypeId = eidtModel.ENGTypeId,
                    ENGYId = eidtModel.ENGYId,
                    ENGSubYId = eidtModel.ENGSubYId,
                    ENGXId = eidtModel.ENGXId,
                    ENGSubXId = eidtModel.ENGSubXId,
                    ENGRId = eidtModel.ENGRId,
                    StatusId = eidtModel.StatusId
                }, GetUserInfo());

                if (res != "")
                {
                    return Json($"錯誤：{res}");
                }

                return Json("");
            }
            catch (Exception ex)
            {
                return Json($"錯誤：{ex.Message}");
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            try
            {
                ViewBag.ShiftSelect = new SelectList(_optionDomainService.GetShiftOptionList().CopyAToB<OptionViewModel>(), "Id", "Value");
                ViewBag.ProcessSelect = new SelectList(_optionDomainService.GetEqProcessOptionByType(OptionTypeEnum.ProcessOption).CopyAToB<OptionViewModel>(), "Id", "Value");
                ViewBag.ProcessAllOptions = _optionDomainService.GetAllEqProcessOption().CopyAToB<EqProcessOptionlViewModel>();
                ViewBag.EqEvenCodeSelect = new SelectList(_optionDomainService.GetEqEvenCodeOptionList().CopyAToB<OptionViewModel>(), "Id", "Value");
                ViewBag.EqAllEvenCodeOptionList = _optionDomainService.GetAllEqEvenCodeOptionList().CopyAToB<EqEvenCodeOptionlViewModel>();
                ViewBag.ProductOption = _optionDomainService.GetLcmProdDescOptions();
                ViewBag.EqIDMappingOption = _optionDomainService.GetEqIDAreaList();

                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Create([FromForm] EquipmentCreateViewModel createVM)
        {
            try
            {
                var res = _equipmentDomainService.Create(new EquipmentEditEntity
                {
                    StartTime = createVM.StartTime,
                    EndTime = createVM.EndTime,
                    ProductId = createVM.ProductId,
                    Equipment = createVM.ToolId,
                    Code = createVM.Code,
                    CodeDesc = createVM.Codedesc,
                    DownType = createVM.DownType,
                    Comments = createVM.Comment,
                    ProcessId = createVM.ProcessId,
                    EqUnitId = createVM.EqUnitId,
                    EqUnitPartId = createVM.EqUnitPartId,
                    DefectQty = createVM.DefectQty,
                    DefectRate = createVM.DefectRate,
                    Shift = createVM.Shift,
                    MntMinutes = createVM.MntMinutes,
                    Memo = createVM.Memo,
                    TypeId = createVM.TypeId,
                    YId = createVM.YId,
                    SubYId = createVM.SubYId,
                    XId = createVM.XId,
                    SubXId = createVM.SubXId,
                    RId = createVM.RId,
                }, GetUserInfo());

                return Json("");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetSubOption([FromQuery] OptionTypeEnum optionTypeId, int mainId, int subId)
        {
            List<OptionViewModel> _processOptions = _optionDomainService.GetEqProcessOptionByType(optionTypeId, mainId, subId).CopyAToB<OptionViewModel>();

            return Json(_processOptions);
        }

        [HttpGet]
        public IActionResult GetEvenCodeOption([FromQuery] int typeId, int yId = 0, int subYId = 0, int xId = 0, int subXId = 0, int rId = 0)
        {
            List<OptionViewModel> _processOptions = _optionDomainService.GetEqEvenCodeOptionList(typeId, yId, subYId, xId, subXId, rId).CopyAToB<OptionViewModel>();

            return Json(_processOptions);
        }

        [HttpGet]
        public IActionResult Detail([FromQuery] int sn)
        {
            try
            {
                var _res = _equipmentDomainService.GetDetailEqpinfo(sn);

                if (_res == null)
                {
                    return Json(new { IsException = true, msg = $"查無資料" });
                }

                return PartialView("_PartialDetail", new EquipmentDetailViewModel
                {
                    sn = _res.sn,
                    ToolId = string.IsNullOrEmpty(_res.ToolName) ? _res.Equipment : $"{_res.Equipment} ({_res.ToolName})",
                    Product = _res.Product,
                    ProductShortName = "",
                    ModelName = "",
                    Code = _res.Code,
                    Codedesc = _res.CodeDesc,
                    Comment = _res.Comments,
                    StartTime = _res.StartTime,
                    Shift = _res.ShiftDesc,
                    Process = _res.Process,
                    EqUnit = _res.EqUnit,
                    EqUnitPart = _res.EqUnitPart,
                    MntUser = _res.MntUser ?? "",
                    Type = _res.TypeDesc,
                    Y = _res.YDesc,
                    SubY = _res.SubYDesc,
                    X = _res.XDesc,
                    SubX = _res.SubXDesc,
                    R = _res.RDesc,
                    ENGType = _res.ENGTypeDesc,
                    ENGY = _res.ENGYDesc,
                    ENGSubY = _res.ENGSubYDesc,
                    ENGX = _res.ENGXDesc,
                    ENGSubX = _res.ENGSubXDesc,
                    ENGR = _res.ENGRDesc,
                    MntMinutes = _res.MntMinutes ?? "",
                    DefectQty = _res.DefectQty,
                    DefectRate = _res.DefectRate ?? "",
                    Engineer = _res.Engineer ?? "",
                    Priority = _res.Priority,
                    Memo = _res.Memo ?? ""
                });
            }
            catch (Exception ex)
            {
                return Json(new { IsException = true, msg = $"錯誤：{ex.Message}" });
            }
        }

        [HttpGet]
        public IActionResult VerifyEqStatus([FromQuery] int eqsn, EqIssueStatusEnum statusId)
        {
            try
            {
                string _verifyRes = _equipmentDomainService.VerifyEqpStatus(eqsn, statusId, GetUserInfo());

                if (_verifyRes != "")
                {
                    return Json(_verifyRes);
                }

                return Json("");
            }
            catch (Exception ex)
            {
                return Json($"錯誤：{ex.Message}");
            }
        }

        public IActionResult Dashboard()
        {
            try
            {
                //DateTime dt = DateTime.Now.StartOfWeek(DayOfWeek.Monday);


                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

    }
}
