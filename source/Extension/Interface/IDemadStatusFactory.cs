using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using System;

namespace MOD4.Web.Extension.Interface
{
    public interface IDemadStatusFactory
    {
        Func<DemandFlowEntity, (bool, string)> GetFlow(DemandStatusEnum statusId);
    }
}
