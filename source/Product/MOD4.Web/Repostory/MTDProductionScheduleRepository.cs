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
            DateTime? dateStart,
            DateTime? dateEnd)
        {
            string sql = "select * from mtd_production_schedule where floor = @floor and ownerId = @ownerId and date between @dateStart and @dateEnd order by sn asc, model desc, lcmProdId asc, date asc ; ";

            var dao = _dbHelper.ExecuteQuery<MTDProductionScheduleDao>(sql, new
            {
                floor = floor,
                ownerId = ownerId,
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
where date between @DateStart and @DateEnd and floor = @Floor and ownerId = @OwnerId;";

            var dao = _dbHelper.ExecuteQuery<MTDProductionScheduleDao>(_sql, new
            {
                Floor = floor,
                OwnerId = owner,
                DateStart = dateStart,
                DateEnd = dateEnd,
                year = dateStart.Year,
                month = dateStart.Month
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
where DATEPART(YEAR, date) = @Year and DATEPART(MONTH, date) = @Month and date != @DateStart and floor = @Floor and ownerId = @OwnerId ; ";

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
,[lcmProdId]
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
,@lcmProdId
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

        public int DeleteSchedule(int ownerId)
        {
            string sql = @"
 Delete [dbo].[mtd_production_schedule] where ownerId = @OwnerId; ";

            var dao = _dbHelper.ExecuteNonQuery(sql, new
            {
                OwnerId = ownerId
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

    }
}
