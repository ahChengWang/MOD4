using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using System.Collections.Generic;

namespace MOD4.Web.DomainService
{
    public interface IDemandDomainService
    {
        List<DemandEntity> GetDemands(string sn = "");

        string InsertDemand(DemandEntity insertEntity, UserEntity userEntity);

    }
}
