using System.Collections.Generic;

namespace MOD4.Web.DomainService.Entity
{
    public class MTBFMTTRDashboardEntity
    {
        public string MTBFTarget { get; set; }

        public string MTBFActual { get; set; }

        public string MTTRTarget { get; set; }

        public string MTTRActual { get; set; }

        public List<MTTRDetailEntity> MTTRDetail { get; set; }

        public List<EquipmentEntity> EqpInfoDetail { get; set; }

    }
}
