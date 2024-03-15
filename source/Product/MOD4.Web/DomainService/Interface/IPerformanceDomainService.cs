using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
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

        List<TakeBackWTEntity> GetTBWTList(DateTime? searchDate, WTCategoryEnum wtCategoryId);

        (string, TakeBackWTEntity) UpdateTakeBackWT(TakeBackWTEntity editEntity, UserEntity userInfo);
    }
}
