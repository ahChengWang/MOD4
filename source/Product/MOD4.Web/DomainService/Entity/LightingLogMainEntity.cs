using System;
using System.Collections.Generic;

namespace MOD4.Web.DomainService.Entity
{
    public class LightingLogMainEntity
    {
        public DateTime LogDate { get; set; }

        public List<LightingLogSubEntity> ProcessList { get; set; }
    }
}
