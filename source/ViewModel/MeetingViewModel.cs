using MOD4.Web.Enum;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MOD4.Web.ViewModel
{
    public class MeetingViewModel
    {
        [DisplayName("會議室")]
        public MeetingRoomEnum MeetingRoomId { get; set; }

        public string MeetingRoom { get; set; }

        [DisplayName("日期")]
        public int Date { get; set; }

        [DisplayName("時間")]
        public string TimeStart { get; set; }

        [DisplayName("名字")]
        public string Name { get; set; }

        [DisplayName("用途")]
        public string Subject { get; set; }

        [DisplayName("每週重複")]
        public bool RepeatWeekly { get; set; }
    }
}
