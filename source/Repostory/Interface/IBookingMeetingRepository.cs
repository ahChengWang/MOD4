using MOD4.Web.Enum;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface IBookingMeetingRepository
    {
        List<BookingMeetingDao> SelectByConditions(int sn = 0);

        int Insert(List<BookingMeetingDao> bookingDaoList);

        List<BookingMeetingDao> VerifyOverlap(MeetingRoomEnum roomId, DateTime startTime, DateTime endTime);

        int Update(BookingMeetingDao updDao);

        int Delete(int meetingSn);
    }
}
