using MOD4.Web.DomainService.Entity;
using System.Collections.Generic;

namespace MOD4.Web.DomainService
{
    public interface ISPCReportDomainService
    {
        List<SPCMainEntity> Search(string dateRange, string equpId, string prodId, string dataGroup);
        SPCOnlineChartEntity Detail(string dateRange, string equpId, string prodId, string dataGroup);
    }
}
