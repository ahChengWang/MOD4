using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MOD4.Web.DomainService;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Models;
using MOD4.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.Controllers
{
    [Authorize]
    public class EquipmentController : Controller
    {
        private readonly IEquipmentDomainService _equipmentDomainService;

        public EquipmentController(IEquipmentDomainService equipmentDomainService)
        {
            _equipmentDomainService = equipmentDomainService;
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
                List<OptionViewModel> _eqpOptions = new List<OptionViewModel>() {
                    new OptionViewModel{ EquipmentId = 1,EquipmentDesc = "GDD100BC0010S"},
                    new OptionViewModel{ EquipmentId = 2,EquipmentDesc = "GDD101IA0030S"},
                    new OptionViewModel{ EquipmentId = 3,EquipmentDesc = "GDJ070IA7110S"},
                    new OptionViewModel{ EquipmentId = 4,EquipmentDesc = "GDD340IA0090S"},
                    new OptionViewModel{ EquipmentId = 5,EquipmentDesc = "GJ0900IA00150"},
                };

                SelectList _eqpSelect = new SelectList(_eqpOptions, "EquipmentId", "EquipmentDesc");

                var _res = _equipmentDomainService.GetEditEqpinfo(sn);

                return View(new EquipmentEditViewModel
                {
                    sn = _res.sn,
                    ToolId = _res.Equipment,
                    Code = _res.Code,
                    Codedesc = _res.CodeDesc,
                    Comment = _res.Comments,
                    StartTime = _res.StartTime.ToString("yyyy/MM/dd HH:mm:ss"),
                    Shift = _res.Shift ?? "",
                    EqUnit = _res.EqUnit ?? "",
                    MntUser = _res.MntUser ?? "",
                    MntMinutes = _res.MntMinutes ?? "",
                    DefectQty = _res.DefectQty,
                    DefectRate = _res.DefectRate ?? "",
                    Engineer = _res.Engineer ?? "",
                    Category = _res.Category ?? "",
                    Memo = _res.Memo ?? "",
                    IsPMProcess = isPM,
                    IsEngineerProcess = isEng,
                    SearchVal = searchVal,
                    EqpOptionList = _eqpSelect
                });
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
                    Shift = eidtModel.Shift,
                    EqUnit = eidtModel.EqUnit,
                    DefectQty = eidtModel.DefectQty,
                    DefectRate = eidtModel.DefectRate,
                    MntUser = eidtModel.MntUser,
                    MntMinutes = eidtModel.MntMinutes,
                    Engineer = eidtModel.Engineer,
                    Category = eidtModel.Category,
                    Memo = eidtModel.Memo,
                    IsPMProcess = eidtModel.IsPMProcess,
                    IsEngineerProcess = eidtModel.IsEngineerProcess
                });

                if (res != "")
                {
                    return Json(new { IsException = true, msg = $"錯誤：{res}" });
                }

                return Json("");
            }
            catch (Exception ex)
            {
                return Json(new { IsException = true, msg = $"錯誤：{ex.Message}" });
            }
        }


        [HttpGet]
        public IActionResult ModelNameOption([FromQuery] int id)
        {
            List<OptionViewModel> _test = new List<OptionViewModel>();

            switch (id)
            {
                case 1:
                    _test.AddRange(new List<OptionViewModel> {
                        new OptionViewModel{EquipmentId = 1,EquipmentDesc = "LCD+OCA" },
                        new OptionViewModel{EquipmentId = 2,EquipmentDesc = "TPM+LCD"},
                        new OptionViewModel{EquipmentId = 3,EquipmentDesc = "SG+OCA"},
                        new OptionViewModel{EquipmentId = 4,EquipmentDesc = "VCS SG"},
                    });
                    break;
                case 2:
                    _test.AddRange(new List<OptionViewModel> {
                        new OptionViewModel{EquipmentId = 1,EquipmentDesc = "PADI+LCD" },
                        new OptionViewModel{EquipmentId = 2,EquipmentDesc = "CG+SG"},
                        new OptionViewModel{EquipmentId = 3,EquipmentDesc = "SG"}
                    });
                    break;
                case 3:
                    _test.AddRange(new List<OptionViewModel> {
                        new OptionViewModel{EquipmentId = 1,EquipmentDesc = "OCA" },
                        new OptionViewModel{EquipmentId = 2,EquipmentDesc = "TPM"},
                        new OptionViewModel{EquipmentId = 3,EquipmentDesc = "FILM"}
                    });
                    break;
                case 4:
                    _test.AddRange(new List<OptionViewModel> {
                        new OptionViewModel{EquipmentId = 1,EquipmentDesc = "AZV" },
                        new OptionViewModel{EquipmentId = 2,EquipmentDesc = "PADI"},
                        new OptionViewModel{EquipmentId = 3,EquipmentDesc = "GM"}
                    });
                    break;
                case 5:
                    _test.AddRange(new List<OptionViewModel> {
                        new OptionViewModel{EquipmentId = 1,EquipmentDesc = "0" },
                        new OptionViewModel{EquipmentId = 2,EquipmentDesc = "1"},
                        new OptionViewModel{EquipmentId = 3,EquipmentDesc = "2"},
                        new OptionViewModel{EquipmentId = 4,EquipmentDesc = "3"}
                    });
                    break;
                default:
                    break;
            }

            return Json(_test);
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
                    Shift = _res.Shift ?? "",
                    EqUnit = _res.EqUnit ?? "",
                    MntUser = _res.MntUser ?? "",
                    MntMinutes = _res.MntMinutes ?? "",
                    DefectQty = _res.DefectQty,
                    DefectRate = _res.DefectRate ?? "",
                    Engineer = _res.Engineer ?? "",
                    Category = _res.Category ?? "",
                    Memo = _res.Memo ?? ""
                });
            }
            catch (Exception ex)
            {
                return Json(new { IsException = true, msg = $"錯誤：{ex.Message}" });
            }
        }
    }
}
