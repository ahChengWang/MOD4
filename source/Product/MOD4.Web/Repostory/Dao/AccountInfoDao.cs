using MOD4.Web.Enum;

namespace MOD4.Web.Repostory.Dao
{
    public class AccountInfoDao
    {
        public int sn { get; set; }
        public string account { get; set; }
        public string password { get; set; }
        public string name { get; set; }
        public int role { get; set; }
        public JobLevelEnum level_id { get; set; }
        public string jobId { get; set; }
        public string apiKey { get; set; }
        public int deptSn { get; set; }
        public string mail { get; set; }
    }
}
