using MOD4.Web.Enum;

namespace MOD4.Web.DomainService.Entity
{
    public class MenuActionEntity
    {
        public bool IsActionActive { get; set; }
        public PermissionEnum ActionId { get; set; }
        public string Action { get; set; }
    }
}
