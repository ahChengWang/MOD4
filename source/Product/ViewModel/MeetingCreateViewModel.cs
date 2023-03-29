using MOD4.Web.Enum;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MOD4.Web.ViewModel
{
    public class MeetingCreateViewModel
    {
        [DisplayName("會議室")]
        public MeetingRoomEnum MeetingRoomId { get; set; }

        public string MeetingRoom { get; set; }

        [DisplayName("日期")]
        [Required(ErrorMessage = "必填")]
        public string Date { get; set; }

        [DisplayName("時間")]
        [Required(ErrorMessage = "必填")]
        public string TimeStart { get; set; }

        [DisplayName("名字")]
        [Required(ErrorMessage = "必填")]
        public string Name { get; set; }

        [DisplayName("議題")]
        [Required(ErrorMessage = "必填")]
        public string Subject { get; set; }

        [DisplayName("每週重複")]
        public bool RepeatWeekly { get; set; }

        public int Sn { get; set; }
        public string BackgroundColor { get; set; }
        public int StartYear { get; set; }
        public int StartMonth { get; set; }
        public int StartDay { get; set; }
        public int StartHour { get; set; }
        public int StartMinute { get; set; }
        public int StartSecond { get; set; }

        public int EndYear { get; set; }
        public int EndMonth { get; set; }
        public int EndDay { get; set; }
        public int EndHour { get; set; }
        public int EndMinute { get; set; }
        public int EndSecond { get; set; }

        public DateTime ResizeStartTime { get; set; }
        public DateTime ResizeEndTime { get; set; }
    }
}
