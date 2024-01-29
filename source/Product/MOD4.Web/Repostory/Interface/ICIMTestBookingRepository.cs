using MOD4.Web.Enum;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface ICIMTestBookingRepository
    {
        int Delete(int meetingSn);
        int Insert(List<CIMTestBookingDao> bookingDaoList);
        List<CIMTestBookingDao> SelectByConditions(int sn = 0, CIMTestTypeEnum testTypeId = 0, string jobId = "", int floor = 0, CIMTestDayTypeEnum testDayTypeId = 0, DateTime? startTime = null, DateTime? endTime = null);
        int Update(CIMTestBookingDao updDao);
        List<CIMTestBookingDao> VerifyOverlap(DateTime startTime, DateTime endTime, int sn = 0);
        List<CIMTestBookingDao> VerifyOverlapByTimeList(List<DateTime> startTimeList, List<DateTime> endTimeList);
        int UpdateAnn(string updAnn);
    }
}