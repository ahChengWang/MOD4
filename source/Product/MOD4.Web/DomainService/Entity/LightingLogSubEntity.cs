using MOD4.Web.Enum;
using System;
using System.Collections.Generic;

namespace MOD4.Web.DomainService.Entity
{
    public class LightingLogSubEntity
    {
        public LightingCategoryEnum CategoryId { get; set; }
        public string Category { get; set; }
        public int ProcessCnt { get; set; }
    }
}
