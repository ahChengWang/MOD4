using MOD4.Web.Enum;
using System;

namespace MOD4.Web.Repostory.Dao
{
    public class EqpInfoDao
    {
        public int sn { get; set; }
        public string Equipment { get; set; }
        public string Operator { get; set; }
        public string Code { get; set; }
        public string Code_Desc { get; set; }
        public string Comments { get; set; }
        public DateTime Start_Time { get; set; }
        public string Repair_Time { get; set; }
        public DateTime Update_Time { get; set; }
        public int shift { get; set; }
        public int processId { get; set; }
        public int eq_unitId { get; set; }
        public int eq_unit_partId { get; set; }
        public int defect_qty { get; set; }
        public string defect_rate { get; set; }
        public string engineer { get; set; }
        public int priority { get; set; }
        public string memo { get; set; }
        public DateTime MFG_Day { get; set; }
        public int MFG_HR { get; set; }
        public string mnt_user { get; set; }
        public string mnt_minutes { get; set; }
        public string prod_id { get; set; }
        public int typeId { get; set; }
        public int yId { get; set; }
        public int subYId { get; set; }
        public int xId { get; set; }
        public int subXId { get; set; }
        public int rId { get; set; }
        public EqIssueStatusEnum statusId { get; set; }
        public string tool_name { get; set; }
        public int prod_sn { get; set; }
        public int isManual { get; set; }
        public string status_desc_ie { get; set; }
    }
}
