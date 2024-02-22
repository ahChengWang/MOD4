using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using System;
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

        List<MESPermissionEntity> GetMESApplicantList(UserEntity userEntity, int sn = 0, string dateStart = null, string dateEnd = null, string statusId = "1,2,7,8,9", string kw = "", string orderType = "1,2");

        (bool, string) CreateMESApplicant(MESPermissionEntity mESPermissionEntity, UserEntity userEntity);

        MESPermissionEntity GetDetail(int mesPermissionSn);

        MESPermissionEntity GetAudit(int mesPermissionSn, UserEntity userEntity);

        (bool, string) UpdateMES(MESPermissionEntity mesEntity, UserEntity userEntity);

        string AuditMES(string remark, MESPermissionEntity updMESEntity, UserEntity userEntity);

        (bool, string, string) Download(int orderSn);

        List<IELayoutEntity> GetList(DateTime? beginDate, DateTime? endDate, AuditStatusEnum statusId, string applicantUser, UserEntity userInfo);

        string Create(IELayoutCreateEntity createEntity, UserEntity userInfo);

        IELayoutDetailEntity GetLayoutApplyDetail(int orderSn);

        string AuditLayoutApply(IELayoutAuditEntity layoutInfoEntity);

        string Resend(IELayoutCreateEntity createEntity, UserEntity userInfo);

        string Cancel(int orderSn, UserEntity userInfo);
    }
}
