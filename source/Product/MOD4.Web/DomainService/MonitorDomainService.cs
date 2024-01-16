using MathNet.Numerics.Statistics;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using MOD4.Web.Repostory;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Utility.Helper;

namespace MOD4.Web.DomainService
{
    public class MonitorDomainService : BaseDomainService, IMonitorDomainService
    {
        private readonly IAlarmXmlRepository _alarmXmlRepository;
        private readonly IMonitorSettingRepository _monitorSettingRepository;
        private readonly ITargetSettingRepository _targetSettingRepository;
        private readonly IMTDDashboardDomainService _mtdDashboardDomainService;
        private readonly IProdPerformanceDetailRepository _prodPerformanceDetailRepository;
        private readonly ILcmProductRepository _lcmProductRepository;

        public MonitorDomainService(IAlarmXmlRepository alarmXmlRepository,
            IMonitorSettingRepository monitorSettingRepository,
            ITargetSettingRepository targetSettingRepository,
            IMTDDashboardDomainService mtdDashboardDomainService,
            IProdPerformanceDetailRepository prodPerformanceDetailRepository,
            ILcmProductRepository lcmProductRepository)
        {
            _alarmXmlRepository = alarmXmlRepository;
            _monitorSettingRepository = monitorSettingRepository;
            _targetSettingRepository = targetSettingRepository;
            _mtdDashboardDomainService = mtdDashboardDomainService;
            _prodPerformanceDetailRepository = prodPerformanceDetailRepository;
            _lcmProductRepository = lcmProductRepository;
        }

