using MOD4.Web.Enum;

namespace MOD4.Web.ViewModel
{
    public class EfficiencySettingDetailViewModel
    {
        public string Process { get; set; }
        public ProcessEnum ProcessId { get; set; }
        public int Node { get; set; }
        public decimal WT { get; set; }
        public int InlineEmps { get; set; }
        public int OfflineEmps { get; set; }
    }
}
