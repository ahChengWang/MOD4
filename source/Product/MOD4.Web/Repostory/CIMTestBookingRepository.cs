using MOD4.Web.Enum;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public class CIMTestBookingRepository : BaseRepository, ICIMTestBookingRepository
    {

        public List<CIMTestBookingDao> SelectByConditions(int sn = 0)
        {
            string sql = "select * from cim_test_booking where 1=1 ";

            if (sn != 0)
                sql += " and sn = @sn ";

            var dao = _dbHelper.ExecuteQuery<CIMTestBookingDao>(sql, new
            {
                sn = sn
            });

            return dao;
        }

        public int Insert(List<CIMTestBookingDao> bookingDaoList)
        {
            string sql = @"INSERT INTO [dbo].[cim_test_booking]
([cimTestTypeId]
,[name]
,[jobId]
,[subject]
,[floor]
,[cimTestDayTypeId]
,[startTime]
,[endTime]
,[backgroundColor]
,[remark])
VALUES
(@cimTestTypeId
,@name
,@jobId
,@subject
,@floor
,@cimTestDayTypeId
,@startTime
,@endTime
,@backgroundColor
,@remark); ";

            var dao = _dbHelper.ExecuteNonQuery(sql, bookingDaoList);

            return dao;
        }


        public List<CIMTestBookingDao> VerifyOverlap(DateTime startTime, DateTime endTime, int sn = 0)
        {
            string sql = @"select * from cim_test_booking where ((@StartTime between startTime and endTime) 
or (@EndTime between startTime and endTime)) ";

            if (sn != 0)
                sql += " and sn != @Sn";

            var dao = _dbHelper.ExecuteQuery<CIMTestBookingDao>(sql, new
            {
                StartTime = startTime,
                EndTime = endTime,
                Sn = sn
            });

            return dao;
        }


        public int Update(CIMTestBookingDao updDao)
        {
            string sql = @" UPDATE [dbo].[cim_test_booking]
   SET [cimTestTypeId] = @CIMTestTypeId
,[name] = @Name
,[jobId] = @JobId
,[subject] = @Subject
,[floor] = @Floor
,[cimTestDayTypeId] = @CIMTestDayTypeId
,[startTime] = @StartTime
,[endTime] = @EndTime
,[backgroundColor] = @BackgroundColor
,[remark] = @remark
 WHERE sn = @Sn ; ";

            var dao = _dbHelper.ExecuteNonQuery(sql, updDao);

            return dao;
        }

        public int Delete(int meetingSn)
        {
            string sql = @" DELETE [dbo].[cim_test_booking] WHERE Sn=@Sn ; ";

            var dao = _dbHelper.ExecuteNonQuery(sql, new
            {
                Sn = meetingSn
            });

            return dao;
        }
    }
}
