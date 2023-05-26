using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
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

        public List<(string, List<string>)> GetUnRepaireEqOptions()
        {
            try
            {
                DateTime _mfgDate = DateTime.Now;

                string _beginDTE = $"{_mfgDate.AddMonths(-3).ToString("yyyy-MM-dd")} 07:30:00";
                string _endDTE = $"{_mfgDate.ToString("yyyy-MM-dd")} 07:30:00";

                var _repairedEqList = _eqpInfoRepository.SelectUnRepaireEqList(_beginDTE, _endDTE);

                return _repairedEqList.GroupBy(g => g.AREA).Select(s => (s.Key, s.Select(ss => ss.EQUIP_NBR).ToList())).ToList();

                //return _optionDomainService.GetAreaEqGroupOptions();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<(string, List<string>)> GetRepairedEqDropdown(string date = null)
        {
            try
            {
                string beginDTE = DateTime.Now.AddMonths(-2).ToString("yyyy-MM") + "-01";

                var _eqpInfoList = _eqpInfoRepository.SelectToolList(date ?? beginDTE);

                return new List<(string, List<string>)> {
                    ("BONDING", _eqpInfoList.Where(w => "ACOG,AOLB,PCBI,AOVC".Contains(w.Substring(0,4))).ToList()),
                    ("LAM", _eqpInfoList.Where(w => "AFOG,CLAM,PTI2,LEST".Contains(w.Substring(0,4))).ToList()),
                    ("ASSY", _eqpInfoList.Where(w => "ASSY,AATS,OTP,AAFC,DMUR,AAT2,M3UV".Contains(w.Substring(0,4))).ToList()),
                    ("ACDP", _eqpInfoList.Where(w => "CKEN,ACKE,DKEN,DTWO".Contains(w.Substring(0,4))).ToList())
                };
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

        public List<EquipmentEntity> GetRepairedEqList(string date = null, string toolId = null, string statusIdList = null, bool showAuto = false)
        {
            try
            {
                string beginDTE = DateTime.Now.ToString("yyyy-MM-dd");

                List<EqpInfoDao> _resEquipmentList = new List<EqpInfoDao>();

                var _eqpInfoList = _eqpInfoRepository.SelectByConditions(
                        date ?? beginDTE,
                        string.IsNullOrEmpty(toolId) ? null : toolId.Split(",").ToList() ?? null,
                        date == null,
                        showAuto);

                var _eqpDic = _eqpInfoList.GroupBy(g => g.statusId).ToDictionary(dic => dic.Key, dic => dic.ToList());

                if (!string.IsNullOrEmpty(statusIdList))
                {
                    List<EqIssueStatusEnum> _statusIdList = statusIdList.Split(',').Select(s => (EqIssueStatusEnum)Convert.ToInt32(s)).ToList();

                    _statusIdList.ForEach(fe =>
                    {
                        _resEquipmentList.AddRange(_eqpDic[fe]);
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
                        ToolName = s.tool_name,
                        ToolStatus = s.Code,
                        StatusCdsc = s.Code_Desc,
                        UserId = s.Operator,
                        Comment = s.Comments,
                        LmTime = s.Start_Time,
                        RepairTime = s.Repair_Time,
                        MFGDay = s.MFG_Day.ToString("yyyy-MM-dd"),
                        MFGHr = s.MFG_HR.ToString(),
                        StatusId = s.statusId
                    };
                })).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<(string, List<string>)> GetRepairedEqOptions(string mfgDate)
        {
            try
            {
                DateTime _mfgDate = DateTime.Now;

                string _beginDTE = $"{_mfgDate.AddMonths(-3).ToString("yyyy-MM-dd")} 07:30:00";
                string _endDTE = $"{_mfgDate.ToString("yyyy-MM-dd")} 07:30:00";

                var _repairedEqList = _eqpInfoRepository.SelectRepairedEqList(_beginDTE, _endDTE);

                return _repairedEqList.GroupBy(g => g.AREA).Select(s => (s.Key, s.Select(ss => ss.EQUIP_NBR).ToList())).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public (int, int) GetTodayRepairedEqPendingList()
        {
            var _eqpInfoList = _eqpInfoRepository.SelectByConditions(DateTime.Now.ToString("yyyy-MM-dd"), null, false, false);

            return (_eqpInfoList.Where(w => string.IsNullOrEmpty(w.mnt_user)).ToList().Count(),
                    _eqpInfoList.Where(w => !string.IsNullOrEmpty(w.mnt_user) && string.IsNullOrEmpty(w.engineer)).ToList().Count());
        }

        public EquipmentEditEntity GetEditEqpinfo(int sn, UserEntity userEntity = null)
        {
            try
            {
                var _r = _eqpInfoRepository.SelectEqpinfoByConditions(sn).FirstOrDefault();

                if (_r == null)
                {
                    return null;
                }

                var _lcmProdList = _optionDomainService.GetLcmProdOptions().SelectMany(option => option.Item2).ToList();

                //if (userEntity != null)
                //    CatchHelper.Set($"Eq_Edit:{sn}", userEntity.Account, 600);

                var _evenCodeList = _optionDomainService.GetEqEvenCode(_r.typeId, _r.yId, _r.subYId, _r.xId, _r.subXId, _r.rId);

                return new EquipmentEditEntity
                {
                    sn = _r.sn,
                    Equipment = _r.Equipment,
                    ToolName = _r.tool_name,
                    Product = _r.prod_id,
                    ProductName = _lcmProdList.FirstOrDefault(f => f.Item1 == _r.prod_sn).Item2 ?? "",
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
                    MntMinutes = _r.mnt_minutes,
                    TypeId = _r.typeId,
                    TypeDesc = _r.typeId == 0 ? "" : _evenCodeList.FirstOrDefault(f => f.TypeId == _r.typeId)?.Type ?? "Other",
                    YId = _r.yId,
                    YDesc = _r.yId == 0 ? "" : _evenCodeList.FirstOrDefault(f => f.YId == _r.yId)?.Y ?? "Other",
                    SubYId = _r.subYId,
                    SubYDesc = _r.subYId == 0 ? "" : _evenCodeList.FirstOrDefault(f => f.SubYId == _r.subYId)?.SubY ?? "Other",
                    XId = _r.xId,
                    XDesc = _r.xId == 0 ? "" : _evenCodeList.FirstOrDefault(f => f.XId == _r.xId)?.X ?? "Other",
                    SubXId = _r.subXId,
                    SubXDesc = _r.subXId == 0 ? "" : _evenCodeList.FirstOrDefault(f => f.SubXId == _r.subXId)?.SubX ?? "Other",
                    RId = _r.rId,
                    RDesc = _r.rId == 0 ? "" : _evenCodeList.FirstOrDefault(f => f.RId == _r.rId)?.R ?? "Other",
                    StatusId = _r.statusId
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
                var _r = _eqpInfoRepository.SelectEqpinfoByConditions(sn).FirstOrDefault();

                if (_r == null)
                {
                    return null;
                }

                var _processOptions = _optionDomainService.GetEqProcessOptionByType(OptionTypeEnum.ProcessOption);
                var _eqUnitOptions = _optionDomainService.GetEqProcessOptionByType(OptionTypeEnum.EqUnit, _r.processId);
                var _eqUnitPartOptions = _optionDomainService.GetEqProcessOptionByType(OptionTypeEnum.EqUnitPart, _r.processId, _r.eq_unitId);
                var _shiftOptions = _optionDomainService.GetShiftOptionList();
                var _priorityOptions = _optionDomainService.GetPriorityOptionList();
                var _evenCodeList = _optionDomainService.GetEqEvenCode(_r.typeId, _r.yId, _r.subYId, _r.xId, _r.subXId, _r.rId);

                return new EquipmentEditEntity
                {
                    sn = _r.sn,
                    Equipment = _r.Equipment,
                    ToolName = _r.tool_name,
                    Code = _r.Code,
                    CodeDesc = _r.Code_Desc,
                    Product = _r.prod_id,
                    Operator = _r.Operator,
                    Comments = _r.Comments,
                    StartTime = _r.Start_Time,
                    UpdateTime = _r.Update_Time,
                    ShiftDesc = _shiftOptions.FirstOrDefault(w => w.Id == _r.shift)?.Value ?? "",
                    ProcessId = _r.processId,
                    Process = _processOptions.FirstOrDefault(w => w.Id == _r.processId)?.Value ?? "",
                    EqUnitId = _r.eq_unitId,
                    EqUnit = _eqUnitOptions.FirstOrDefault(w => w.Id == _r.eq_unitId)?.Value ?? "",
                    EqUnitPartId = _r.eq_unit_partId,
                    EqUnitPart = _eqUnitPartOptions.FirstOrDefault(w => w.Id == _r.eq_unit_partId)?.Value ?? "",
                    DefectQty = _r.defect_qty,
                    DefectRate = _r.defect_rate,
                    Engineer = _r.engineer,
                    Priority = _priorityOptions.FirstOrDefault(w => w.Id == _r.priority)?.Value ?? "",
                    Memo = _r.memo,
                    MntUser = _r.mnt_user,
                    MntMinutes = _r.mnt_minutes,
                    TypeDesc = _r.typeId == 0 ? "" : _evenCodeList.FirstOrDefault(f => f.TypeId == _r.typeId)?.Type ?? "Other",
                    YDesc = _r.yId == 0 ? "" : _evenCodeList.FirstOrDefault(f => f.YId == _r.yId)?.Y ?? "Other",
                    SubYDesc = _r.subYId == 0 ? "" : _evenCodeList.FirstOrDefault(f => f.SubYId == _r.subYId)?.SubY ?? "Other",
                    XDesc = _r.xId == 0 ? "" : _evenCodeList.FirstOrDefault(f => f.XId == _r.xId)?.X ?? "Other",
                    SubXDesc = _r.subXId == 0 ? "" : _evenCodeList.FirstOrDefault(f => f.SubXId == _r.subXId)?.SubX ?? "Other",
                    RDesc = _r.rId == 0 ? "" : _evenCodeList.FirstOrDefault(f => f.RId == _r.rId)?.R ?? "Other"
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string VerifyEqpStatus(int sn, EqIssueStatusEnum statusId, UserEntity userEntity)
        {
            try
            {
                //var _catchInfo = CatchHelper.Get($"Eq_Edit:{sn}");

                //if (_catchInfo != null && _catchInfo != userEntity.Account)
                //{
                //    return $"{_catchInfo} 編輯機況中";
                //}

                if (statusId == 0)
                    return "參數異常";

                var _eqpinfo = _eqpInfoRepository.SelectEqpinfoByConditions(sn).FirstOrDefault();

                if (statusId == EqIssueStatusEnum.PendingPM && !string.IsNullOrEmpty(_eqpinfo.mnt_user))
                    return "機況已更新, 請重整頁面";
                else if (statusId == EqIssueStatusEnum.PendingENG && !string.IsNullOrEmpty(_eqpinfo.engineer))
                    return "機況已更新, 請重整頁面";

                return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EquipmentEntity> GetEntityHistoryDetail(DateTime startTime, DateTime endTime, List<string> eqpListStr, List<int> prodSnList)
        {
            try
            {
                var _eqpHisList = _eqpInfoRepository.SelectEqpinfoByConditions(0, eqpListStr, startTime: startTime, endTime: endTime, prodSnList: prodSnList);

                return (_eqpHisList.Select(s =>
                {
                    return new EquipmentEntity
                    {
                        IEStatusDesc = s.status_desc_ie,
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

        /// <summary>
        /// 新增機況
        /// </summary>
        /// <param name="editEntity"></param>
        /// <param name="userEntity"></param>
        /// <returns></returns>
        public string Create(EquipmentEditEntity editEntity, UserEntity userEntity)
        {
            try
            {
                var _updateResponse = "";
                DateTime _updTime = DateTime.Now;

                List<EqpInfoDao> _insEqpinfoList = new List<EqpInfoDao>();
                string _pKey = Guid.NewGuid().ToString("N");

                var _prodDesc = (_optionDomainService.GetLcmProdOptions().SelectMany(s => s.Item2).FirstOrDefault(f => f.Item1 == editEntity.ProductId).Item2).Split("-")[0] ?? "";

                // 帶入 key in 機況
                EqpInfoDao _updEqpinfo = new EqpInfoDao
                {
                    Start_Time = editEntity.StartTime,
                    Repair_Time = editEntity.EndTime.Subtract(editEntity.StartTime).TotalMinutes.ToString(),
                    Equipment = editEntity.Equipment,
                    Code = editEntity.Code,
                    Code_Desc = editEntity.CodeDesc,
                    Comments = editEntity.Comments,
                    processId = editEntity.ProcessId,
                    eq_unitId = editEntity.EqUnitId,
                    eq_unit_partId = editEntity.EqUnitPartId,
                    shift = editEntity.Shift,
                    defect_qty = editEntity.DefectQty,
                    defect_rate = editEntity.DefectRate,
                    mnt_user = userEntity.Name,
                    mnt_minutes = editEntity.MntMinutes,
                    typeId = editEntity.TypeId,
                    yId = editEntity.YId,
                    subYId = editEntity.SubYId,
                    xId = editEntity.XId,
                    subXId = editEntity.SubXId,
                    rId = editEntity.RId,
                    memo = editEntity.Memo,
                    Update_Time = _updTime,
                    prod_id = _prodDesc,
                    prod_sn = editEntity.ProductId,
                    statusId = EqIssueStatusEnum.PendingENG,
                    Operator = userEntity.JobId,
                    isManual = 1,
                    status_desc_ie = editEntity.DownType,
                    P_key = _pKey
                };

                decimal _firstRepairTime = Convert.ToDecimal(_updEqpinfo.Repair_Time);

                // 依機況維修時間分配至分時
                // 起始時間在 0~30分
                if (_updEqpinfo.Start_Time.Minute < 30)
                {
                    var _culTime = _updEqpinfo.Start_Time.Minute + decimal.Parse(_updEqpinfo.Repair_Time);
                    decimal _culRecord = _culTime / 30;// 分時起始都是.30分

                    if (_culRecord >= 1)
                    {
                        _firstRepairTime = Convert.ToDecimal(new TimeSpan(new DateTime(_updEqpinfo.Start_Time.Year, _updEqpinfo.Start_Time.Month, _updEqpinfo.Start_Time.Day, _updEqpinfo.Start_Time.Hour, 30, 0).Ticks
                                - _updEqpinfo.Start_Time.Ticks).TotalMinutes);

                        for (int i = 1; i < _culRecord;)
                        {
                            DateTime _newStartTime = new DateTime(_updEqpinfo.Start_Time.Year, _updEqpinfo.Start_Time.Month, _updEqpinfo.Start_Time.Day, _updEqpinfo.Start_Time.Hour, 30, 0);

                            _insEqpinfoList.Add(new EqpInfoDao
                            {
                                Equipment = _updEqpinfo.Equipment,
                                Operator = _updEqpinfo.Operator,
                                Code = _updEqpinfo.Code,
                                Code_Desc = _updEqpinfo.Code_Desc,
                                Comments = _updEqpinfo.Comments,
                                Start_Time = _newStartTime.AddMinutes(30 * (i - 1)),
                                Repair_Time = Convert.ToDecimal(_updEqpinfo.Repair_Time) - _firstRepairTime - (30 * (i - 1)) > 60
                                    ? "60"
                                    : (Convert.ToDecimal(_updEqpinfo.Repair_Time) - _firstRepairTime - (30 * (i - 1))).ToString(),
                                prod_id = _updEqpinfo.prod_id,
                                statusId = EqIssueStatusEnum.PendingPM,
                                tool_name = _updEqpinfo.tool_name,
                                prod_sn = _updEqpinfo.prod_sn,
                                status_desc_ie = _updEqpinfo.status_desc_ie,
                                Update_Time = _updTime,
                                processId = editEntity.ProcessId,
                                eq_unitId = editEntity.EqUnitId,
                                eq_unit_partId = editEntity.EqUnitPartId,
                                shift = editEntity.Shift,
                                defect_qty = editEntity.DefectQty,
                                defect_rate = editEntity.DefectRate,
                                mnt_user = userEntity.Name,
                                mnt_minutes = editEntity.MntMinutes,
                                typeId = editEntity.TypeId,
                                yId = editEntity.YId,
                                subYId = editEntity.SubYId,
                                xId = editEntity.XId,
                                subXId = editEntity.SubXId,
                                rId = editEntity.RId,
                                memo = editEntity.Memo,
                                P_key = _pKey
                            });

                            i += 2;
                        }

                        _updEqpinfo.Repair_Time = _firstRepairTime.ToString();
                    }
                }
                // 起始時間在 30~60分
                else
                {
                    decimal _culRecord = decimal.Parse(_updEqpinfo.Repair_Time) / 60;

                    if (_culRecord >= 1)
                    {
                        _firstRepairTime = Convert.ToDecimal(new TimeSpan(new DateTime(_updEqpinfo.Start_Time.Year, _updEqpinfo.Start_Time.Month, _updEqpinfo.Start_Time.Day, _updEqpinfo.Start_Time.Hour + 1, 30, 0).Ticks
                                - _updEqpinfo.Start_Time.Ticks).TotalMinutes);

                        for (int i = 0; i < _culRecord - 1; i++)
                        {
                            DateTime _newStartTime = new DateTime(_updEqpinfo.Start_Time.Year, _updEqpinfo.Start_Time.Month, _updEqpinfo.Start_Time.Day, _updEqpinfo.Start_Time.Hour, 30, 0);

                            _insEqpinfoList.Add(new EqpInfoDao
                            {
                                Equipment = _updEqpinfo.Equipment,
                                Operator = _updEqpinfo.Operator,
                                Code = _updEqpinfo.Code,
                                Code_Desc = _updEqpinfo.Code_Desc,
                                Comments = _updEqpinfo.Comments,
                                Start_Time = _newStartTime.AddHours(i + 1),
                                Repair_Time = Convert.ToDecimal(_updEqpinfo.Repair_Time) - _firstRepairTime - (60 * i) > 60
                                    ? "60"
                                    : (Convert.ToDecimal(_updEqpinfo.Repair_Time) - _firstRepairTime - (60 * i)).ToString(),
                                prod_id = _updEqpinfo.prod_id,
                                statusId = EqIssueStatusEnum.PendingPM,
                                tool_name = _updEqpinfo.tool_name,
                                prod_sn = _updEqpinfo.prod_sn,
                                status_desc_ie = _updEqpinfo.status_desc_ie,
                                Update_Time = _updTime,
                                processId = editEntity.ProcessId,
                                eq_unitId = editEntity.EqUnitId,
                                eq_unit_partId = editEntity.EqUnitPartId,
                                shift = editEntity.Shift,
                                defect_qty = editEntity.DefectQty,
                                defect_rate = editEntity.DefectRate,
                                mnt_user = userEntity.Name,
                                mnt_minutes = editEntity.MntMinutes,
                                typeId = editEntity.TypeId,
                                yId = editEntity.YId,
                                subYId = editEntity.SubYId,
                                xId = editEntity.XId,
                                subXId = editEntity.SubXId,
                                rId = editEntity.RId,
                                memo = editEntity.Memo,
                                P_key = _pKey
                            });
                        }
                    }

                }

                _updEqpinfo.Repair_Time = _firstRepairTime.ToString();
                _insEqpinfoList.Add(_updEqpinfo);


                using (var scope = new TransactionScope())
                {
                    var _uprdResult = false;

                    _uprdResult = _eqpInfoRepository.Insert(_insEqpinfoList) == _insEqpinfoList.Count;

                    if (_uprdResult)
                    {
                        scope.Complete();
                    }
                    else
                        _updateResponse = "新增失敗";
                }

                return _updateResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string UpdateEqpinfo(EquipmentEditEntity editEntity, UserEntity userEntity)
        {
            try
            {
                var _updateResponse = "";
                var oldEqpinfo = _eqpInfoRepository.SelectEqpinfoByConditions(editEntity.sn).FirstOrDefault();

                if (oldEqpinfo == null)
                    return "查無機況";

                if (editEntity.StatusId == 0 ||
                    (editEntity.StatusId == EqIssueStatusEnum.PendingPM && !string.IsNullOrEmpty(oldEqpinfo.mnt_user)) ||
                    (editEntity.StatusId == EqIssueStatusEnum.PendingENG && !string.IsNullOrEmpty(oldEqpinfo.engineer)) ||
                    editEntity.StatusId == EqIssueStatusEnum.Complete)
                    return "機況內容已變更, 無法覆寫";

                EqpInfoDao _updEqpinfo = new EqpInfoDao
                {
                    sn = editEntity.sn
                };

                if (editEntity.StatusId == EqIssueStatusEnum.PendingPM)
                {
                    _updEqpinfo.Comments = editEntity.Comments;
                    _updEqpinfo.processId = editEntity.ProcessId;
                    _updEqpinfo.eq_unitId = editEntity.EqUnitId;
                    _updEqpinfo.eq_unit_partId = editEntity.EqUnitPartId;
                    _updEqpinfo.shift = editEntity.Shift;
                    _updEqpinfo.defect_qty = editEntity.DefectQty;
                    _updEqpinfo.defect_rate = editEntity.DefectRate;
                    _updEqpinfo.mnt_user = userEntity.Name;
                    _updEqpinfo.mnt_minutes = editEntity.MntMinutes;
                    _updEqpinfo.typeId = editEntity.TypeId;
                    _updEqpinfo.yId = editEntity.YId;
                    _updEqpinfo.subYId = editEntity.SubYId;
                    _updEqpinfo.xId = editEntity.XId;
                    _updEqpinfo.subXId = editEntity.SubXId;
                    _updEqpinfo.rId = editEntity.RId;
                    _updEqpinfo.statusId = EqIssueStatusEnum.PendingENG;
                }
                else if (editEntity.StatusId == EqIssueStatusEnum.PendingENG)
                {
                    _updEqpinfo.xId = editEntity.XId;
                    _updEqpinfo.subXId = editEntity.SubXId;
                    _updEqpinfo.rId = editEntity.RId;
                    _updEqpinfo.priority = editEntity.PriorityId;
                    _updEqpinfo.engineer = userEntity.Name;
                    _updEqpinfo.memo = editEntity.Memo;
                    _updEqpinfo.statusId = EqIssueStatusEnum.Complete;
                }

                using (var scope = new TransactionScope())
                {
                    var _uprdResult = false;

                    if (editEntity.StatusId == EqIssueStatusEnum.PendingPM)
                        _uprdResult = _eqpInfoRepository.UpdateEqpinfoByPM(_updEqpinfo) == 2; //含觸發自動更新 updateTime trigger
                    else if (editEntity.StatusId == EqIssueStatusEnum.PendingENG)
                        _uprdResult = _eqpInfoRepository.UpdateEqpinfoByENG(_updEqpinfo) == 2; //含觸發自動更新 updateTime trigger

                    if (_uprdResult)
                    {
                        //CatchHelper.Delete(new string[] { $"Eq_Edit:{editEntity.sn}" });
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
