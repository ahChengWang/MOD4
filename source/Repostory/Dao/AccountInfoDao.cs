using MOD4.Web.Enum;

namespace MOD4.Web.Repostory.Dao
{
    public class AccountInfoDao
    {
        public int sn { get; set; }
        public string account { get; set; }
        public string password { get; set; }
        public string name { get; set; }
        public RoleEnum role { get; set; }
        public int level_id { get; set; }
        public string emp_no { get; set; }
        public string api_key { get; set; }
    }
}
