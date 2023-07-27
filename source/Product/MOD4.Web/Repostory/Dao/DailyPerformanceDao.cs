using MOD4.Web.Enum;
using System;

namespace MOD4.Web.Repostory.Dao
{
    public class DailyPerformanceDao
    {
        public DateTime MFG_Day { get; set; }
        public ProcessEnum ProcessId { get; set; }
        public int LcmProdSn { get; set; }
        public string ProdNo { get; set; }
        public int Floor { get; set; }
        public string Shift { get; set; }
        public int PassQty { get; set; }
        public decimal InlineEfficiency { get; set; }
        public decimal InOfflineEfficiency { get; set; }
        public DateTime? OpenTime { get; set; }
        public DateTime? CloseTime { get; set; }
        public decimal MedianTT { get; set; }
        public int TTLProcessSec { get; set; }
        public int TTLProcessSecOffRest { get; set; }
    }
}
