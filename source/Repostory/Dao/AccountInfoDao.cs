using MOD4.Web.Enum;

namespace MOD4.Web.Repostory.Dao
{
    public class AccountInfoDao
    {
        public int sn { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public RoleEnum RoleId { get; set; }
        public int Level { get; set; }
    }
}
