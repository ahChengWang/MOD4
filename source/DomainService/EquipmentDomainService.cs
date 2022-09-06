using Helper;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Repostory;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Transactions;
using System.Xml.Linq;

namespace MOD4.Web.DomainService
{
    public class EquipmentDomainService : IEquipmentDomainService
    {
        private readonly IAlarmXmlRepository _alarmXmlRepository;
        private readonly IEqpInfoRepository _eqpInfoRepository;

        public EquipmentDomainService(IAlarmXmlRepository alarmXmlRepository, IEqpInfoRepository eqpInfoRepository)
        {
            _alarmXmlRepository = alarmXmlRepository;
            _eqpInfoRepository = eqpInfoRepository;
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
                        IsPMProcess = string.IsNullOrEmpty(s.shift) && string.IsNullOrEmpty(s.mnt_user),
                        IsEngineerProcess = !string.IsNullOrEmpty(s.mnt_user) && string.IsNullOrEmpty(s.engineer)
                    };
                })).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EquipmentEditEntity GetEditEqpinfo(int sn)
        {
            try
            {
                var _r = _eqpInfoRepository.SelectEqpinfoByConditions(sn);

                if (_r == null)
                {
                    return null;
                }

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
                    EqUnit = _r.eq_unit,
                    DefectQty = _r.defect_qty,
                    DefectRate = _r.defect_rate,
                    Engineer = _r.engineer,
                    Category = _r.category.ToString(),
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
                var oldEqpinfo = GetEditEqpinfo(editEntity.sn);

                if (oldEqpinfo == null)
                {
                    return "error";
                }

                EqpInfoDao _updEqpinfo = new EqpInfoDao
                {
                    sn = editEntity.sn
                };

                if (editEntity.IsPMProcess == 1)
                {
                    _updEqpinfo.Comments = editEntity.Comments;
                    _updEqpinfo.shift = editEntity.Shift;
                    _updEqpinfo.eq_unit = editEntity.EqUnit;
                    _updEqpinfo.defect_qty = editEntity.DefectQty;
                    _updEqpinfo.defect_rate = editEntity.DefectRate;
                    _updEqpinfo.mnt_user = editEntity.MntUser;
                    _updEqpinfo.mnt_minutes = editEntity.MntMinutes;
                }
                if (editEntity.IsEngineerProcess == 1)
                {
                    _updEqpinfo.Comments = oldEqpinfo.Comments;
                    _updEqpinfo.shift = oldEqpinfo.Shift;
                    _updEqpinfo.eq_unit = oldEqpinfo.EqUnit;
                    _updEqpinfo.defect_qty = oldEqpinfo.DefectQty;
                    _updEqpinfo.defect_rate = oldEqpinfo.DefectRate;
                    _updEqpinfo.mnt_user = oldEqpinfo.MntUser;
                    _updEqpinfo.mnt_minutes = oldEqpinfo.MntMinutes;
                    _updEqpinfo.category = 1;
                    _updEqpinfo.engineer = editEntity.Engineer;
                    _updEqpinfo.memo = editEntity.Memo;
                }

                using (var scope = new TransactionScope())
                {
                    var _uprdResult = false;

                    _uprdResult = _eqpInfoRepository.UpdateEqpinfo(_updEqpinfo) == 2; //含觸發自動更新 updateTime trigger

                    if (_uprdResult)
                        scope.Complete();

                }

                return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
