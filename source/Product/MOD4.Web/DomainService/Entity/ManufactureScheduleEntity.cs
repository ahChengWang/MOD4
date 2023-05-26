using System.Collections.Generic;

namespace MOD4.Web.DomainService.Entity
{
    public class ManufactureScheduleEntity
    {
        public string Process { get; set; }
        public string Category { get; set; }
        public string MonthPlan { get; set; }
        public string ProductName { get; set; }
        public List<ManufactureDetailEntity> PlanDetail { get; set; }
    }
}
