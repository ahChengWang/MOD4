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
        public string shift { get; set; }
        public string eq_unit { get; set; }
        public int defect_qty { get; set; }
        public string defect_rate { get; set; }
        public string engineer { get; set; }
        public int category { get; set; }
        public string memo { get; set; }
        public DateTime MFG_Day { get; set; }
        public int MFG_HR { get; set; }
        public string mnt_user { get; set; }
        public string mnt_minutes { get; set; }
    }
}
