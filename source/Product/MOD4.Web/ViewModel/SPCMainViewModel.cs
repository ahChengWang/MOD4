using System.ComponentModel;

namespace MOD4.Web.ViewModel
{
    public class SPCMainViewModel
    {
        [DisplayName("Equioment ID")]
        public string EquiomentId { get; set; }

        [DisplayName("PROD ID")]
        public string ProductId { get; set; }

        [DisplayName("測項")]
        public string DataGroup { get; set; }

        [DisplayName("樣本組數")]
        public int Count { get; set; }

        [DisplayName("OOS Num.")]
        public int OOSCount { get; set; }

        [DisplayName("OOC Num.")]
        public int OOCCount { get; set; }

        [DisplayName("OOR Num.")]
        public int OORCount { get; set; }

    }
}
