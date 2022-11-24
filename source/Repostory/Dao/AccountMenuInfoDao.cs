using MOD4.Web.Enum;

namespace MOD4.Web.Repostory.Dao
{
    public class AccountMenuInfoDao
    {
        public int account_sn { get; set; }
        public MenuEnum menu_sn { get; set; }
        public int menu_group_sn { get; set; }
        public int account_permission { get; set; }
    }
}
