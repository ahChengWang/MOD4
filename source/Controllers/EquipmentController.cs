using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MOD4.Web.DomainService;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using MOD4.Web.Helper;
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
            IOptionDomainService optionDomainService)
            : base(httpContextAccessor)
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
        public IActionResult SearchRepairedEqHistory([FromQuery] string date, string toolIdList, string statusIdList)
        {
            try
            {
                var res = _equipmentDomainService.GetRepairedEqList(date, toolIdList, statusIdList);

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
                        RepairedTime = s.RepairTime,
                        MFGDay = s.MFGDay,
                        MFGHr = s.MFGHr,
                        IsPMProcess = s.IsPMProcess,
                        IsEngineerProcess = s.IsEngineerProcess
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

                ViewBag.ToolId = _equipmentDomainService.GetUnrepairedEqDropdown();
                ViewBag.RepairedToolId = _equipmentDomainService.GetRepairedEqDropdown();

                List<EquipmentEntity> _repairedEqList = new List<EquipmentEntity>();

                var _unrepairedList = _equipmentDomainService.GetUnrepairedEqList();

                if (searchConditions != null)
                    _repairedEqList = _equipmentDomainService.GetRepairedEqList(date: searchConditions[0], toolId: searchConditions[1], statusIdList: searchConditions[2]);
                else
                    _repairedEqList = _equipmentDomainService.GetRepairedEqList();

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
                        ToolId = s.ToolId,
                        ToolStatus = s.ToolStatus,
                        StatusCdsc = s.StatusCdsc,
                        UserId = s.UserId,
                        Comment = s.Comment,
                        LmTime = s.LmTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        RepairedTime = s.RepairTime,
                        MFGDay = s.MFGDay,
                        MFGHr = s.MFGHr,
                        IsPMProcess = s.IsPMProcess,
                        IsEngineerProcess = s.IsEngineerProcess
                    };
                }).ToList();

                return View(new EquipmentViewModel
                {
                    UnrepairedEqList = _unrepairedRes,
                    RepairedEqInfoList = _repairedEqRes
                });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Edit(int sn, int isPM, int isEng, string searchVal)
        {
            try
            {
                List<OptionViewModel> _shiftOptions = new List<OptionViewModel>() {
                    new OptionViewModel{ Id = 1,Value = "A"},
                    new OptionViewModel{ Id = 2,Value = "B"},
                    new OptionViewModel{ Id = 3,Value = "C"},
                    new OptionViewModel{ Id = 4,Value = "D"}
                };

                List<OptionViewModel> _processOptions = _optionDomainService.GetOptionByType(OptionTypeEnum.ProcessOption).CopyAToB<OptionViewModel>();

                SelectList _shiftSelect = new SelectList(_shiftOptions, "Id", "Value");
                SelectList _processSelect = new SelectList(_processOptions, "Id", "Value");
                SelectList _prioritySelect = new SelectList(new List<OptionViewModel>() 
                {
                    new OptionViewModel{ Id = 1,Value = "一般"},
                    new OptionViewModel{ Id = 2,Value = "嚴重"},
                    new OptionViewModel{ Id = 3,Value = "追蹤"}
                }, "Id", "Value");

                var _res = _equipmentDomainService.GetEditEqpinfo(sn, GetUserInfo());

                EquipmentEditViewModel _resModel = new EquipmentEditViewModel
                {
                    sn = _res.sn,
                    ToolId = _res.Equipment,
                    Code = _res.Code,
                    Codedesc = _res.CodeDesc,
                    Product = "GDD340IA0090S",
                    ProductShortName = "VCS",
                    ModelName = "VCS 1234",
                    Comment = _res.Comments,
                    StartTime = _res.StartTime.ToString("yyyy/MM/dd HH:mm:ss"),
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
                    Memo = _res.Memo ?? "",
                    IsPMProcess = isPM,
                    IsEngineerProcess = isEng,
                    SearchVal = searchVal,
                    ProcessOptionList = _processSelect,
                    ShiftOptionList = _shiftSelect,
                    PriorityOptionList = _prioritySelect
                };

                if (isPM == 0 && isEng == 1)
                {
                    _resModel.EqUnitOptionList =
                        new SelectList(_optionDomainService.GetOptionByType(OptionTypeEnum.EqUnit, _res.ProcessId, 0).CopyAToB<OptionViewModel>(), "Id", "Value");
                    _resModel.EqUnitPartOptionList =
                        new SelectList(_optionDomainService.GetOptionByType(OptionTypeEnum.EqUnitPart, _res.ProcessId, _res.EqUnitId).CopyAToB<OptionViewModel>(), "Id", "Value");
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
                    IsPMProcess = eidtModel.IsPMProcess,
                    IsEngineerProcess = eidtModel.IsEngineerProcess
                });

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
        public IActionResult GetSubOption([FromQuery] OptionTypeEnum optionTypeId, int mainId, int subId)
        {
            List<OptionViewModel> _processOptions = _optionDomainService.GetOptionByType(optionTypeId, mainId, subId).CopyAToB<OptionViewModel>();

            return Json(_processOptions);
        }

        [HttpGet]
        public IActionResult Detail([FromQuery] int sn)
        {
            try
            {
                var _res = _equipmentDomainService.GetEditEqpinfo(sn);

                if (_res == null)
                {
                    return Json(new { IsException = true, msg = $"查無資料" });
                }

                return PartialView("_PartialDetail", new EquipmentEditViewModel
                {
                    sn = _res.sn,
                    ToolId = _res.Equipment,
                    Code = _res.Code,
                    Codedesc = _res.CodeDesc,
                    Comment = _res.Comments,
                    StartTime = _res.StartTime.ToString("yyyy/MM/dd HH:mm:ss"),
                    Shift = _res.Shift,
                    MntUser = _res.MntUser ?? "",
                    MntMinutes = _res.MntMinutes ?? "",
                    DefectQty = _res.DefectQty,
                    DefectRate = _res.DefectRate ?? "",
                    Engineer = _res.Engineer ?? "",
                    PriorityId = _res.PriorityId,
                    Memo = _res.Memo ?? ""
                });
            }
            catch (Exception ex)
            {
                return Json(new { IsException = true, msg = $"錯誤：{ex.Message}" });
            }
        }

        [HttpGet]
        public IActionResult VerifyEqStatus([FromQuery] int eqsn, int isPM, int isEng)
        {
            try
            {

                string _verifyRes = _equipmentDomainService.VerifyEqpStatus(eqsn, isPM, isEng, GetUserInfo());

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
