using System;

namespace MOD4.Web.Repostory.Dao
{
    public class MaterialSettingHistoryDao
    {
        public int MatlSn { get; set; }
        public decimal LossRate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
