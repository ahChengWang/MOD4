using MOD4.Web.Enum;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface ICarUXBulletinRepository
    {
        int Delete(int meetingSn);
        int Insert(CarUXBulletinDao bulletinDao);
        int InsertDetail(List<CarUXBulletinDetailDao> bulletinDetailDao);
        List<CarUXBulletinDao> SelectByConditions(int sn = 0);
        int Update(BookingMeetingDao updDao);
        List<BookingMeetingDao> VerifyOverlap(MeetingRoomEnum roomId, DateTime startTime, DateTime endTime);
    }
}