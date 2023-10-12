using MOD4.Web.Enum;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.Repostory
{
    public class CarUXBulletinRepository : BaseRepository, ICarUXBulletinRepository
    {

        public List<CarUXBulletinDao> SelectByConditions(List<int> snList = null, long orderNo = 0, DateTime? startDate = null, DateTime? endDate = null)
        {
            string sql = "select * from carux_bulletin where 1=1 ";

            if (snList != null && snList.Any())
                sql += " and serialNo IN @SerialNo ";
            if (orderNo != 0)
                sql += " and orderNo = @OrderNo ";
            if (startDate != null && endDate != null)
                sql += " and date between @StartDate and @EndDate ";

            var dao = _dbHelper.ExecuteQuery<CarUXBulletinDao>(sql, new
            {
                SerialNo = snList,
                OrderNo = orderNo,
                StartDate = startDate,
                EndDate = endDate
            });

            return dao;
        }

        public List<CarUXBulletinDetailDao> SelectDetailByConditions(List<int> bulletinSn = null, string jobId = "", string readStatus = "")
        {
            string sql = @" select * from carux_bulletin_detail where 1=1 ";

            if (bulletinSn != null && bulletinSn.Any())
                sql += " and bulletinSn in @BulletinSn ";
            if (!string.IsNullOrEmpty(jobId))
                sql += " and jobId = @JobId ";
            if (!string.IsNullOrEmpty(readStatus) && readStatus != "0" && int.TryParse(readStatus, out _))
                sql += " and status = @Status ";

            var dao = _dbHelper.ExecuteQuery<CarUXBulletinDetailDao>(sql, new
            {
                BulletinSn = bulletinSn,
                JobId = jobId,
                Status = readStatus
            });

            return dao;
        }

        public int Insert(CarUXBulletinDao bulletinDao)
        {
            string sql = @"INSERT INTO [dbo].[carux_bulletin]
           ([date]
           ,[orderNo]
           ,[authorAccountId]
           ,[subject]
           ,[content]
           ,[fileName]
           ,[filePath]
           ,[targetSections]
           ,[createUser]
           ,[createTime])
     VALUES
           (@date
           ,@orderNo
           ,@authorAccountId
           ,@subject
           ,@content
           ,@fileName
           ,@filePath
           ,@targetSections
           ,@createUser
           ,@createTime); ";

            var dao = _dbHelper.ExecuteNonQuery(sql, bulletinDao);

            return dao;
        }

        public int UpdateDetail(CarUXBulletinDetailDao updDao)
        {
            string sql = @" UPDATE [dbo].[carux_bulletin_detail]
   SET [status] = @status
      ,[readDate] = @readDate
      ,[ip] = ''
      ,[updateTime] = @updateTime
 WHERE [bulletinSn] = @bulletinSn and jobId = @jobId ; ";

            var dao = _dbHelper.ExecuteNonQuery(sql, updDao);

            return dao;
        }

        public int InsertDetail(List<CarUXBulletinDetailDao> bulletinDetailDao)
        {
            string sql = @"INSERT INTO [dbo].[carux_bulletin_detail]
           ([bulletinSn]
           ,[jobId]
           ,[status])
     VALUES
           (@bulletinSn
           ,@jobId
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
