using MOD4.Web.Enum;
using System;
using System.Collections.Generic;

namespace MOD4.Web.ViewModel
{
    public class MonitorEqTTHistoryViewModel
    {
        public string ProdNo { get; set; }
        public string Operator { get; set; }
        public decimal MaxTT { get; set; }
        public decimal minTT { get; set; }
        public decimal MedianTT { get; set; }

        public List<MonitorEqTTDetailModel> EqTTDetailList { get; set; }
    }

    public class MonitorEqTTDetailModel
    {
        public string EquipNo { get; set; }
        public string ProdDesc { get; set; }
        public DateTime TransDate { get; set; }
        public string TransDateStr { get; set; }
        public int TimeTarget { get; set; }
        public decimal TackTime { get; set; }
        public string TackTimeStr { get; set; }
        public TTWarningLevelEnum TTWarningLevelId { get; set; }
    }
}
