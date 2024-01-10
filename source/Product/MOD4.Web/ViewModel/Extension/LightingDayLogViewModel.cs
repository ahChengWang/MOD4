using MOD4.Web.Enum;
using System;
using System.Collections.Generic;

namespace MOD4.Web.ViewModel
{
    public class LightingDayLogViewModel
    {
        public DateTime LogDate { get; set; }
        public LightingCategoryEnum LightingCategoryId { get; set; }

        public List<LightingDayLogDetailViewModel> Detail { get; set; }
    }


    public class LightingDayLogDetailViewModel
    {
        public int PanelSn { get; set; }
        public LightingCategoryEnum CategoryId { get; set; }
        public string Category { get; set; }
        public string PanelId { get; set; }
        public DateTime PanelDate { get; set; }
        public string CreateUser { get; set; }
        public int Floor { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }
}
