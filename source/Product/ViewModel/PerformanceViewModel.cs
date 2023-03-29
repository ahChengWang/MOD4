using MOD4.Web.Enum;
using System;

namespace MOD4.Web.ViewModel
{
    public class PerformanceViewModel
    {
        public int sn { get; set; }
        public string Account { get; set; }
        public string Name { get; set; }
        public string Main { get; set; }
        public MenuEnum MainId { get; set; }
        public bool HaveSub { get; set; }
        public string Sub { get; set; }
        public int SubId { get; set; }
    }

    public class PerformanceDetail
    {
        public string Prod { get; set; }
        public DateTime Time { get; set; }
        public string Equipment { get; set; }
        public int NG { get; set; }
        public int Qty { get; set; }
        public string Node { get; set; }
    }
}
