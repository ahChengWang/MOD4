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
            DateTime dateStart, 
            DateTime dateEnd)
        {
            string sql = @"select mtdAllPlan.* from mtd_production_schedule mtdAllPlan
  join (select productName,process from mtd_production_schedule 
         where DATEPART(YEAR, date) = @year and DATEPART(MONTH, date) = @month and floor = @floor and ownerId = @ownerId  
		 group by productName ,process  
		 having SUM(value) !=0) mtdNoPlan 
    on mtdAllPlan.productName = mtdNoPlan.productName 
   and mtdAllPlan.process = mtdNoPlan.process 
 where mtdAllPlan.floor = @floor and ownerId = @ownerId and date between @dateStart and @dateEnd 
 order by mtdAllPlan.sn asc, mtdAllPlan.model desc, mtdAllPlan.productName asc, date asc ;";

            var dao = _dbHelper.ExecuteQuery<MTDProductionScheduleDao>(sql, new
            {
                floor = floor,
                ownerId = ownerId,
                dateStart = dateStart,
                dateEnd = dateEnd,
                year = dateStart.Year,
                month = dateStart.Month
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
,[node]
,[model]
,[productName]
,[date]
,[value]
,[floor]
,[ownerId]
,[updateUser]
,[updateTime])
VALUES
(@sn
,@process
,@node
,@model
,@productName
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

            var dao = _dbHelper.ExecuteNonQuery(sql,new {
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
