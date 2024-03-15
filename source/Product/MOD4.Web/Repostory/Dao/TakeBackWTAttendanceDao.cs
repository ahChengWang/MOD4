using MOD4.Web.Enum;
using System;

namespace MOD4.Web.Repostory.Dao
{
    public class TakeBackWTAttendanceDao
    {
        public int TakeBackWtSn { get; set; }
        public CountryEnum CountryId { get; set; }
        public int ShouldPresentCnt { get; set; }
        public int OverTimeCnt { get; set; }
        public int AcceptSupCnt { get; set; }
        public int HaveDayOffCnt { get; set; }
        public int OffCnt { get; set; }
        public int Support { get; set; }
        public int PresentCnt { get; set; }
        public int TotalWorkTime { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateTime { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
