using MOD4.Web.Enum;
using System.Collections.Generic;

namespace MOD4.Web.DomainService.Entity
{
    public class TakeBackWTEntity
    {
        public int Sn { get; set; }

        public WTCategoryEnum? WTCategoryId { get; set; }

        public string Date { get; set; }

        public string TakeBackBonding { get; set; }

        public string TakeBackFOG { get; set; }

        public string TakeBackLAM { get; set; }

        public string TakeBackASSY { get; set; }

        public string TakeBackCDP { get; set; }

        public string TotalTakeBack { get; set; }

        public string TakeBackPercent { get; set; }

        public List<TakeBackWTProdEntity> DetailList { get; set; }

        public List<TakeBackAttendanceEntity> AttendanceList { get; set; }
    }
}
