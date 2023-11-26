using MOD4.Web.DomainService.Entity;
using System;
using System.Collections.Generic;

namespace MOD4.Web.DomainService
{
    public interface IMonitorDomainService
    {
        MonitorEntity GetMapPerAlarmData();

        List<MonitorProdPerInfoEntity> GetProdPerformanceInfo();

        List<MonitorAlarmTopEntity> GetAlarmTopDaily();

        MonitorSettingMainEntity GetMonitorMainList(int prodSn = 1206);

        List<MonitorProdTTEntity> GetMonitorProdTTList(int prodSn);

        List<MonitorSettingEntity> GetMonitorAreaSettingList();

        string UpdateProdTT(List<MonitorProdTTEntity> prodTTEntity, UserEntity userEntity);

        string UpdateInsertMapArea(List<MonitorSettingEntity> mapAreaEntity, UserEntity userEntity);
    }
}