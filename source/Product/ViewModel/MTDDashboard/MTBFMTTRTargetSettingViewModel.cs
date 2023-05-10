using System.ComponentModel;

namespace MOD4.Web.ViewModel
{
    public class MTBFMTTRTargetSettingViewModel
    {
        [DisplayName("EQUIP No")]
        public string EquipNo { get; set; }

        [DisplayName("MTBF Target(min.)")]
        public string MTBFTarget { get; set; }

        [DisplayName("MTTR Target(min.)")]
        public string MTTRTarget { get; set; }

        [DisplayName("更新時間")]
        public string UpdateTime { get; set; }

        [DisplayName("更新人員")]
        public string UpdateUser { get; set; }
    }
}
