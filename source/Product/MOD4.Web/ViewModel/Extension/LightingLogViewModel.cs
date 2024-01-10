using MOD4.Web.Enum;
using System;
using System.Collections.Generic;

namespace MOD4.Web.ViewModel
{
    public class LightingLogViewModel
    {
        public DateTime LogDate { get; set; }
        public List<LightingLogSubViewModel> ProcessList { get; set; }
    }

    public class LightingLogSubViewModel
    {
        public LightingCategoryEnum CategoryId { get; set; }
        public string Category { get; set; }
        public int ProcessCnt { get; set; }
    }
}
