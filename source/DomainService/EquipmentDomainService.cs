using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using MOD4.Web.Helper;
using MOD4.Web.Repostory;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace MOD4.Web.DomainService
{
    public class EquipmentDomainService : IEquipmentDomainService
    {
        private readonly IAlarmXmlRepository _alarmXmlRepository;
        private readonly IEqpInfoRepository _eqpInfoRepository;
        private readonly IOptionDomainService _optionDomainService;

        public EquipmentDomainService(IAlarmXmlRepository alarmXmlRepository, 
            IEqpInfoRepository eqpInfoRepository,
            IOptionDomainService optionDomainService)
        {
            _alarmXmlRepository = alarmXmlRepository;
            _eqpInfoRepository = eqpInfoRepository;
            _optionDomainService = optionDomainService;
        }

        public List<string> GetUnrepairedEqDropdown()
        {
            try
            {
                return _alarmXmlRepository.SelectToolList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<string> GetRepairedEqDropdown(string date = null)
        {
            try
            {
                string beginDTE = DateTime.Now.ToString("yyyy-MM") + "-01";

                return _eqpInfoRepository.SelectToolList(date ?? beginDTE);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EquipmentEntity> GetUnrepairedEqList(string date = null, string toolId = null)
        {
            try
            {
                string beginDTE = DateTime.Now.ToString("yyyy-MM-dd");

                var _alarmList = _alarmXmlRepository.SelectByConditions(date ?? beginDTE, toolId?.Split(",").ToList() ?? null);

                return (_alarmList.OrderByDescending(o => o.lm_time).Select(s =>
                {
                    return new EquipmentEntity
                    {
                        sn = s.sn,
                        ToolId = s.tool_id,
                        ToolStatus = s.tool_status,
                        StatusCdsc = s.status_cdsc,
                        UserId = s.user_id,
                        Comment = s.comment,
                        LmTime = s.lm_time,
                        XMLTime = s.XML_time.ToString("yyyy-MM-dd HH:mm:ss"),
                        MFGDay = s.MFG_Day.ToString("yyyy-MM-dd"),
                        MFGHr = s.MFG_HR.ToString()
                    };
                })).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EquipmentEntity> GetRepairedEqList(string date = null, string toolId = null, string statusIdList = null)
        {
            try
            {
                string beginDTE = DateTime.Now.ToString("yyyy-MM-dd");

                List<EqpInfoDao> _resEquipmentList = new List<EqpInfoDao>();

                var _eqpInfoList = _eqpInfoRepository.SelectByConditions(date ?? beginDTE, string.IsNullOrEmpty(toolId) ? null : toolId.Split(",").ToList() ?? null, date == null);

                if (!string.IsNullOrEmpty(statusIdList))
                {
                    List<int> _statusIdList = !string.IsNullOrEmpty(statusIdList) ? statusIdList.Split(',').Select(s => Convert.ToInt32(s)).ToList() : new List<int>();

                    _statusIdList.ForEach(fe =>
                    {
                        switch (fe)
                        {
                            case 1:
                                _resEquipmentList.AddRange(_eqpInfoList.Where(w => string.IsNullOrEmpty(w.mnt_user)).ToList());
                                break;
                            case 2:
                                _resEquipmentList.AddRange(_eqpInfoList.Where(w => !string.IsNullOrEmpty(w.mnt_user) && string.IsNullOrEmpty(w.engineer)).ToList());
                                break;
                            case 3:
                                _resEquipmentList.AddRange(_eqpInfoList.Where(w => !string.IsNullOrEmpty(w.mnt_user) && !string.IsNullOrEmpty(w.engineer)).ToList());
                                break;
                            default:
                                break;
                        }
                    });
                }
                else
                {
                    _resEquipmentList = _eqpInfoList;
                }

                return (_resEquipmentList.OrderByDescending(o => o.Start_Time).Select(s =>
                {
                    return new EquipmentEntity
                    {
                        sn = s.sn,
                        ToolId = s.Equipment,
                        ToolStatus = s.Code,
                        StatusCdsc = s.Code_Desc,
                        UserId = s.Operator,
                        Comment = s.Comments,
                        LmTime = s.Start_Time,
                        RepairTime = s.Repair_Time,
                        MFGDay = s.MFG_Day.ToString("yyyy-MM-dd"),
                        MFGHr = s.MFG_HR.ToString(),
                        IsPMProcess = s.shift == 0 && string.IsNullOrEmpty(s.mnt_user),
                        IsEngineerProcess = !string.IsNullOrEmpty(s.mnt_user) && string.IsNullOrEmpty(s.engineer)
                    };
                })).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EquipmentEditEntity GetEditEqpinfo(int sn, UserEntity userEntity = null)
        {
            try
            {
                var _r = _eqpInfoRepository.SelectEqpinfoByConditions(sn);

                if (_r == null)
                {
                    return null;
                }

                if (userEntity != null)
                    CatchHelper.Set($"Eq_Edit:{sn}", userEntity.Account, 600);

                return new EquipmentEditEntity
                {
                    sn = _r.sn,
                    Equipment = _r.Equipment,
                    Code = _r.Code,
                    CodeDesc = _r.Code_Desc,
                    Operator = _r.Operator,
                    Comments = _r.Comments,
                    StartTime = _r.Start_Time,
                    UpdateTime = _r.Update_Time,
                    Shift = _r.shift,
                    ProcessId = _r.processId,
                    EqUnitId = _r.eq_unitId,
                    EqUnitPartId = _r.eq_unit_partId,
                    DefectQty = _r.defect_qty,
                    DefectRate = _r.defect_rate,
                    Engineer = _r.engineer,
                    Memo = _r.memo,
                    MntUser = _r.mnt_user,
                    MntMinutes = _r.mnt_minutes
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public EquipmentEditEntity GetDetailEqpinfo(int sn)
        {
            try
            {
                var _r = _eqpInfoRepository.SelectEqpinfoByConditions(sn);

                if (_r == null)
                {
                    return null;
                }

                var _processOptions = _optionDomainService.GetOptionByType(OptionTypeEnum.ProcessOption);
                var _eqUnitOptions = _optionDomainService.GetOptionByType(OptionTypeEnum.EqUnit, _r.processId);
                var _eqUnitPartOptions = _optionDomainService.GetOptionByType(OptionTypeEnum.EqUnitPart, _r.processId,_r.eq_unitId);

                return new EquipmentEditEntity
                {
                    sn = _r.sn,
                    Equipment = _r.Equipment,
                    Code = _r.Code,
                    CodeDesc = _r.Code_Desc,
                    Operator = _r.Operator,
                    Comments = _r.Comments,
                    StartTime = _r.Start_Time,
                    UpdateTime = _r.Update_Time,
                    Shift = _r.shift,
                    ProcessId = _r.processId,
                    Process = _processOptions.FirstOrDefault(w => w.Id == _r.processId).Value,
                    EqUnitId = _r.eq_unitId,
                    EqUnit = _eqUnitOptions.FirstOrDefault(w => w.Id == _r.eq_unitId).Value,
                    EqUnitPartId = _r.eq_unit_partId,
                    EqUnitPart = _eqUnitPartOptions.FirstOrDefault(w => w.Id == _r.eq_unit_partId).Value,
                    DefectQty = _r.defect_qty,
                    DefectRate = _r.defect_rate,
                    Engineer = _r.engineer,
                    Memo = _r.memo,
                    MntUser = _r.mnt_user,
                    MntMinutes = _r.mnt_minutes
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }





        public string VerifyEqpStatus(int sn, int isPM, int isEng, UserEntity userEntity)
        {
            try
            {
                var _catchInfo = CatchHelper.Get($"Eq_Edit:{sn}");

                if (_catchInfo != null && _catchInfo != userEntity.Account)
                {
                    return $"{_catchInfo} 編輯機況中";
                }

                if ((isPM != 0 && isPM != 1 && isEng != 0 && isEng != 1) || (isPM == isEng))
                    return "參數異常";

                var _eqpinfo = _eqpInfoRepository.SelectEqpinfoByConditions(sn);

                if (isPM == 1 && !string.IsNullOrEmpty(_eqpinfo.mnt_user))
                    return "機況已更新";
                else if (isEng == 1 && !string.IsNullOrEmpty(_eqpinfo.engineer))
                    return "機況已更新";

                return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EquipmentEntity> GetEntityHistoryDetail(string mfgDay, List<string> eqpListStr)
        {
            try
            {
                var _eqpHisList = _eqpInfoRepository.SelectByConditions(mfgDay, eqpListStr, false);

                return (_eqpHisList.Select(s =>
                {
                    return new EquipmentEntity
                    {
                        ToolId = s.Equipment,
                        ToolStatus = s.Code,
                        StatusCdsc = s.Code_Desc,
                        Comment = s.Comments,
                        RepairTime = s.Repair_Time,
                        LmTime = s.Start_Time,
                        MFGDay = s.MFG_Day.ToString("yyyy-MM-dd")
                    };
                })).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string UpdateEqpinfo(EquipmentEditEntity editEntity)
        {
            try
            {
                var _updateResponse = "";
                var oldEqpinfo = _eqpInfoRepository.SelectEqpinfoByConditions(editEntity.sn);

                if ((editEntity.IsPMProcess == 1 && editEntity.IsEngineerProcess == 1) ||
                    (editEntity.IsPMProcess == 0 && editEntity.IsEngineerProcess == 0))
                    return "Process error";

                if (oldEqpinfo == null)
                    return "查無機況";

                EqpInfoDao _updEqpinfo = new EqpInfoDao
                {
                    sn = editEntity.sn
                };

                if (editEntity.IsPMProcess == 1)
                {
                    _updEqpinfo.Comments = editEntity.Comments;
                    _updEqpinfo.processId = editEntity.ProcessId;
                    _updEqpinfo.eq_unitId = editEntity.EqUnitId;
                    _updEqpinfo.eq_unit_partId = editEntity.EqUnitPartId;
                    _updEqpinfo.shift = editEntity.Shift;
                    _updEqpinfo.defect_qty = editEntity.DefectQty;
                    _updEqpinfo.defect_rate = editEntity.DefectRate;
                    _updEqpinfo.mnt_user = editEntity.MntUser;
                    _updEqpinfo.mnt_minutes = editEntity.MntMinutes;
                }
                else if (editEntity.IsEngineerProcess == 1)
                {
                    _updEqpinfo.priorityId = editEntity.PriorityId;
                    _updEqpinfo.engineer = editEntity.Engineer;
                    _updEqpinfo.memo = editEntity.Memo;
                }

                using (var scope = new TransactionScope())
                {
                    var _uprdResult = false;

                    if (editEntity.IsPMProcess == 1)
                        _uprdResult = _eqpInfoRepository.UpdateEqpinfoByPM(_updEqpinfo) == 2; //含觸發自動更新 updateTime trigger
                    else if (editEntity.IsEngineerProcess == 1)
                        _uprdResult = _eqpInfoRepository.UpdateEqpinfoByENG(_updEqpinfo) == 2; //含觸發自動更新 updateTime trigger

                    if (_uprdResult)
                    {
                        CatchHelper.Delete(new string[] { $"Eq_Edit:{editEntity.sn}" });
                        scope.Complete();
                    }
                    else
                        _updateResponse = "更新失敗";

                }

                return _updateResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
