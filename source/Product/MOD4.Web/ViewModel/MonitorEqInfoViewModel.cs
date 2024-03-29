﻿using MOD4.Web.Enum;

namespace MOD4.Web.ViewModel
{
    public class MonitorEqInfoViewModel
    {
        public string EqNumber { get; set; }

        public decimal DefTopRate { get; set; }

        public decimal DefLeftRate { get; set; }

        public decimal DefWidth { get; set; }

        public decimal DefHeight { get; set; }

        public string Border { get; set; }

        public string Background { get; set; }

        public string StatusCode { get; set; }

        public string ProdNoConcate { get; set; }

        public string Comment { get; set; }

        public bool IsFrontEnd { get; set; }

        public string ProdNo { get; set; }

        public string StartTime { get; set; }

        public bool IsAbnormal { get; set; }

        public string Area { get; set; }

        public string PassQty { get; set; }

        // for tack time monitor

        public string TackTime { get; set; }

        public TTWarningLevelEnum TTWarningLevelId { get; set; }
    }
}
