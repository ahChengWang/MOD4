using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using System.Collections.Generic;
using System.IO;

namespace MOD4.Web.DomainService
{
    public interface IDemandDomainService
    {
        List<DemandEntity> GetDemands(UserEntity userEntity, string sn = null, string dateStart = null, string dateEnd = null, string categoryId = null, string statusId = "1,2,3,6", string kw = "");

        DemandEntity GetDemandDetail(int sn, UserEntity userEntity, string orderId = "");

        (FileStream, string) GetDownFileStr(int sn, int typeId, int fileNo);

        (bool, string) InsertDemand(DemandEntity insertEntity, UserEntity userEntity);

        (bool, string) UpdateDemand(DemandEntity updEntity, UserEntity userEntity);

        List<MESPermissionEntity> GetMESApplicantList(UserEntity userEntity, int sn = 0, string dateStart = null, string dateEnd = null, string statusId = "1,2,4,7,8,9", string kw = "");

        (bool, string) CreateMESApplicant(MESPermissionEntity mESPermissionEntity, UserEntity userEntity);

        MESPermissionEntity GetDetail(int mesPermissionSn);

        MESPermissionEntity GetAudit(int mesPermissionSn, UserEntity userEntity);

        (bool, string) UpdateMES(MESPermissionEntity mesEntity, UserEntity userEntity);

        string AuditMES(int orderSn, DemandStatusEnum statusId, string remark, UserEntity userEntity);
    }
}
