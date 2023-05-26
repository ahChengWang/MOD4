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
        List<CIMTestBookingDao> SelectByConditions(int sn = 0);
        int Update(CIMTestBookingDao updDao);
        List<CIMTestBookingDao> VerifyOverlap(DateTime startTime, DateTime endTime, int sn = 0);
    }
}