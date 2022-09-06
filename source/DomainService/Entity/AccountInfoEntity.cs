using MOD4.Web.Enum;

namespace MOD4.Web.DomainService.Entity
{
    public class AccountInfoEntity
    {
        public int sn { get; set; }
        public string Name { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public RoleEnum RoleId { get; set; }
    }
}
