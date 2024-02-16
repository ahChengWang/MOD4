using System.Collections.Generic;

namespace MOD4.Web.DomainService.Entity
{
    public class IELayoutDetailEntity
    {
        public IELayoutEntity LayoutOrderInfo { get; set; }
        public List<IELayoutAuditEntity> AuditList { get; set; }
    }
}
