using MOD4.Web.Enum;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface ICarUXBulletinRepository
    {
        List<CarUXBulletinDao> SelectByConditions(List<int> snList = null, long orderNo = 0, DateTime? startDate = null, DateTime? endDate = null);
        List<CarUXBulletinDetailDao> SelectDetailByConditions(List<int> bulletinSn = null, string jobId = "", string readStatus = "");
        int Insert(CarUXBulletinDao bulletinDao);
        int InsertDetail(List<CarUXBulletinDetailDao> bulletinDetailDao);
        int UpdateDetail(CarUXBulletinDetailDao updDao);
        List<BookingMeetingDao> VerifyOverlap(MeetingRoomEnum roomId, DateTime startTime, DateTime endTime);
        int Delete(int meetingSn);
    }
}