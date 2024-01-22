using MOD4.Web.Enum;
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
        public int prod_sn { get; set; }
        public int spend_time { get; set; }
        public string area { get; set; }
        public int repairedTime { get; set; }
        public int shift { get; set; }
        public int processId { get; set; }
        public int eq_unitId { get; set; }
        public int eq_unit_partId { get; set; }
        public int defect_qty { get; set; }
        public string defect_rate { get; set; }
        public string engineer { get; set; }
        public int priority { get; set; }
        public string memo { get; set; }
        public string mnt_user { get; set; }
        public string mnt_minutes { get; set; }
        public int typeId { get; set; }
        public int yId { get; set; }
        public int subYId { get; set; }
        public int xId { get; set; }
        public int subXId { get; set; }
        public int rId { get; set; }
        public int engTypeId { get; set; }
        public int engYId { get; set; }
        public int engSubYId { get; set; }
        public int engXId { get; set; }
        public int engSubXId { get; set; }
        public int engRId { get; set; }
        public int isManual { get; set; }
        public EqIssueStatusEnum statusId { get; set; }
    }
}
