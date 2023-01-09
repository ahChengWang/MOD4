using MOD4.Web.Enum;
using System;

namespace MOD4.Web.Repostory.Dao
{
    public class BookingMeetingDao
    {
        public int Sn { get; set; }
        public MeetingRoomEnum MeetingRoomId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string BackgroundColor { get; set; }
    }
}
