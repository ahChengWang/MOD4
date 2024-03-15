using MOD4.Web.Enum;

namespace MOD4.Web.ViewModel
{
    public class TakeBackAttendanceViewModel
    {
        public int TakeBackWTSn { get; set; }
        public CountryEnum CountryId { get; set; }

        public string Country { get; set; }

        public int ShouldPresentCnt { get; set; }

        public int OverTimeCnt { get; set; }

        public int AcceptSupCnt { get; set; }

        public int HaveDayOffCnt { get; set; }

        public int OffCnt { get; set; }

        public int Support { get; set; }

        public int PresentCnt { get; set; }

        public int TotalWorkTime { get; set; }
    }
}
