using System;
using System.ComponentModel;

namespace MOD4.Web.ViewModel
{
    public class SPCDataViewModel
    {
        [DisplayName("Measure Date")]
        public string MeasureDate { get; set; }

        [DisplayName("Measure Time")]
        public string MeasureTime { get; set; }

        [DisplayName("SHT ID")]
        public string SHTId { get; set; }

        [DisplayName("PROD ID")]
        public string ProductId { get; set; }

        [DisplayName("Data Group")]
        public string DataGroup { get; set; }

        [DisplayName("DTX")]
        public double DTX { get; set; }

        [DisplayName("DTRM")]
        public double DTRM { get; set; }
    }
}
