using MOD4.Web.Enum;
using System;

namespace MOD4.Web.DomainService.Entity
{
    public class BookingRoomEntity
    {
        public int Sn { get; set; }
        public MeetingRoomEnum MeetingRoomId { get; set; }
        public string MeetingRoom { get; set; }
        public DateTime StartTime { get; set; }
        public string StartTimeStr { get; set; }
        public DateTime EndTime { get; set; }
        public string EndTimeStr { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string BackgroundColor { get; set; }

        public string Date { get; set; }
        public string Time { get; set; }
        public bool RepeatWeekly { get; set; }
    }
}
