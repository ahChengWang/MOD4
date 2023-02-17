using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MOD4.Web.ViewModel
{
    public class SPCOnlineChartViewModel
    {
        [DisplayName("Chart ID")]
        public string ChartId { get; set; }

        [DisplayName("Type")]
        public string TypeStr { get; set; }

        [DisplayName("測項")]
        public string TestItem { get; set; }

        [DisplayName("XBar-Bar")]
        public string XBarBar { get; set; }

        [DisplayName("Sigma")]
        public string Sigma { get; set; }

        [DisplayName("Ca")]
        public string Ca { get; set; }

        [DisplayName("Cp")]
        public string Cp { get; set; }

        [DisplayName("Cpk")]
        public string Cpk { get; set; }

        [DisplayName("Sample")]
        public string Sample { get; set; }

        [DisplayName("n")]
        public string n { get; set; }

        [DisplayName("RM-Bar")]
        public string RMBar { get; set; }

        [DisplayName("Ppk Bar")]
        public string PpkBar { get; set; }

        [DisplayName("Ppk Sigma")]
        public string PpkSigma { get; set; }

        [DisplayName("Pp")]
        public string Pp { get; set; }

        [DisplayName("Ppk")]
        public string Ppk { get; set; }

        public List<SPCDataViewModel> SPCDetail { get; set; }
    }
}
