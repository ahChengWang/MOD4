namespace MOD4.Web.ViewModel
{
    public class MonitorEqInfoViewModel
    {
        public string EqNumber { get; set; }

        public decimal DefTopRate { get; set; }

        public decimal DefLeftRate { get; set; }

        public decimal DefWith { get; set; }

        public decimal DefHeight { get; set; }

        public string Border { get; set; }

        public string Background { get; set; }

        public MonitorAlarmViewModel AlarmInfo { get; set; }

        public MonitorProdPerInfoViewModel ProdPerInfo { get; set; }
    }
}
