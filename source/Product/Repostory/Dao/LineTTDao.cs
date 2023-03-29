using System;

namespace MOD4.Web.Repostory.Dao
{
    public class LineTTDao
    {
        public int sn { get; set; }
        public string line { get; set; }
        public string model { get; set; }
        public int Time_Diff { get; set; }
        public int P_Diff { get; set; }
        public decimal Line_TT { get; set; }
        public DateTime MFG_Day { get; set; }
        public int MFG_HR { get; set; }
        public DateTime lm_time { get; set; }
        public string Remark { get; set; }
    }
}
