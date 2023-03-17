using MOD4.Web.Enum;
using System;

namespace MOD4.Web.DomainService.Entity
{
    public class MESPermissionAuditHistoryEntity : BaseAuditHistoryEntity
    {
        public int MESPermissionSn { get; set; }
        public DemandStatusEnum StatusId { get; set; }
    }
}
