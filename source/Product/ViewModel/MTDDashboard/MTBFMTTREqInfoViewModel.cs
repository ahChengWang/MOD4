using System.ComponentModel;

namespace MOD4.Web.ViewModel
{
    public class MTBFMTTREqInfoViewModel
    {
        [DisplayName("人員")]
        public string Operator { get; set; }
        [DisplayName("Code")]
        public string Code { get; set; }
        [DisplayName("Code Desc.")]
        public string CodeDesc { get; set; }
        [DisplayName("備註")]
        public string Comments { get; set; }
        [DisplayName("Start Time")]
        public string StartTime { get; set; }
        [DisplayName("Repair Time")]
        public string RepairTime { get; set; }
    }
}
