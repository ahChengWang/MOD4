using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using System.Collections.Generic;

namespace MOD4.Web.DomainService
{
    public interface IDemandDomainService
    {
        List<DemandEntity> GetDemands(string sn = null, string dateStart = null, string dateEnd = null, string categoryId = null, string statusId = null);

        DemandEntity GetDemandDetail(int sn, string orderId);

        (bool, string) InsertDemand(DemandEntity insertEntity, UserEntity userEntity);

    }
}
