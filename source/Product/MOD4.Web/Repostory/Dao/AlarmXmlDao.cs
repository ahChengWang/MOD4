using System;

namespace MOD4.Web.Repostory.Dao
{
    public class AlarmXmlDao
    {
        public int sn { get; set; }
        public string tool_id { get; set; }
        public string tool_status { get; set; }
        public string status_cdsc { get; set; }
        public string user_id { get; set; }
        public string comment { get; set; }
        public DateTime lm_time { get; set; }
        public DateTime XML_time { get; set; }
        public DateTime MFG_Day { get; set; }
        public int MFG_HR { get; set; }
        public DateTime post_time { get; set; }
        public DateTime? end_time { get; set; }
        public string remark4 { get; set; }
        public string remark5 { get; set; }
        public string prod_id { get; set; }
        public string prod_sn { get; set; }
        public int spend_time { get; set; }
        public string area { get; set; }
        public int repairedTime { get; set; }
    }
}
