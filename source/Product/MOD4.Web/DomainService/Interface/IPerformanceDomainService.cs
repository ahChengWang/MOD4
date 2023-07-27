﻿using MOD4.Web.DomainService.Entity;
using System;
using System.Collections.Generic;

namespace MOD4.Web.DomainService
{
    public interface IPerformanceDomainService
    {
        List<PassQtyEntity> GetList(string mfgDTE = "", string prodList = "1206", string shift = "", string nodeAryStr = "", int ownerId = 1);

        List<EfficiencyEntity> GetDailyEfficiencyList(string mfgDate, int floor = 3);

        List<EfficiencySettingEntity> GetEfficiencySetting(int floor);

        string UpdateEfficiencySetting(List<EfficiencySettingEntity> updEntity, UserEntity userEntity);
    }
}
