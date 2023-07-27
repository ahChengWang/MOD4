using System.Collections.Generic;

namespace MOD4.Web.ViewModel
{
    public class EfficiencySettingViewModel
    {
        public int ProdSn { get; set; }
        public string ProdNo { get; set; }
        public int Floor { get; set; }
        public string Shift { get; set; }
        public string ShiftDesc { get; set; }
        public List<EfficiencySettingDetailViewModel> DetailList { get; set; }
    }
}