        public MonitorEntity GetMapPerAlarmData()
        {
            try
            {
                return new MonitorEntity
                {
                    AlarmDayTop = GetAlarmTopDaily(),
                    ProdPerformanceList = GetProdPerformanceInfo(),
                    DailyMTD = GetMTDDailyInfoAsync().Result
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<MonitorProdPerInfoEntity> GetProdPerformanceInfo()
        {
            DateTime _mfgDate = DateTime.Now.AddMinutes(-450);

            var _mapAreaSettingList = _monitorSettingRepository.SelectSettings();
            var _alarmEqList = _alarmXmlRepository.SelectUnrepaired();
            var _prodPerInfo = _alarmXmlRepository.SelectProdInfo(_mfgDate.ToString("yyyy-MM-dd")).GroupBy(g => new { g.tool_id, g.area })
                .Select(s => new
                {
                    s.Key.tool_id,
                    s.Key.area,
                    prod_id = s.OrderByDescending(o => o.XML_time).FirstOrDefault().prod_id,
                    prodStr = string.Join("﹑\r\n", s.Select(x => x.prod_id)),
                    move_cnt = ProcessProdMoveCnt(s.Select(detail => detail).ToList())
                });

            return (from area in _mapAreaSettingList
                    join per in _prodPerInfo
                    on area.EqNumber equals per.tool_id
                    join alarm in _alarmEqList
                    on per.tool_id equals alarm.tool_id into tmpAlarm
                    from alarm in tmpAlarm.DefaultIfEmpty()
                    select new MonitorProdPerInfoEntity
                    {
                        Node = area.Node,
                        EqNumber = area.EqNumber,
                        DefTopRate = area.DefTopRate,
                        DefLeftRate = area.DefLeftRate,
                        DefWidth = area.DefWidth,
                        DefHeight = area.DefHeight,
                        Border = area.Border,
                        Background = ConvertColorHEXtoRGB(area.Background),
                        Area = per.area,
                        ProdNoConcate = per.prodStr,
                        ProdNo = per.prod_id,
                        PassQty = per.move_cnt,
                        StatusCode = alarm?.tool_status ?? "",
                        Comment = $"{alarm?.comment ?? ""}{alarm?.status_cdsc ?? ""}",
                        IsFrontEnd = "BONDING,LAM-FOG".Contains(alarm?.area ?? ""),
                        StartTime = alarm?.lm_time.ToString("yyyy/MM/dd HH:mm:ss") ?? ""
                    }).ToList();
        }

        public List<MonitorAlarmTopEntity> GetAlarmTopDaily()
        {
            DateTime _mfgDate = DateTime.Now.AddMinutes(-450);

            var _alarmDayTop = _alarmXmlRepository.SelectDayTopRepaired(_mfgDate.ToString("yyyy-MM-dd"));

            return _alarmDayTop.Select(alarm => new MonitorAlarmTopEntity
            {
                EqNumber = alarm.tool_id,
                StatusCode = alarm.tool_status,
                Comment = string.IsNullOrEmpty(alarm.comment.Trim()) ? alarm.status_cdsc : alarm.comment,
                ProdNo = alarm.prod_id,
                RepairedTime = $"{alarm.repairedTime}(min.)"
            }).ToList();
        }

        public async Task<List<MTDProcessDailyEntity>> GetMTDDailyInfoAsync()
        {
            try
            {
                var _dailyMTD = await _mtdDashboardDomainService.GetMonitorDailyMTDAsync();

                return _dailyMTD.GroupBy(g => new { g.Sn, g.Process }).Select(mtd => new MTDProcessDailyEntity
                {
                    Sn = mtd.Key.Sn,
                    Process = mtd.Key.Process,
                    DayPlanQty = mtd.Sum(s => s.DayPlanQty),
                    DayActQty = mtd.Sum(s => s.DayActQty),
                }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public MonitorSettingMainEntity GetMonitorMainList(int prodSn = 1206)
        {
            try
            {
                return new MonitorSettingMainEntity
                {
                    SettingDetails = GetMonitorAreaSettingList(),
                    ProdTTDetails = GetMonitorProdTTList(prodSn)
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<MonitorProdTTEntity> GetMonitorProdTTList(int prodSn)
        {
            try
            {
                var _targetSettings = _targetSettingRepository.SelectForMonitor(prodSn);

                return _targetSettings.Select(ts => new MonitorProdTTEntity
                {
                    Node = ts.Node,
                    DownEquipment = ts.DownEquipment,
                    LcmProdSn = ts.lcmProdSn,
                    TimeTarget = ts.TimeTarget,
                    ProdDesc = ts.ProdDesc
                }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<MonitorSettingEntity> GetMonitorAreaSettingList()
        {
            try
            {
                var _monitorSettings = _monitorSettingRepository.SelectSettings();

                return _monitorSettings.Select(ms => new MonitorSettingEntity
                {
                    Node = ms.Node,
                    EqNumber = ms.EqNumber,
                    DefTopRate = ms.DefTopRate * 100,
                    DefLeftRate = ms.DefLeftRate * 100,
                    DefWidth = ms.DefWidth,
                    DefHeight = ms.DefHeight,
                    LocX0 = ms.LocX0,
                    LocY0 = ms.LocY0,
                    LocX1 = ms.LocX1,
                    LocY1 = ms.LocY1,
                    Background = ms.Background,
                    Border = ms.Border
                }).OrderBy(ob => Convert.ToInt32(ob.EqNumber.Substring(ob.EqNumber.Length - 4, 4))).ThenBy(tb => tb.EqNumber).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string UpdateProdTT(List<MonitorProdTTEntity> prodTTEntity, UserEntity userEntity)
        {
            try
            {
                string _updRes = "";
                DateTime _nowTime = DateTime.Now;

                List<TargetSettingDao> _updDaoList = prodTTEntity.Select(setting => new TargetSettingDao
                {
                    Node = setting.Node,
                    DownEquipment = setting.DownEquipment ?? "",
                    lcmProdSn = setting.LcmProdSn,
                    TimeTarget = setting.TimeTarget,
                    UpdateUser = userEntity.Name,
                    UpdateTime = _nowTime
                }).ToList();

                using (TransactionScope scope = new TransactionScope())
                {
                    if (_targetSettingRepository.UpdateTT(_updDaoList) == _updDaoList.Count)
                        scope.Complete();
                    else
                        _updRes = "更新失敗";
                }

                return _updRes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string UpdateInsertMapArea(List<MonitorSettingEntity> mapAreaEntity, UserEntity userEntity)
        {
            try
            {
                string _updRes = "";
                DateTime _nowTime = DateTime.Now;

                List<MonitorSettingDao> _insMonitorSettings = new List<MonitorSettingDao>();

                mapAreaEntity.ForEach(set =>
                {
                    MonitorSettingDao _tmp = new MonitorSettingDao();

                    _tmp.Node = set.Node;
                    _tmp.EqNumber = set.EqNumber;
                    _tmp.LocX0 = set.LocX0;
                    _tmp.LocY0 = set.LocY0;
                    _tmp.LocX1 = set.LocX1;
                    _tmp.LocY1 = set.LocY1;
                    _tmp.Background = set.Background;
                    _tmp.Border = set.Border;
                    _tmp.Floor = 2;
                    _tmp.UpdateTime = _nowTime;
                    _tmp.UpdateUser = userEntity.Name;

                    _tmp.DefTopRate = Convert.ToDecimal(Convert.ToDouble(set.LocY0) / Convert.ToDouble(530));
                    _tmp.DefLeftRate = Convert.ToDecimal(Convert.ToDouble(set.LocX0) / Convert.ToDouble(1300));
                    _tmp.DefWidth = set.LocX1 - set.LocX0;
                    _tmp.DefHeight = set.LocY1 - set.LocY0;

                    _insMonitorSettings.Add(_tmp);
                });

                using (TransactionScope scope = new TransactionScope())
                {
                    _monitorSettingRepository.Delete();

                    if (_monitorSettingRepository.Insert(_insMonitorSettings) == _insMonitorSettings.Count)
                        scope.Complete();
                    else
                        _updRes = "新增異常";
                }

                return _updRes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string ProcessProdMoveCnt(List<ProdXmlTmpDao> prodList)
        {
            string _moveCntStr = "";
            int _lastMoveCnt = 0;

            prodList.OrderBy(o => o.XML_time).ToList().ForEach(f =>
            {
                _moveCntStr += Math.Abs(_lastMoveCnt - Convert.ToInt32(f.move_cnt)) + "/";
                _lastMoveCnt = Convert.ToInt32(f.move_cnt);
            });

            return _moveCntStr.Remove(_moveCntStr.Length - 1);
        }

        #region Tack Time

        public MonitorEqTTMainEntity GetEqTackTimeList()
        {
            try
            {
                DateTime _nowTime = DateTime.Now;
                bool _isShiftB = _nowTime.TimeOfDay > new TimeSpan(19, 30, 0);

                var _allProdTTInfoToday = GetProdPerfDetail(_isShiftB, _nowTime);

                var _prodTTMapList = GetEqTackTimeMap(_allProdTTInfoToday);

                IEnumerable<MonitorTackTimeEntity> monitorTackTimeList = _allProdTTInfoToday.GroupBy(g => new { g.ProdId, g.ProdDesc })
                    .Select(prod => new MonitorTackTimeEntity
                    {
                        ProdSn = prod.Key.ProdId,
                        ProdDesc = prod.Key.ProdDesc,
                        DetailTTInfo = prod.OrderBy(o => o.WorkCtr).Select(s => new MonitorTackTimeDetailEntity
                        {
                            Node = s.WorkCtr,
                            EquipmentNo = s.EquipNo,
                            TargetTackTime = s.TimeTarget,
                            TackTime = s.TackTimeStr,
                            TTWarningLevel = s.TimeTarget == 0 ? "N/A" : s.TTWarningLevelId.GetDescription(),
                            TTWarningLevelId = s.TTWarningLevelId,
                        }).ToList()
                    });

                return new MonitorEqTTMainEntity
                {
                    EqTackTimeList = monitorTackTimeList.OrderBy(o => o.ProdSn).ToList(),
                    EqTackTimeAreaList = _prodTTMapList
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public (string, List<MonitorEqTTHistoryEntity>) GetEqHistory(string eqNumber)
        {
            try
            {
                DateTime _nowTime = DateTime.Now;
                bool _isShiftB = _nowTime.TimeOfDay > new TimeSpan(19, 30, 0);

                var _prodDetailAllList = _prodPerformanceDetailRepository.SelectByEqNumber(
                        eqNumber,
                        _isShiftB ? DateTime.Parse($"{_nowTime:yyyy-MM-dd} 19:30:00") : DateTime.Parse($"{_nowTime:yyyy-MM-dd} 07:30:00"),
                        _isShiftB ? DateTime.Parse($"{_nowTime.AddDays(1):yyyy-MM-dd} 07:30:00") : DateTime.Parse($"{_nowTime:yyyy-MM-dd} 19:30:00"));

                var _defProdList = _lcmProductRepository.SelectByConditions(prodNoList: _prodDetailAllList.Select(detail => detail.ProdNo).Distinct().ToList());
                var _targetSettingList = _targetSettingRepository.SelectByConditions(_defProdList.Select(def => def.sn).ToList(), null);
                var _ttTargetList = _inxReportService.GetEntityTTMntReportAsync<INXRptTTMntEntity>(_nowTime, _prodDetailAllList.Select(detail => detail.ProdNo).Distinct().ToList()).Result.Date.Data.Table;


                //var _chk = from detail in _prodDetailAllList.GroupBy(b => new { b.ProdNo, b.WorkCtr, b.EquipNo }).Select(s => s.Key)
                //           join defProd in _defProdList
                //           on detail.ProdNo equals defProd.ProdNo
                //           join target in _targetSettingList
                //           on new { detail.EquipNo, defProd.sn } equals new { EquipNo = target.DownEquipment, sn = target.lcmProdSn }
                //           into temp
                //           from target in temp.DefaultIfEmpty()
                //           where target is null
                //           select new
                //           {
                //               detail.ProdNo,
                //               detail.EquipNo,
                //               detail.WorkCtr
                //           };

                //if (_chk.Any())
                //{
                //    return (string.Join("\r\n", _chk.Select(s => $"{s.WorkCtr}/{s.ProdNo}/{s.EquipNo}")), null);
                //}

                //var _resList = (from detail in _prodDetailAllList
                //                join defProd in _defProdList
                //                on detail.ProdNo equals defProd.ProdNo
                //                join target in _targetSettingList
                //                on new { detail.EquipNo, defProd.sn } equals new { EquipNo = target.DownEquipment, sn = target.lcmProdSn }
                //                select new ProdPerfDetailEntity
                //                {
                //                    EquipNo = detail.EquipNo,
                //                    TackTime = detail.TackTime,
                //                    TimeTarget = target.TimeTarget,
                //                    TransDate = detail.TransDate,
                //                    Operator = detail.Operator,
                //                    ProdDesc = detail.ProdNo,
                //                    ProdId = defProd.sn
                //                }).GroupBy(g => new { g.ProdId, g.ProdDesc, g.Operator })
                //                .Select(s => new MonitorEqTTHistoryEntity
                //                {
                //                    ProdNo = s.Key.ProdDesc,
                //                    Operator = s.Key.Operator,
                //                    TargetTT = s.FirstOrDefault().TimeTarget,
                //                    MaxTT = Convert.ToDecimal(s.Max(max => max.TackTime) ?? 0),
                //                    minTT = Convert.ToDecimal(s.Min(min => min.TackTime) ?? 0),
                //                    MedianTT = CalculateMedian(s.Select(sub => Convert.ToDouble(sub.TackTime)).ToArray()),
                //                    AvgTT = Math.Round(s.Sum(sum => sum.TackTime) / s.Count() ?? 0, 1),
                //                    EqTTHistoryList = s.OrderBy(o => o.TransDate).ToList()
                //                }).ToList();

                var _resList = (from detail in _prodDetailAllList
                                join trg in _ttTargetList
                                on new { PROD_NBR = detail.ProdNo, EQUIP_NBR = detail.EquipNo } equals new { trg.PROD_NBR, trg.EQUIP_NBR } into r
                                from t in r.DefaultIfEmpty()
                                select new ProdPerfDetailEntity
                                {
                                    EquipNo = detail.EquipNo,
                                    TackTime = detail.TackTime,
                                    TimeTarget = Convert.ToInt32(t?.TACT_TIME ?? 0),
                                    TransDate = detail.TransDate,
                                    Operator = detail.Operator,
                                    ProdDesc = detail.ProdNo,
                                }).GroupBy(g => new { g.ProdId, g.ProdDesc, g.Operator })
                                .Select(s => new MonitorEqTTHistoryEntity
                                {
                                    ProdNo = s.Key.ProdDesc,
                                    Operator = s.Key.Operator,
                                    TargetTT = s.FirstOrDefault().TimeTarget,
                                    MaxTT = Convert.ToDecimal(s.Max(max => max.TackTime) ?? 0),
                                    minTT = Convert.ToDecimal(s.Min(min => min.TackTime) ?? 0),
                                    MedianTT = CalculateMedian(s.Select(sub => Convert.ToDouble(sub.TackTime)).ToArray()),
                                    AvgTT = Math.Round(s.Sum(sum => sum.TackTime) / s.Count() ?? 0, 1),
                                    EqTTHistoryList = s.OrderBy(o => o.TransDate).ToList()
                                }).ToList();

                return ("", _resList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<MonitorEqTTInfoEntity> GetEqTackTimeMap(IEnumerable<ProdPerfDetailEntity> prodPerfDetailList)
        {
            try
            {
                var _mapAreaSettingList = _monitorSettingRepository.SelectSettings();

                return (from detail in prodPerfDetailList.GroupBy(g => new { g.WorkCtr, g.EquipNo })
                        join setting in _mapAreaSettingList
                        on detail.Key.EquipNo equals setting.EqNumber
                        select new MonitorEqTTInfoEntity
                        {
                            Node = detail.Key.WorkCtr,
                            EqNumber = setting.EqNumber,
                            DefTopRate = setting.DefTopRate,
                            DefLeftRate = setting.DefLeftRate,
                            DefWidth = setting.DefWidth,
                            DefHeight = setting.DefHeight,
                            Border = setting.Border,
                            Background = ConvertColorHEXtoRGB(setting.Background),
                            TackTime = detail.FirstOrDefault(f => f.TTWarningLevelId != TTWarningLevelEnum.None)?.TackTimeStr ?? "--",
                            TTWarningLevelId = detail.FirstOrDefault(f => f.TTWarningLevelId != TTWarningLevelEnum.None)?.TTWarningLevelId ?? TTWarningLevelEnum.None
                        }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private IEnumerable<ProdPerfDetailEntity> GetProdPerfDetail(bool isShiftB, DateTime nowTime)
        {
            try
            {
                var _prodDetailAllList = _prodPerformanceDetailRepository.SelectByConditions(
                        isShiftB ? DateTime.Parse($"{nowTime:yyyy-MM-dd} 19:30:00") : DateTime.Parse($"{nowTime:yyyy-MM-dd} 07:30:00"),
                        isShiftB ? DateTime.Parse($"{nowTime.AddDays(1):yyyy-MM-dd} 07:30:00") : DateTime.Parse($"{nowTime:yyyy-MM-dd} 19:30:00"));

                var _prodDetailCurrent = _prodDetailAllList.Where(w => w.TransDate > nowTime.AddMinutes(-6)).ToList();
                var _prodDetailToday = _prodDetailAllList.Where(w => w.TransDate <= nowTime.AddMinutes(-6) && w.Sn == 1).ToList();

                var _defProdList = _lcmProductRepository.SelectByConditions(prodNoList: _prodDetailAllList.Select(detail => detail.ProdNo).Distinct().ToList());

                var _targetSettingList = _targetSettingRepository.SelectByConditions(_defProdList.Select(def => def.sn).ToList(), null);
                var _ttTargetList = _inxReportService.GetEntityTTMntReportAsync<INXRptTTMntEntity>(nowTime, _prodDetailAllList.Select(detail => detail.ProdNo).Distinct().ToList()).Result.Date.Data.Table;

                //var _prodTTInfo = from detail in _prodDetailCurrent.Where(w => w.Sn == 1)
                //                  join def in _defProdList
                //                  on detail.ProdNo equals def.ProdNo into r1
                //                  from t1 in r1.DefaultIfEmpty()
                //                  join tSetting in _targetSettingList
                //                  on new { lcmProdSn = t1?.sn ?? 0, Node = detail.WorkCtr } equals new { tSetting.lcmProdSn, tSetting.Node } into r2
                //                  from t2 in r2.DefaultIfEmpty()
                //                  select new ProdPerfDetailEntity
                //                  {
                //                      WorkCtr = detail.WorkCtr,
                //                      EquipNo = detail.EquipNo,
                //                      ProdId = t1?.sn ?? 0,
                //                      ProdDesc = $"{detail.ProdNo}-{t1?.Descr ?? "XX"}",
                //                      TimeTarget = t2?.TimeTarget ?? 0,
                //                      TackTime = detail.TackTime,
                //                      //MedianTT = CalculateMedian(_prodDetailList.Where(w => w.ProdNo == detail.ProdNo && w.WorkCtr == detail.WorkCtr).Select(s => Convert.ToDouble(s.TackTime)).ToArray()),
                //                      TTWarningLevelId = detail.TackTime > (t2?.TimeTarget ?? 0) && detail.TackTime <= Convert.ToDecimal((t2?.TimeTarget ?? 0) * 1.1) ? TTWarningLevelEnum.Warning
                //                          : detail.TackTime > Convert.ToDecimal((t2?.TimeTarget ?? 0) * 1.1) ? TTWarningLevelEnum.Bad : TTWarningLevelEnum.Good
                //                  };

                var _prodTTInfo = (from detail in _prodDetailCurrent.Where(w => w.Sn == 1)
                                  join trg in _ttTargetList
                                  on new { PROD_NBR = detail.ProdNo, EQUIP_NBR = detail.EquipNo } equals new { trg.PROD_NBR, trg.EQUIP_NBR } into r1
                                  from t1 in r1.DefaultIfEmpty()
                                  select new ProdPerfDetailEntity
                                  {
                                      WorkCtr = detail.WorkCtr,
                                      EquipNo = detail.EquipNo,
                                      ProdDesc = detail.ProdNo,
                                      TimeTarget = Convert.ToInt32(t1?.TACT_TIME ?? 0),
                                      TackTime = detail.TackTime,
                                      TTWarningLevelId = detail.TackTime > (t1?.TACT_TIME ?? 0) && detail.TackTime <= Convert.ToDecimal(Convert.ToInt32(t1?.TACT_TIME ?? 0) * 1.1) ? TTWarningLevelEnum.Warning
                                          : detail.TackTime > Convert.ToDecimal(Convert.ToInt32(t1?.TACT_TIME ?? 0) * 1.1) ? TTWarningLevelEnum.Bad : TTWarningLevelEnum.Good
                                  }).ToList();

                var _prodTTInfoToday = from detail in _prodDetailToday
                                       join trg in _ttTargetList
                                       on new { PROD_NBR = detail.ProdNo, EQUIP_NBR = detail.EquipNo } equals new { trg.PROD_NBR, trg.EQUIP_NBR } into r1
                                       from t1 in r1.DefaultIfEmpty()
                                       select new ProdPerfDetailEntity
                                       {
                                           WorkCtr = detail.WorkCtr,
                                           EquipNo = detail.EquipNo,
                                           ProdDesc = detail.ProdNo,
                                           TimeTarget = Convert.ToInt32(t1?.TACT_TIME ?? 0),
                                           TackTime = null,
                                           TTWarningLevelId = TTWarningLevelEnum.None
                                       };

                return _prodTTInfo.Union(_prodTTInfoToday);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        private string ConvertColorHEXtoRGB(string colorHEX)
        {
            Color color = ColorTranslator.FromHtml(colorHEX);
            int r = Convert.ToInt16(color.R);
            int g = Convert.ToInt16(color.G);
            int b = Convert.ToInt16(color.B);
            return string.Format("rgba({0}, {1}, {2}, {3})", r, g, b, 0.6);
        }


        /// <summary>
        /// 計算中位數
        /// </summary>
        /// <param name="timeDiffArray">TT</param>
        /// <returns></returns>
        private decimal CalculateMedian(double[] tackTimeArray)
        {
            try
            {
                decimal _manualCul;

                if (tackTimeArray.Length == 0)
                    return 0;
                else if (tackTimeArray.Length < 4)
                    return Convert.ToDecimal(tackTimeArray[0]);

                // 小到大排序
                Array.Sort(tackTimeArray);

                tackTimeArray = ReProcessWithLimit(tackTimeArray);

                // 取中位數
                if (tackTimeArray.Length % 2 != 0)
                {
                    var _idx = tackTimeArray.Length / 2;
                    _manualCul = Convert.ToDecimal(tackTimeArray[_idx]);
                }
                else
                {
                    var _idx = tackTimeArray.Length / 2;
                    _manualCul = (Convert.ToDecimal(tackTimeArray[_idx - 1]) + Convert.ToDecimal(tackTimeArray[_idx])) / 2;
                }

                var _ex = tackTimeArray.Median();

                return _manualCul;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private double[] ReProcessWithLimit(double[] tackTimeArraySort)
        {
            try
            {
                double _Q1;
                double _Q3;
                double _upLimit;
                double _butLimit;
                List<double> _outArray = new List<double>();

                // 取中位數
                if (tackTimeArraySort.Length % 2 != 0)
                {
                    _Q1 = tackTimeArraySort[Convert.ToInt32(tackTimeArraySort.Length * 0.25)]; // 第 25% 的數值
                    _Q3 = tackTimeArraySort[Convert.ToInt32(tackTimeArraySort.Length * 0.75)]; // 第 75% 的數值
                }
                else
                {
                    _Q1 = (tackTimeArraySort[Convert.ToInt32(tackTimeArraySort.Length * 0.25) - 1] + tackTimeArraySort[Convert.ToInt32(tackTimeArraySort.Length * 0.25)]) / 2; // 第 25% 的數值
                    _Q3 = (tackTimeArraySort[Convert.ToInt32(tackTimeArraySort.Length * 0.75) - 1] + tackTimeArraySort[Convert.ToInt32(tackTimeArraySort.Length * 0.75)]) / 2; // 第 75% 的數值
                }

                _upLimit = _Q3 + (1.5 * (_Q3 - _Q1)); // 上邊界
                _butLimit = _Q1 - (1.5 * (_Q3 - _Q1)); // 下邊界

                foreach (double diffT in tackTimeArraySort)
                {
                    if (_butLimit <= diffT && diffT <= _upLimit)
                        _outArray.Add(diffT);
                }

                return _outArray.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
