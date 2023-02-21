using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface ISPCChartSettingRepository
    {
        List<SPCChartSettingDao> SelectByConditions(string chartgrade, int floor, List<string> dataGroupList = null, List<string> prodList = null, List<string> eqList = null);

        List<SPCChartSettingDao> SelectSPCFloorAndChartGrade();
    }
}
