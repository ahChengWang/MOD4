using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using System.Collections.Generic;
using System.IO;

namespace MOD4.Web.DomainService
{
    public interface IDemandDomainService
    {
        List<DemandEntity> GetDemands(UserEntity userEntity, string sn = null, string dateStart = null, string dateEnd = null, string categoryId = null, string statusId = "1,2,3,4", string kw = "");

        DemandEntity GetDemandDetail(int sn, string orderId = "");

        (FileStream, string) GetDownFileStr(int sn, int typeId, int fileNo);

        (bool, string) InsertDemand(DemandEntity insertEntity, UserEntity userEntity);

        (bool, string) UpdateDemand(DemandEntity updEntity, DemandStatusEnum newStatusId, UserEntity userEntity);

    }
}
