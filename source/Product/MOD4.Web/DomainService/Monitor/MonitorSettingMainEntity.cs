using System;
using System.Collections.Generic;

namespace MOD4.Web.DomainService.Entity
{
    public class MonitorSettingMainEntity
    {
        public List<MonitorSettingEntity> SettingDetails { get; set; }

        public List<MonitorProdTTEntity> ProdTTDetails { get; set; }

    }
}
