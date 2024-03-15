using MOD4.Web.Enum;

namespace MOD4.Web.ViewModel
{
    public class TakeBackWTProdViewModel
    {
        public int TakeBackWTSn { get; set; }

        public ProcessEnum ProcessId { get; set; }

        public string EqId { get; set; }

        public string Prod { get; set; }

        public string ProdOption { get; set; }

        public int ProdId { get; set; }

        public string IEStandard { get; set; }

        public string IETT { get; set; }

        public string IEWT { get; set; }

        public string PassQty { get; set; }

        public string TakeBackTime { get; set; }

    }
}
