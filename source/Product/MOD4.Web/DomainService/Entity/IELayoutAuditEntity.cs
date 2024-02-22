using System.Collections.Generic;

namespace MOD4.Web.DomainService.Entity
{
    public class IELayoutAuditEntity
    {
        public IELayoutEntity LayoutOrderDetailEntity { get; set; }
        public List<OptionEntity> SecretOptoins { get; set; }
        public IELayoutAuditDetailEntity AuditDetailEntity { get; set; }
        public UserEntity UserInfo { get; set; }
    }
}
