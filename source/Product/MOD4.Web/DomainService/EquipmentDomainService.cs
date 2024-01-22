using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using MOD4.Web.Repostory;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Transactions;

namespace MOD4.Web.DomainService
{
    public class EquipmentDomainService : IEquipmentDomainService
    {
        private readonly IAlarmXmlRepository _alarmXmlRepository;
        private readonly IEqpInfoRepository _eqpInfoRepository;
        private readonly IOptionDomainService _optionDomainService;
        private readonly IEquipMappingRepository _equipMappingRepository;
        private readonly ILcmProductRepository _lcmProductRepository;

        public EquipmentDomainService(IAlarmXmlRepository alarmXmlRepository,
            IEqpInfoRepository eqpInfoRepository,
            IOptionDomainService optionDomainService,
            IEquipMappingRepository equipMappingRepository,
            ILcmProductRepository lcmProductRepository)
        {
            _alarmXmlRepository = alarmXmlRepository;
            _eqpInfoRepository = eqpInfoRepository;
            _optionDomainService = optionDomainService;
            _equipMappingRepository = equipMappingRepository;
            _lcmProductRepository = lcmProductRepository;
        }

        public List<(string, List<string>)> GetUnRepaireEqOptions()
        {
            try
            {
                DateTime _mfgDate = DateTime.Now;

                string _beginDTE = $"{_mfgDate.AddMonths(-6).ToString("yyyy-MM-dd")} 07:30:00";
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

                var _alarmList = _alarmXmlRepository.SelectByConditions(date ?? beginDTE, toolId?.Split(",").ToList() ?? null, 0, false, null, false);

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
                DateTime _mfgDay = DateTime.Now.AddMinutes(-450);

                List<EqpInfoDao> _resEquipmentList = new List<EqpInfoDao>();

                var _alarmRepairedList = _alarmXmlRepository.SelectByConditions(string.IsNullOrEmpty(date) ? _mfgDay.ToString("yyyy-MM-dd") : date,
                    string.IsNullOrEmpty(toolId) ? null : toolId.Split(",").ToList() ?? null,
                    0,
                    true,
                    string.IsNullOrEmpty(statusIdList) ? null : statusIdList.Split(",").ToList() ?? null,
                    showAuto);

                var _eqpMapList = _equipMappingRepository.SelectEqByEqIdList(_alarmRepairedList.Select(s => s.tool_id).ToList());

                var _response = (from alarm in _alarmRepairedList
                                 join map in _eqpMapList
                                 on alarm.tool_id equals map.EQUIP_NBR into t
                                 from map in t.DefaultIfEmpty()
                                 select new EquipmentEntity
                                 {
                                     sn = alarm.sn,
                                     ToolId = alarm.tool_id,
                                     ToolStatus = alarm.tool_status,
                                     StatusCdsc = alarm.status_cdsc,
                                     UserId = alarm.user_id,
                                     Comment = alarm.comment,
                                     LmTime = alarm.XML_time,
                                     RepairTime = ((DateTime)alarm.end_time).Subtract(alarm.XML_time).TotalMinutes.ToString("0.00"),
                                     MFGDay = alarm.MFG_Day.ToString("yyyy-MM-dd"),
                                     MFGHr = alarm.MFG_HR.ToString(),
                                     StatusId = alarm.statusId
                                 }).ToList();

                return _response;

                //return (_resEquipmentList.OrderByDescending(o => o.Start_Time).Select(s =>
                //{
                //    return new EquipmentEntity
                //    {
                //        sn = s.sn,
                //        ToolId = s.Equipment,
                //        ToolName = s.tool_name,
                //        ToolStatus = s.Code,
                //        StatusCdsc = s.Code_Desc,
                //        UserId = s.Operator,
                //        Comment = s.Comments,
                //        LmTime = s.Start_Time,
                //        RepairTime = s.Repair_Time,
                //        MFGDay = s.MFG_Day.ToString("yyyy-MM-dd"),
                //        MFGHr = s.MFG_HR.ToString(),
                //        StatusId = s.statusId
                //    };
                //})).ToList();
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
                string _endDTE = $"{_mfgDate.AddDays(1).ToString("yyyy-MM-dd")} 00:00:00";

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
                //var _r = _eqpInfoRepository.SelectEqpinfoByConditions(sn).FirstOrDefault();
                var _alarmDetail = _alarmXmlRepository.SelectByConditions(null, null, sn, true, null, false).FirstOrDefault();

                if (_alarmDetail == null)
                {
                    return null;
                }

                //if (userEntity != null)
                //    CatchHelper.Set($"Eq_Edit:{sn}", userEntity.Account, 600);

                var _evenCodeList = _optionDomainService.GetEqEvenCode(_alarmDetail.typeId, _alarmDetail.yId, _alarmDetail.subYId, _alarmDetail.xId, _alarmDetail.subXId, _alarmDetail.rId);

                return new EquipmentEditEntity
                {
                    sn = _alarmDetail.sn,
                    Equipment = _alarmDetail.tool_id,
                    ToolName = "",
                    Product = _alarmDetail.prod_id,
                    //ProductName = _lcmProdList.FirstOrDefault(f => f.Item1 == _alarmDetail.prod_sn).Item2 ?? "",
                    Code = _alarmDetail.tool_status,
                    CodeDesc = _alarmDetail.status_cdsc,
                    Operator = _alarmDetail.user_id,
                    Comments = _alarmDetail.comment,
                    StartTime = _alarmDetail.XML_time.ToString("yyyy/MM/dd HH:mm"),
                    UpdateTime = (DateTime)_alarmDetail.end_time,
                    RepairedTime = Math.Round(Convert.ToDecimal(((DateTime)_alarmDetail.end_time).Subtract(_alarmDetail.XML_time).TotalMinutes), 3),
                    Shift = _alarmDetail.shift,
                    ProcessId = _alarmDetail.processId,
                    EqUnitId = _alarmDetail.eq_unitId,
                    EqUnitPartId = _alarmDetail.eq_unit_partId,
                    DefectQty = _alarmDetail.defect_qty,
                    DefectRate = _alarmDetail.defect_rate,
                    Engineer = _alarmDetail.engineer,
                    Memo = _alarmDetail.memo,
                    MntUser = _alarmDetail.mnt_user,
                    MntMinutes = _alarmDetail.mnt_minutes,
                    TypeId = _alarmDetail.typeId,
                    TypeDesc = _alarmDetail.typeId == 0 ? "" : _evenCodeList.FirstOrDefault(f => f.TypeId == _alarmDetail.typeId)?.Type ?? "Other",
                    YId = _alarmDetail.yId,
                    YDesc = _alarmDetail.yId == 0 ? "" : _evenCodeList.FirstOrDefault(f => f.YId == _alarmDetail.yId)?.Y ?? "Other",
                    SubYId = _alarmDetail.subYId,
                    SubYDesc = _alarmDetail.subYId == 0 ? "" : _evenCodeList.FirstOrDefault(f => f.SubYId == _alarmDetail.subYId)?.SubY ?? "Other",
                    XId = _alarmDetail.xId,
                    XDesc = _alarmDetail.xId == 0 ? "" : _evenCodeList.FirstOrDefault(f => f.XId == _alarmDetail.xId)?.X ?? "Other",
                    SubXId = _alarmDetail.subXId,
                    SubXDesc = _alarmDetail.subXId == 0 ? "" : _evenCodeList.FirstOrDefault(f => f.SubXId == _alarmDetail.subXId)?.SubX ?? "Other",
                    RId = _alarmDetail.rId,
                    RDesc = _alarmDetail.rId == 0 ? "" : _evenCodeList.FirstOrDefault(f => f.RId == _alarmDetail.rId)?.R ?? "Other",
                    StatusId = _alarmDetail.statusId,
                    ENGTypeId = _alarmDetail.engTypeId,
                    ENGTypeDesc = _alarmDetail.engTypeId == 0 ? "" : _evenCodeList.FirstOrDefault(f => f.TypeId == _alarmDetail.engTypeId)?.Type ?? "Other",
                    ENGYId = _alarmDetail.engYId,
                    ENGYDesc = _alarmDetail.engYId == 0 ? "" : _evenCodeList.FirstOrDefault(f => f.YId == _alarmDetail.engYId)?.Y ?? "Other",
                    ENGSubYId = _alarmDetail.engSubYId,
                    ENGSubYDesc = _alarmDetail.engSubYId == 0 ? "" : _evenCodeList.FirstOrDefault(f => f.SubYId == _alarmDetail.engSubYId)?.SubY ?? "Other",
                    ENGXId = _alarmDetail.engXId,
                    ENGXDesc = _alarmDetail.engXId == 0 ? "" : _evenCodeList.FirstOrDefault(f => f.XId == _alarmDetail.engXId)?.X ?? "Other",
                    ENGSubXId = _alarmDetail.engSubXId,
                    ENGSubXDesc = _alarmDetail.engSubXId == 0 ? "" : _evenCodeList.FirstOrDefault(f => f.SubXId == _alarmDetail.engSubXId)?.SubX ?? "Other",
                    ENGRId = _alarmDetail.engRId,
                    ENGRDesc = _alarmDetail.engRId == 0 ? "" : _evenCodeList.FirstOrDefault(f => f.RId == _alarmDetail.engRId)?.R ?? "Other",
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
                var _alarmEqData = _alarmXmlRepository.SelectByConditions(null, null, sn, true, null, false).FirstOrDefault();

                if (_alarmEqData == null)
                {
                    return null;
                }

                var _processOptions = _optionDomainService.GetEqProcessOptionByType(OptionTypeEnum.ProcessOption);
                var _eqUnitOptions = _optionDomainService.GetEqProcessOptionByType(OptionTypeEnum.EqUnit, _alarmEqData.processId);
                var _eqUnitPartOptions = _optionDomainService.GetEqProcessOptionByType(OptionTypeEnum.EqUnitPart, _alarmEqData.processId, _alarmEqData.eq_unitId);
                var _shiftOptions = _optionDomainService.GetShiftOptionList();
                var _priorityOptions = _optionDomainService.GetPriorityOptionList();
                var _evenCodeList = _optionDomainService.GetEqEvenCode(_alarmEqData.typeId, _alarmEqData.yId, _alarmEqData.subYId, _alarmEqData.xId, _alarmEqData.subXId, _alarmEqData.rId);

                return new EquipmentEditEntity
                {
                    sn = _alarmEqData.sn,
                    Equipment = _alarmEqData.tool_id,
                    //ToolName = _alarmEqData.tool_name,
                    Code = _alarmEqData.tool_status,
                    CodeDesc = _alarmEqData.status_cdsc,
                    Product = _alarmEqData.prod_id,
                    Operator = _alarmEqData.user_id,
                    Comments = _alarmEqData.comment,
                    StartTime = _alarmEqData.XML_time.ToString("yyyy/MM/dd HH:mm"),
                    EndTime = ((DateTime)_alarmEqData.end_time).ToString("yyyy/MM/dd HH:mm"),
                    ShiftDesc = _shiftOptions.FirstOrDefault(w => w.Id == _alarmEqData.shift)?.Value ?? "",
                    ProcessId = _alarmEqData.processId,
                    Process = _processOptions.FirstOrDefault(w => w.Id == _alarmEqData.processId)?.Value ?? "",
                    EqUnitId = _alarmEqData.eq_unitId,
                    EqUnit = _eqUnitOptions.FirstOrDefault(w => w.Id == _alarmEqData.eq_unitId)?.Value ?? "",
                    EqUnitPartId = _alarmEqData.eq_unit_partId,
                    EqUnitPart = _eqUnitPartOptions.FirstOrDefault(w => w.Id == _alarmEqData.eq_unit_partId)?.Value ?? "",
                    DefectQty = _alarmEqData.defect_qty,
                    DefectRate = _alarmEqData.defect_rate,
                    Engineer = _alarmEqData.engineer,
                    Priority = _priorityOptions.FirstOrDefault(w => w.Id == _alarmEqData.priority)?.Value ?? "",
                    Memo = _alarmEqData.memo,
                    MntUser = _alarmEqData.mnt_user,
                    MntMinutes = _alarmEqData.mnt_minutes,
                    TypeDesc = _alarmEqData.typeId == 0 ? "" : _evenCodeList.FirstOrDefault(f => f.TypeId == _alarmEqData.typeId)?.Type ?? "Other",
                    YDesc = _alarmEqData.yId == 0 ? "" : _evenCodeList.FirstOrDefault(f => f.YId == _alarmEqData.yId)?.Y ?? "Other",
                    SubYDesc = _alarmEqData.subYId == 0 ? "" : _evenCodeList.FirstOrDefault(f => f.SubYId == _alarmEqData.subYId)?.SubY ?? "Other",
                    XDesc = _alarmEqData.xId == 0 ? "" : _evenCodeList.FirstOrDefault(f => f.XId == _alarmEqData.xId)?.X ?? "Other",
                    SubXDesc = _alarmEqData.subXId == 0 ? "" : _evenCodeList.FirstOrDefault(f => f.SubXId == _alarmEqData.subXId)?.SubX ?? "Other",
                    RDesc = _alarmEqData.rId == 0 ? "" : _evenCodeList.FirstOrDefault(f => f.RId == _alarmEqData.rId)?.R ?? "Other",
                    ENGTypeDesc = _alarmEqData.engTypeId == 0 ? "" : _evenCodeList.FirstOrDefault(f => f.TypeId == _alarmEqData.engTypeId)?.Type ?? "Other",
                    ENGYDesc = _alarmEqData.engYId == 0 ? "" : _evenCodeList.FirstOrDefault(f => f.YId == _alarmEqData.engYId)?.Y ?? "Other",
                    ENGSubYDesc = _alarmEqData.engSubYId == 0 ? "" : _evenCodeList.FirstOrDefault(f => f.SubYId == _alarmEqData.engSubYId)?.SubY ?? "Other",
                    ENGXDesc = _alarmEqData.engXId == 0 ? "" : _evenCodeList.FirstOrDefault(f => f.XId == _alarmEqData.engXId)?.X ?? "Other",
                    ENGSubXDesc = _alarmEqData.engSubXId == 0 ? "" : _evenCodeList.FirstOrDefault(f => f.SubXId == _alarmEqData.engSubXId)?.SubX ?? "Other",
                    ENGRDesc = _alarmEqData.engRId == 0 ? "" : _evenCodeList.FirstOrDefault(f => f.RId == _alarmEqData.engRId)?.R ?? "Other"
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

                var _alarmDetail = _alarmXmlRepository.SelectByConditions(null, null, sn, true, null, false).FirstOrDefault();

                if (statusId == EqIssueStatusEnum.PendingPM && !string.IsNullOrEmpty(_alarmDetail.mnt_user))
                    return "機況已更新, 請重整頁面";
                else if (statusId == EqIssueStatusEnum.PendingENG && !string.IsNullOrEmpty(_alarmDetail.engineer))
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

                if (!DateTime.TryParseExact(editEntity.StartTime, "yyyy/MM/dd HH:mm", null, DateTimeStyles.None, out _) ||
                    !DateTime.TryParseExact(editEntity.EndTime, "yyyy/MM/dd HH:mm", null, DateTimeStyles.None, out _))
                    throw new Exception("起訖時間格式錯誤");

                AlarmXmlDao alarmXmlDao = new AlarmXmlDao
                {
                    XML_time = DateTime.Parse(editEntity.StartTime),
                    end_time = DateTime.Parse(editEntity.EndTime),
                };

                var _prodDesc = (_optionDomainService.GetLcmProdOptions().SelectMany(s => s.Item2).FirstOrDefault(f => f.Item1 == editEntity.ProductId).Item2).Split("-")[0] ?? "";

                alarmXmlDao.tool_id = editEntity.Equipment;
                alarmXmlDao.tool_status = editEntity.Code;
                alarmXmlDao.status_cdsc = editEntity.CodeDesc;
                alarmXmlDao.MFG_Day = alarmXmlDao.XML_time.AddMinutes(-450).Date;
                alarmXmlDao.MFG_HR = alarmXmlDao.XML_time.AddMinutes(-450).Hour;
                alarmXmlDao.comment = editEntity.Comments;
                alarmXmlDao.processId = editEntity.ProcessId;
                alarmXmlDao.eq_unitId = editEntity.EqUnitId;
                alarmXmlDao.eq_unit_partId = editEntity.EqUnitPartId;
                alarmXmlDao.shift = editEntity.Shift;
                alarmXmlDao.defect_qty = editEntity.DefectQty;
                alarmXmlDao.defect_rate = editEntity.DefectRate;
                alarmXmlDao.mnt_user = userEntity.Name;
                alarmXmlDao.mnt_minutes = editEntity.MntMinutes;
                alarmXmlDao.typeId = editEntity.TypeId;
                alarmXmlDao.yId = editEntity.YId;
                alarmXmlDao.subYId = editEntity.SubYId;
                alarmXmlDao.xId = editEntity.XId;
                alarmXmlDao.subXId = editEntity.SubXId;
                alarmXmlDao.rId = editEntity.RId;
                alarmXmlDao.lm_time = _updTime;
                alarmXmlDao.prod_id = _prodDesc;
                alarmXmlDao.prod_sn = editEntity.ProductId;
                alarmXmlDao.statusId = EqIssueStatusEnum.PendingENG;
                alarmXmlDao.user_id = userEntity.JobId;
                alarmXmlDao.isManual = 1;

                // 帶入 key in 機況
                EqpInfoDao _updEqpinfo = new EqpInfoDao
                {
                    Start_Time = alarmXmlDao.XML_time,
                    Repair_Time = ((DateTime)alarmXmlDao.end_time).Subtract(alarmXmlDao.XML_time).TotalMinutes.ToString(),
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
                    _uprdResult = _alarmXmlRepository.InsertAlarmXml(alarmXmlDao) == 1;

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
                var _oldAlarmEqinfo = _alarmXmlRepository.SelectByConditions(null, null, editEntity.sn, true, null, false).FirstOrDefault();

                if (_oldAlarmEqinfo == null)
                    return "查無機況";

                if (editEntity.StatusId == 0 ||
                    (editEntity.StatusId == EqIssueStatusEnum.PendingPM && !string.IsNullOrEmpty(_oldAlarmEqinfo.mnt_user)) ||
                    (editEntity.StatusId == EqIssueStatusEnum.PendingENG && !string.IsNullOrEmpty(_oldAlarmEqinfo.engineer)) ||
                    editEntity.StatusId == EqIssueStatusEnum.Complete)
                    return "機況內容已變更, 無法覆寫";

                AlarmXmlDao _alarmXmlDao = new AlarmXmlDao
                {
                    sn = editEntity.sn
                };

                if (editEntity.StatusId == EqIssueStatusEnum.PendingPM)
                {
                    _alarmXmlDao.comment = editEntity.Comments;
                    _alarmXmlDao.processId = editEntity.ProcessId;
                    _alarmXmlDao.eq_unitId = editEntity.EqUnitId;
                    _alarmXmlDao.eq_unit_partId = editEntity.EqUnitPartId;
                    _alarmXmlDao.shift = editEntity.Shift;
                    _alarmXmlDao.defect_qty = editEntity.DefectQty;
                    _alarmXmlDao.defect_rate = editEntity.DefectRate;
                    _alarmXmlDao.mnt_user = userEntity.Name;
                    _alarmXmlDao.mnt_minutes = editEntity.MntMinutes;
                    _alarmXmlDao.typeId = editEntity.TypeId;
                    _alarmXmlDao.yId = editEntity.YId;
                    _alarmXmlDao.subYId = editEntity.SubYId;
                    _alarmXmlDao.xId = editEntity.XId;
                    _alarmXmlDao.subXId = editEntity.SubXId;
                    _alarmXmlDao.rId = editEntity.RId;
                    _alarmXmlDao.statusId = EqIssueStatusEnum.PendingENG;
                }
                else if (editEntity.StatusId == EqIssueStatusEnum.PendingENG)
                {
                    _alarmXmlDao.engTypeId = editEntity.ENGTypeId;
                    _alarmXmlDao.engYId = editEntity.ENGYId;
                    _alarmXmlDao.engSubYId = editEntity.ENGSubYId;
                    _alarmXmlDao.engXId = editEntity.ENGXId;
                    _alarmXmlDao.engSubXId = editEntity.ENGSubXId;
                    _alarmXmlDao.engRId = editEntity.ENGRId;
                    _alarmXmlDao.priority = editEntity.PriorityId;
                    _alarmXmlDao.engineer = userEntity.Name;
                    _alarmXmlDao.memo = editEntity.Memo;
                    _alarmXmlDao.statusId = EqIssueStatusEnum.Complete;
                }

                using (var scope = new TransactionScope())
                {
                    var _uprdResult = false;

                    if (_oldAlarmEqinfo.statusId == EqIssueStatusEnum.PendingPM)
                        _uprdResult = _alarmXmlRepository.UpdateAlarmInfo(_alarmXmlDao) == 1; //含觸發自動更新 updateTime trigger
                    else if (_oldAlarmEqinfo.statusId == EqIssueStatusEnum.PendingENG)
                        _uprdResult = _alarmXmlRepository.UpdateAlarmInfoByENG(_alarmXmlDao) == 1; //含觸發自動更新 updateTime trigger

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
