using MOD4.Web.Enum;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public class BookingMeetingRepository : BaseRepository, IBookingMeetingRepository
    {

        public List<BookingMeetingDao> SelectByConditions(int sn = 0)
        {
            string sql = "select * from booking_meeting where 1=1 ";

            if (sn != 0)
                sql += " and sn = @sn ";

            var dao = _dbHelper.ExecuteQuery<BookingMeetingDao>(sql, new
            {
                sn = sn
            });

            return dao;
        }

        public int Insert(List<BookingMeetingDao> bookingDaoList)
        {
            string sql = @"INSERT INTO [dbo].[booking_meeting]
([meetingRoomId]
,[startTime]
,[endTime]
,[name]
,[subject]
,[backgroundColor])
VALUES
(@meetingRoomId
,@startTime
,@endTime
,@name
,@subject
,@backgroundColor); ";

            var dao = _dbHelper.ExecuteNonQuery(sql, bookingDaoList);

            return dao;
        }


        public List<BookingMeetingDao> VerifyOverlap(MeetingRoomEnum roomId, DateTime startTime, DateTime endTime)
        {
            string sql = @"select * from [booking_meeting]
where meetingRoomId = @MeetingRoomId
and ((@StartTime between startTime and DATEADD(SECOND,-1,endTime)) 
or (@EndTime between DATEADD(SECOND,1,startTime) and endTime)
or (@StartTime < startTime and endTime < @EndTime)) ";

            var dao = _dbHelper.ExecuteQuery<BookingMeetingDao>(sql, new
            {
                MeetingRoomId = roomId,
                StartTime = startTime,
                EndTime = endTime
            });

            return dao;
        }


        public int Update(BookingMeetingDao updDao)
        {
            string sql = @" UPDATE [dbo].[booking_meeting]
   SET [startTime] = @StartTime
      ,[endTime] = @EndTime
 WHERE Sn=@Sn ; ";

            var dao = _dbHelper.ExecuteNonQuery(sql, updDao);

            return dao;
        }
        
        public int Delete(int meetingSn)
        {
            string sql = @" DELETE [dbo].[booking_meeting] WHERE Sn=@Sn ; ";

            var dao = _dbHelper.ExecuteNonQuery(sql, new {
                Sn = meetingSn
            });

            return dao;
        }
    }
}
