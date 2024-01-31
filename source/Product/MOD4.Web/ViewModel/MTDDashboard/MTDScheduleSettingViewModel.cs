using System.Collections.Generic;
using System.ComponentModel;

namespace MOD4.Web.ViewModel
{
    public class MTDScheduleSettingViewModel
    {
        [DisplayName("製程")]
        public int Sn { get; set; }

        [DisplayName("過帳站點")]
        public int PassNode { get; set; }
        public int OldPassNode { get; set; }

        [DisplayName("WIP 站點")]
        public int WipNode { get; set; }
        public int OldWipNode { get; set; }

        [DisplayName("WIP 站點 2")]
        public int WipNode2 { get; set; }
        public int OldWipNode2 { get; set; }

        [DisplayName("機況EQ")]
        public string EqNo { get; set; }
        public string OldEqNo { get; set; }

        public int LcmProdSn { get; set; }
    }
}
