using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface IEfficiencySettingRepository
    {
        List<EfficiencySettingDao> SelectByConditions(List<int> prodList = null, int floor = 0);

        int Insert(List<EfficiencySettingDao> insList);

        int UpdateSetting(List<EfficiencySettingDao> updList);
    }
}