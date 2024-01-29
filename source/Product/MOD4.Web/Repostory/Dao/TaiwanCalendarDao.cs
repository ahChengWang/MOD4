using MOD4.Web.Enum;
using System;

namespace MOD4.Web.Repostory.Dao
{
    public class TaiwanCalendarDao
    {
        public DateTime Date { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public bool IsHoliday { get; set; }
        public string Remark { get; set; }
    }
}
