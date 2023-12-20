using System.Collections.Generic;

namespace MOD4.Web.ViewModel
{
    public class MonitorProdTTEditViewModel
    {
        public int Node { get; set; }
        public int LcmProdSn { get; set; }
        public string ProdDesc { get; set; }
        public string DownEquipment { get; set; }
        public List<EqMappingViewModel> DownEqOptions { get; set; }
        public int TimeTarget { get; set; }
    }
}
