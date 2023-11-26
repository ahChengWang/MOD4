using System.Collections.Generic;

namespace MOD4.Web.ViewModel
{
    public class MonitorSettingViewModel
    {
        public int Node { get; set; }
        public string EqNumber { get; set; }
        public decimal DefWidth { get; set; }
        public decimal DefHeight { get; set; }
        public decimal DefTopRate { get; set; }
        public decimal DefLeftRate { get; set; }
        public int LocX0 { get; set; }
        public int LocY0 { get; set; }
        public int LocX1 { get; set; }
        public int LocY1 { get; set; }
        public string Border { get; set; }
        public string Background { get; set; }
    }
}
