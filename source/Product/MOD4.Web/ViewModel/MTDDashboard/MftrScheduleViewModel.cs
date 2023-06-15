using MOD4.Web.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MOD4.Web.ViewModel
{
    public class MftrScheduleViewModel
    {
        public int Sn { get; set; }

        [DisplayName("Process")]
        public string Process { get; set; }

        [DisplayName("日期")]
        public DateTime Date { get; set; }

        public string DateStart { get; set; }

        [DisplayName("專案")]
        public string Project { get; set; }

        [DisplayName("機種")]
        public string Product { get; set; }

        public int ProductId { get; set; }

        [DisplayName("計畫量")]
        public int Quantity { get; set; }

        [DisplayName("量產品?")]
        public bool IsMass { get; set; }

        public MTDCategoryEnum MTDCategoryId { get; set; }

        public int Floor { get; set; }

        public bool IsDrop { get; set; }

        public string DateRange { get; set; }
    }
}
