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
        [Required(ErrorMessage = "必填")]
        public string Date { get; set; }

        [DisplayName("時間")]
        [Required(ErrorMessage = "必填")]
        public string TimeStart { get; set; }

        [DisplayName("名字")]
        [Required(ErrorMessage = "必填")]
        public string Name { get; set; }

        [DisplayName("用途")]
        [Required(ErrorMessage = "必填")]
        public string Subject { get; set; }

        [DisplayName("每週重複")]
        public bool RepeatWeekly { get; set; }
    }
}
