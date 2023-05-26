using System.Collections.Generic;
using System.ComponentModel;

namespace MOD4.Web.ViewModel
{
    public class ManufactureViewModel
    {
        [DisplayName("Process")]
        public string Process { get; set; }

        [DisplayName("類別")]
        public string Category { get; set; }

        [DisplayName("本月計畫")]
        public string MonthPlan { get; set; }

        [DisplayName("名稱")]
        public string ProductName { get; set; }

        public List<ManufactureDetailViewModel> PlanDetail { get; set; }
    }
}
