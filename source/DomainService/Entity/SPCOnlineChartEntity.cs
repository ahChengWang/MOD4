using System;
using System.Collections.Generic;

namespace MOD4.Web.DomainService.Entity
{
    public class SPCOnlineChartEntity
    {
        public string ChartId { get; set; }

        public string TypeStr { get; set; }

        public string TestItem { get; set; }

        public string XBarBar { get; set; }

        public string Sigma { get; set; }

        public string Ca { get; set; }

        public string Cp { get; set; }

        public string Cpk { get; set; }

        public string Sample { get; set; }

        public string n { get; set; }

        public string RMBar { get; set; }

        public string PpkBar { get; set; }

        public string PpkSigma { get; set; }

        public string Pp { get; set; }

        public string Ppk { get; set; }

        public List<SPCMicroScopeDataEntity> DetailList { get; set; }
    }
}
