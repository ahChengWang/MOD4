using MOD4.Web.Enum;
using System.Collections.Generic;

namespace MOD4.Web.DomainService.Entity
{
    public class TakeBackWTKanBanEntity
    {

        public string Date { get; set; }

        public decimal TotalTakeBack { get; set; }

        public decimal TakeBackPercent { get; set; }

        public List<TakeBackWTKanBanDetailEntity> DetailList { get; set; }
    }
}
