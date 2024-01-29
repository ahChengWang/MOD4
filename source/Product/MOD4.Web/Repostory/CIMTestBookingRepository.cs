using MOD4.Web.Enum;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public class CIMTestBookingRepository : BaseRepository, ICIMTestBookingRepository
    {

        public List<CIMTestBookingDao> SelectByConditions(
            int sn = 0,
            CIMTestTypeEnum testTypeId = 0,
            string jobId = "",
            int floor = 0,
            CIMTestDayTypeEnum testDayTypeId = 0,
            DateTime? startTime = null,
            DateTime? endTime = null)
        {
            string sql = "select * from cim_test_booking where 1=1 ";

            if (sn != 0)
                sql += " and sn = @sn ";

            if (testTypeId != 0)
                sql += " and cimTestTypeId = @CimTestTypeId ";
            else
                sql += " and cimTestTypeId != 99 ";

            if (!string.IsNullOrEmpty(jobId))
                sql += " and jobId = @JobId ";
            if (floor != 0)
                sql += " and floor = @Floor ";
            if (testDayTypeId != 0)
                sql += " and cimTestDayTypeId = @CimTestDayTypeId ";
            if (startTime != null)
                sql += " and startTime >= @StartTime ";

            var dao = _dbHelper.ExecuteQuery<CIMTestBookingDao>(sql, new
            {
                sn = sn,
                CimTestTypeId = testTypeId,
                JobId = jobId,
                Floor = floor,
                CimTestDayTypeId = testDayTypeId,
                StartTime = startTime,
                EndTime = endTime
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

        public List<CIMTestBookingDao> VerifyOverlapByTimeList(List<DateTime> startTimeList, List<DateTime> endTimeList)
        {
            string sql = @"select * from cim_test_booking where cimTestTypeId != 6 and (startTime in @StartTime or endTime in @EndTime); ";

            var dao = _dbHelper.ExecuteQuery<CIMTestBookingDao>(sql, new
            {
                StartTime = startTimeList,
                EndTime = endTimeList
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

        public int UpdateAnn(string updAnn)
        {
            string sql = @" UPDATE [dbo].[cim_test_booking]
   SET [remark] = @remark 
 WHERE [cimTestTypeId] = 99 ; ";

            var dao = _dbHelper.ExecuteNonQuery(sql, new
            {
                remark = updAnn
            });

            return dao;
        }

    }
}
