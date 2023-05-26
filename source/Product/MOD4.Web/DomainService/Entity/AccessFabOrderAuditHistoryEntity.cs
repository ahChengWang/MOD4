using MOD4.Web.Enum;
using System;

namespace MOD4.Web.DomainService.Entity
{
    public class AccessFabOrderAuditHistoryEntity : BaseAuditHistoryEntity
    {
        public int AccessFabOrderSn { get; set; }
        public FabInOutStatusEnum StatusId { get; set; }
    }
}
