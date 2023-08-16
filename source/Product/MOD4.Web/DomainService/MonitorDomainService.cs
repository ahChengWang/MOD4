using MOD4.Web.DomainService.Entity;
using MOD4.Web.Repostory;
using System;
using System.Linq;

namespace MOD4.Web.DomainService
{
    public class MonitorDomainService : IMonitorDomainService
    {
        private readonly IAlarmXmlRepository _alarmXmlRepository;
        private readonly IEqpInfoRepository _eqpInfoRepository;
        private readonly IOptionDomainService _optionDomainService;

        public MonitorDomainService(IAlarmXmlRepository alarmXmlRepository,
            IEqpInfoRepository eqpInfoRepository,
            IOptionDomainService optionDomainService)
        {
            _alarmXmlRepository = alarmXmlRepository;
            _eqpInfoRepository = eqpInfoRepository;
            _optionDomainService = optionDomainService;
        }

        public MonitorEntity GetAlarmEq()
        {
            try
            {
                DateTime _mfgDate = DateTime.Now.AddMinutes(-450);
                MonitorEntity monitorEntity = new MonitorEntity();

                var _alarmEqList = _alarmXmlRepository.SelectUnrepaired();
                var _alarmDayTop = _alarmXmlRepository.SelectDayTopRepaired("");

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

                return monitorEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
