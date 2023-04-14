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
            DateTime? dateStart
            , DateTime? dateEnd)
        {
            string sql = "select * from mtd_production_schedule where floor = @floor and date between @dateStart and @dateEnd order by sn asc, model desc, productName asc, date asc ; ";

            var dao = _dbHelper.ExecuteQuery<MTDProductionScheduleDao>(sql, new
            {
                floor = floor,
                dateStart = dateStart,
                dateEnd = dateEnd
            });

            return dao;
        }

        public List<MTDProductionScheduleDao> SelectMonthPlanQty(string year, string month, int floor)
        {
            //string sql = "select process,model,productName,SUM(value)'MonthPlan' from mtd_production_schedule where DATEPART(YEAR, date) = @Year and DATEPART(MONTH, date) = @Month group by process,model,productName; ";
            string sql = "select * from mtd_production_schedule where DATEPART(YEAR, date) = @Year and DATEPART(MONTH, date) = @Month and floor = @Floor ; ";

            var dao = _dbHelper.ExecuteQuery<MTDProductionScheduleDao>(sql, new
            {
                Year = year,
                Month = month,
                Floor = floor
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

        public MTDScheduleUpdateHistoryDao SelectHistory(int floor)
        {
            string sql = "select TOP 1 * from mtd_schedule_update_history where floor = @Floor order by updateTime desc; ";

            var dao = _dbHelper.ExecuteQuery<MTDScheduleUpdateHistoryDao>(sql, new
            {
                Floor = floor
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
,@updateUser
,@updateTime
); ";

            var dao = _dbHelper.ExecuteNonQuery(sql, insMTDSchedule);

            return dao;
        }

        public int DeleteSchedule()
        {
            string sql = @"
 Truncate table [dbo].[mtd_production_schedule]; ";

            var dao = _dbHelper.ExecuteNonQuery(sql);

            return dao;
        }

        public int InsertScheduleHistory(MTDScheduleUpdateHistoryDao insHisDao)
        {
            string sql = @" INSERT INTO [dbo].[mtd_schedule_update_history]
([fileName]
,[floor]
,[updateUser]
,[updateTime])
VALUES
(@fileName
,@floor
,@updateUser
,@updateTime);";

            var dao = _dbHelper.ExecuteNonQuery(sql, insHisDao);

            return dao;
        }

    }
}
