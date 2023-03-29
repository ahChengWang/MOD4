using MOD4.Web.DomainService.Entity;
using System.Collections.Generic;

namespace MOD4.Web.DomainService
{
    public interface IAccessFabDomainService
    {
        List<AccessFabOrderEntity> GetList(UserEntity userEntity, AccessFabSelectOptionEntity selectOption);

        AccessFabOrderEntity GetDetail(int accessFabOrderSn);

        string Create(AccessFabOrderEntity orderEntity, UserEntity userEntity);

        string Edit(AccessFabOrderEntity orderEntity, UserEntity userEntity);

        string Delete(int orderSn, UserEntity userEntity);

        List<AccessFabOrderEntity> GetAuditList(UserEntity userEntity, AccessFabSelectOptionEntity selectOption);

        string Audit(AccessFabOrderEntity orderEntity, UserEntity userEntity);

        string VerifyAuditStatus(int orderSn, UserEntity userEntity);
    }
}
