using MOD4.Web.DomainService.Entity;
using MOD4.Web.Repostory;
using MOD4.Web.Repostory.Dao;
using NPOI.SS.Formula.Functions;
using NPOI.SS.Formula;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Transactions;
using Utility.Helper;
using System.Security.Claims;

namespace MOD4.Web.DomainService
{
    public class MonitorDomainService : IMonitorDomainService
    {
        private readonly IAlarmXmlRepository _alarmXmlRepository;
        private readonly IMonitorSettingRepository _monitorSettingRepository;
        private readonly ITargetSettingRepository _targetSettingRepository;

        public MonitorDomainService(IAlarmXmlRepository alarmXmlRepository,
            IMonitorSettingRepository monitorSettingRepository,
            ITargetSettingRepository targetSettingRepository)
        {
            _alarmXmlRepository = alarmXmlRepository;
            _monitorSettingRepository = monitorSettingRepository;
            _targetSettingRepository = targetSettingRepository;
        }

        public MonitorEntity GetMapPerAlarmData()
        {
            try
            {
                return new MonitorEntity
                {
                    AlarmDayTop = GetAlarmTopDaily(),
                    ProdPerformanceList = GetProdPerformanceInfo()
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
            var _prodPerInfo = _alarmXmlRepository.SelectProdInfo(_mfgDate.ToString("yyyy-MM-dd"));

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
                        ProdNo = per.prod_id,
                        PassQty = per.move_cnt,
                        StatusCode = alarm?.tool_status ?? "",
                        Comment = string.IsNullOrEmpty(alarm?.comment.Trim() ?? "") ? (alarm?.status_cdsc ?? "") : alarm.comment,
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

                    _tmp.DefTopRate = Convert.ToDecimal(Convert.ToDouble(set.LocY0) / Convert.ToDouble(589));
                    _tmp.DefLeftRate = Convert.ToDecimal(Convert.ToDouble(set.LocX0) / Convert.ToDouble(1517));
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


        private string ConvertColorHEXtoRGB(string colorHEX)
        {
            Color color = ColorTranslator.FromHtml(colorHEX);
            int r = Convert.ToInt16(color.R);
            int g = Convert.ToInt16(color.G);
            int b = Convert.ToInt16(color.B);
            return string.Format("rgba({0}, {1}, {2}, {3})", r, g, b, 0.6);
        }
    }
}
