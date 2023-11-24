using MOD4.Web.DomainService.Entity;
using MOD4.Web.Repostory;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Transactions;
using Utility.Helper;

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

        public MonitorEntity GetAlarmEq()
        {
            try
            {
                DateTime _mfgDate = DateTime.Now.AddMinutes(-450);
                MonitorEntity monitorEntity = new MonitorEntity();

                var _mapAreaSettingList = _monitorSettingRepository.SelectSettings();
                var _alarmEqList = _alarmXmlRepository.SelectUnrepaired();
                var _alarmDayTop = _alarmXmlRepository.SelectDayTopRepaired(_mfgDate.ToString("yyyy-MM-dd"));
                var _prodPerInfo = _alarmXmlRepository.SelectProdInfo(_mfgDate.ToString("yyyy-MM-dd"));

                monitorEntity.MapAreaList = _mapAreaSettingList.CopyAToB<MonitorSettingEntity>();

                monitorEntity.MapAreaList.ForEach(f => f.Background = ConvertColorHEXtoRGB(f.Background));

                monitorEntity.AlarmList = _alarmEqList.Select(alarm => new MonitorAlarmEntity
                {
                    EqNumber = alarm.tool_id,
                    StatusCode = alarm.tool_status,
                    Comment = string.IsNullOrEmpty(alarm.comment.Trim()) ? alarm.status_cdsc : alarm.comment,
                    ProdNo = alarm.prod_id,
                    StartTime = alarm.lm_time.ToString("yyyy/MM/dd HH:mm:ss"),
                    IsFrontEnd = "BONDING,LAM-FOG".Contains(alarm.area)
                }).ToList();

                monitorEntity.AlarmDayTop = _alarmDayTop.Select(alarm => new MonitorAlarmTopEntity
                {
                    EqNumber = alarm.tool_id,
                    StatusCode = alarm.tool_status,
                    Comment = string.IsNullOrEmpty(alarm.comment.Trim()) ? alarm.status_cdsc : alarm.comment,
                    ProdNo = alarm.prod_id,
                    RepairedTime = $"{alarm.repairedTime}(m)"
                }).ToList();

                monitorEntity.ProdPerInfo = _prodPerInfo.Select(per => new MonitorProdPerInfoEntity
                {
                    EqNumber = per.tool_id,
                    ProdNo = per.prod_id,
                    PassQty = per.move_cnt
                }).ToList();

                return monitorEntity;
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
                    Background = ms.Background,
                    Border = ms.Border
                }).ToList();
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

                List<MonitorSettingDao> _insMonitorSettings = mapAreaEntity.Select(map => new MonitorSettingDao
                {
                    Node = map.Node,
                    EqNumber = map.EqNumber,
                    DefHeight = map.DefHeight,
                    DefLeftRate = map.DefLeftRate / 100,
                    DefTopRate = map.DefTopRate / 100,
                    DefWidth = map.DefWidth,
                    Background = map.Background,
                    Border = map.Border,
                    Floor = 2,
                    UpdateTime = _nowTime,
                    UpdateUser = userEntity.Name
                }).ToList();


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
