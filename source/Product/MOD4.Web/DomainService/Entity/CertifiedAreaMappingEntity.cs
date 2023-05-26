using MOD4.Web.Enum;

namespace MOD4.Web.DomainService.Entity
{
    public class CertifiedAreaMappingEntity
    {
        public ApplyAreaEnum AreaId { get; set; }
        public int SubjectId { get; set; }
        public string Subject { get; set; }
    }
}
