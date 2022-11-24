using MOD4.Web.Enum;

namespace MOD4.Web.DomainService.Entity
{
    public class AccountMenuInfoEntity
    {
        public int AccountSn { get; set; }
        public MenuEnum MenuSn { get; set; }
        public int MenuGroupSn { get; set; }
        public int AccountPermission { get; set; }
    }
}
