using MOD4.Web.DomainService.Entity;
using System.Collections.Generic;

namespace MOD4.Web.DomainService
{
    public interface ISPCReportDomainService
    {
        List<SPCMainEntity> Search(int floor, string chartgrade, string dateRange, string equpId, string prodId, string dataGroup);
        SPCOnlineChartEntity Detail(int floor, string chartgrade, string dateRange, string equpId, string prodId, string dataGroup);

        List<SPCChartSettingEntity> GetSettingList(int sn, int floor = 0, string chartgrade = "", string prodIdList = "");

        SPCChartSettingEntity GetSettingEdit(int sn);

        string UpdateSPCSetting(SPCChartSettingEntity updEntity, UserEntity userEntity);
    }
}
