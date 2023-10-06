using MOD4.Web.Enum;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public class CarUXBulletinRepository : BaseRepository, ICarUXBulletinRepository
    {

        public List<CarUXBulletinDao> SelectByConditions(int sn = 0)
        {
            string sql = "select * from booking_meeting where 1=1 ";

            if (sn != 0)
                sql += " and sn = @sn ";

            var dao = _dbHelper.ExecuteQuery<CarUXBulletinDao>(sql, new
            {
                sn = sn
            });

            return dao;
        }

        public int Insert(CarUXBulletinDao bulletinDao)
        {
            string sql = @"INSERT INTO [dbo].[carux_bulletin]
           ([date]
           ,[authorAccountId]
           ,[subject]
           ,[content]
           ,[filePath]
           ,[targetSections]
           ,[createUser]
           ,[createTime])
     VALUES
           (@date
           ,@authorAccountId
           ,@subject
           ,@content
           ,@filePath
           ,@targetSections
           ,@createUser
           ,@createTime); ";

            var dao = _dbHelper.ExecuteNonQuery(sql, bulletinDao);

            return dao;
        }

        public int InsertDetail(List<CarUXBulletinDetailDao> bulletinDetailDao)
        {
            string sql = @"INSERT INTO [dbo].[carux_bulletin_detail]
           ([bulletinSn]
           ,[accountId]
           ,[status])
     VALUES
           (@bulletinSn
           ,@accountId
           ,@status); ";

            var dao = _dbHelper.ExecuteNonQuery(sql, bulletinDetailDao);

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

            var dao = _dbHelper.ExecuteNonQuery(sql, new
            {
                Sn = meetingSn
            });

            return dao;
        }
    }
}
