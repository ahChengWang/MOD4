using System.ComponentModel;

namespace MOD4.Web.ViewModel
{
    public class MaterialSettingDetailViewModel
    {
        public int Sn { get; set; }

        [DisplayName("料號")]
        public string MatlNo { get; set; }

        [DisplayName("品名")]
        public string MatlName { get; set; }

        [DisplayName("料號分類")]
        public string MatlCatg { get; set; }

        [DisplayName("使用站點")]
        public string UseNode { get; set; }

        [DisplayName("損耗率")]
        public decimal LossRate { get; set; }
    }
}
