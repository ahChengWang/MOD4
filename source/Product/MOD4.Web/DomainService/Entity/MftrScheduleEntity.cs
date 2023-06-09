using MOD4.Web.Enum;
using System;
using System.Collections.Generic;

namespace MOD4.Web.DomainService.Entity
{
    public class MftrScheduleEntity
    {
        public string Process { get; set; }

        public MTDCategoryEnum MTDCategoryId { get; set; }

        public string MTDCategory { get; set; }

        public string ProdNo { get; set; }

        public string ProdDesc { get; set; }

        public string Model { get; set; }

        public int LcmProdId { get; set; }

        public DateTime Date { get; set; }

        public string DateStart { get; set; }

        public int Qty { get; set; }

        public int Floor { get; set; }

        public bool IsMass { get; set; }

        public string UpdateUser { get; set; }

        public DateTime UpdateTime { get; set; }

        public int MonthPlan { get; set; }
    }
}
