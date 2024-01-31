using MOD4.Web.Enum;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.Repostory
{
    public class MTDProductionScheduleRepository : BaseRepository, IMTDProductionScheduleRepository
    {
        public List<MTDProductionScheduleDao> SelectByConditions(
            int floor,
            int ownerId,
            int prodSn,
            DateTime? dateStart,
            DateTime? dateEnd)
        {
            string sql = "select * from mtd_production_schedule where 1=1 ";

            if (floor != 0)
                sql += " and floor = @floor ";
            if (ownerId != 0)
                sql += " and ownerId = @ownerId ";

            if (prodSn != 0)
                sql += " and lcmProdSn = @LcmProdSn ";

            if (dateStart != null && dateEnd != null)
                sql += " and date between @dateStart and @dateEnd ";

            sql += " order by sn asc, model desc, prodId asc, date asc ;";

            var dao = _dbHelper.ExecuteQuery<MTDProductionScheduleDao>(sql, new
            {
                floor = floor,
                ownerId = ownerId,
                LcmProdSn = prodSn,
                dateStart = dateStart,
                dateEnd = dateEnd
            });

            return dao;
        }


        public List<MTDProductionScheduleDao> SelectMTDTodayPlan(
            int floor,
            int owner,
            DateTime dateStart,
            DateTime dateEnd)
        {
            string _sql = @" select * from mtd_production_schedule 
where date between @DateStart and @DateEnd and floor = @Floor and ownerId = @OwnerId ;";

            var dao = _dbHelper.ExecuteQuery<MTDProductionScheduleDao>(_sql, new
            {
                Floor = floor,
                OwnerId = owner,
                DateStart = dateStart,
                DateEnd = dateEnd,
                //year = dateStart.Year,
                //month = dateStart.Month
            });

            return dao;
        }

        public List<MTDProductionScheduleDao> SelectMTDMonHavePlan(
            int floor,
            int owner,
            DateTime dateStart,
            DateTime dateEnd)
        {
            string _sql = @" select * from mtd_production_schedule 
where DATEPART(YEAR, date) = @Year and DATEPART(MONTH, date) = @Month and date != @DateStart and value != 0 and floor = @Floor and ownerId = @OwnerId ; ";

            var dao = _dbHelper.ExecuteQuery<MTDProductionScheduleDao>(_sql, new
            {
                Floor = floor,
                OwnerId = owner,
                DateStart = dateStart,
                DateEnd = dateEnd,
                Year = dateStart.Year,
                Month = dateStart.Month
            });

            return dao;
        }

        public List<MTDProductionScheduleDao> SelectMonthPlanQty(string year, string month, int floor, int ownerId)
        {
            string sql = "select * from mtd_production_schedule where DATEPART(YEAR, date) = @Year and DATEPART(MONTH, date) = @Month and floor = @Floor and ownerId = @ownerId ; ";

            var dao = _dbHelper.ExecuteQuery<MTDProductionScheduleDao>(sql, new
            {
                Year = year,
                Month = month,
                Floor = floor,
                ownerId = ownerId
            });

            return dao;
        }

        public MTDProductionScheduleDao SelectNewOrderSn(string orderNo)
        {
            string sql = "select * from mes_permission where isCancel = 0 and orderNo=@orderNo ";

            var dao = _dbHelper.ExecuteQuery<MTDProductionScheduleDao>(sql, new
            {
                orderNo = orderNo
            }).FirstOrDefault();

            return dao;
        }

        public MTDScheduleUpdateHistoryDao SelectHistory(int floor, int ownerId)
        {
            string sql = "select TOP 1 * from mtd_schedule_update_history where floor = @Floor and ownerId = @OwnerId order by updateTime desc; ";

            var dao = _dbHelper.ExecuteQuery<MTDScheduleUpdateHistoryDao>(sql, new
            {
                Floor = floor,
                OwnerId = ownerId
            }).FirstOrDefault();

            return dao;
        }

        public int InsertSchedule(List<MTDProductionScheduleDao> insMTDSchedule)
        {
            string sql = @"
INSERT INTO [dbo].[mtd_production_schedule]
([sn]
,[process]
,[model]
,[node]
,[eqNo]
,[lcmProdSn]
,[prodId]
,[date]
,[value]
,[floor]
,[ownerId]
,[updateUser]
,[updateTime])
VALUES
(@sn
,@process
,@model
,@node
,@eqNo
,@lcmProdSn
,@prodId
,@date
,@value
,@floor
,@ownerId
,@updateUser
,@updateTime
); ";

            var dao = _dbHelper.ExecuteNonQuery(sql, insMTDSchedule);

            return dao;
        }

        public int UpdateMTDSchedule(List<MTDProductionScheduleDao> updMTDSchedule)
        {
            string sql = @"
UPDATE [dbo].[mtd_production_schedule]
   SET [node] = @node
      ,[eqNo] = @eqNo
      ,[lcmProdSn] = @lcmProdSn
      ,[updateUser] = @updateUser
      ,[updateTime] = @updateTime
 WHERE sn=@sn and lcmProdSn=@lcmProdSn ; ";

            var dao = _dbHelper.ExecuteNonQuery(sql, updMTDSchedule);

            return dao;
        }

        public int DeleteSchedule(int ownerId, DateTime endTime)
        {
            string sql = @"
 Delete [dbo].[mtd_production_schedule] where ownerId = @OwnerId and date >= @Date ; ";

            var dao = _dbHelper.ExecuteNonQuery(sql, new
            {
                OwnerId = ownerId,
                Date = endTime
            });

            return dao;
        }

        public int InsertScheduleHistory(MTDScheduleUpdateHistoryDao insHisDao)
        {
            string sql = @" INSERT INTO [dbo].[mtd_schedule_update_history]
([fileName]
,[floor]
,[ownerId]
,[updateUser]
,[updateTime])
VALUES
(@fileName
,@floor
,@ownerId
,@updateUser
,@updateTime);";

            var dao = _dbHelper.ExecuteNonQuery(sql, insHisDao);

            return dao;
        }

        public List<MTDScheduleSettingDao> SelectSettingByConditions(int prodSn = 0)
        {
            string sql = "select * from mtd_production_setting where 1=1 ";

            if (prodSn != 0)
            {
                sql += " and lcmProdSn = @ProdSn ; ";
            }

            var dao = _dbHelper.ExecuteQuery<MTDScheduleSettingDao>(sql, new
            {
                ProdSn = prodSn
            });

            return dao;
        }

        public int DeleteSetting(int prodSn)
        {
            string sql = @" Delete [dbo].[mtd_production_setting] where lcmProdSn=@lcmProdSn;";

            var dao = _dbHelper.ExecuteNonQuery(sql, new
            {
                lcmProdSn = prodSn
            });

            return dao;
        }

        public int InsertSetting(List<MTDScheduleSettingDao> insSettingDao)
        {
            string sql = @" INSERT INTO [dbo].[mtd_production_setting]
           ([sn]
           ,[process]
           ,[lcmProdSn]
           ,[passNode]
           ,[wipNode]
           ,[wipNode2]
           ,[eqNo]
           ,[updateUser]
           ,[updateTime])
     VALUES
           (@sn
           ,@process
           ,@lcmProdSn
           ,@passNode
           ,@wipNode
           ,@wipNode2
           ,@eqNo
           ,@updateUser
           ,@updateTime);";

            var dao = _dbHelper.ExecuteNonQuery(sql, insSettingDao);

            return dao;
        }
    }
}
