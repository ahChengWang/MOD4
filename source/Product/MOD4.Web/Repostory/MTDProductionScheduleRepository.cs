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
            int floor = 0,
            bool? isMass = null,
            MTDCategoryEnum? mtdCategoryId = null,
            DateTime? dateStart = null,
            DateTime? dateEnd = null,
            int prodId = 0)
        {
            string _sql = "select * from mtd_production_schedule where 1 = 1 ";

            if (floor != 0)
                _sql += " and floor = @Floor ";
            if (isMass != null)
                _sql += " and isMass = @IsMass ";
            if (mtdCategoryId != null)
                _sql += " and mtdCategoryId = @MTDCategoryId ";
            if (prodId != 0)
                _sql += " and lcmProdId = @LcmProdId ";
            if (dateStart != null)
                _sql += " and date >= @StartDate ";
            if (dateEnd != null)
                _sql += " and date <= @EndDate ";

            var dao = _dbHelper.ExecuteQuery<MTDProductionScheduleDao>(_sql, new
            {
                Floor = floor,
                IsMass = isMass,
                MTDCategoryId = mtdCategoryId,
                StartDate = dateStart,
                EndDate = dateEnd,
                LcmProdId = prodId
            });

            return dao;
        }


        public List<MTDProductionScheduleDao> SelectForMTDDashboard(
            int floor,
            bool isMass,
            DateTime dateStart, 
            DateTime dateEnd)
        {
            string _sql = @"select mtdAllPlan.* from mtd_production_schedule mtdAllPlan
  join (select productName,process from mtd_production_schedule 
         where DATEPART(YEAR, date) = @year and DATEPART(MONTH, date) = @month and floor = @floor and isMass = @IsMass 
		 group by productName ,process  
		 having SUM(value) !=0) mtdNoPlan 
    on mtdAllPlan.productName = mtdNoPlan.productName 
   and mtdAllPlan.process = mtdNoPlan.process 
 where mtdAllPlan.floor = @floor and isMass = @IsMass and date between @dateStart and @dateEnd 
 order by mtdAllPlan.sn asc, mtdAllPlan.model desc, mtdAllPlan.productName asc, date asc ;";

            var dao = _dbHelper.ExecuteQuery<MTDProductionScheduleDao>(_sql, new
            {
                Floor = floor,
                IsMass = isMass,
                dateStart = dateStart,
                dateEnd = dateEnd,
                year = dateStart.Year,
                month = dateStart.Month
            });

            return dao;
        }

        public List<MTDProductionScheduleDao> SelectMonthPlanQty(string year, string month, int floor, bool isMass)
        {            
            string sql = "select * from mtd_production_schedule where DATEPART(YEAR, date) = @Year and DATEPART(MONTH, date) = @Month and floor = @Floor and isMass = @IsMass ; ";

            var dao = _dbHelper.ExecuteQuery<MTDProductionScheduleDao>(sql, new
            {
                Year = year,
                Month = month,
                Floor = floor,
                IsMass = isMass
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
([process]
,[mtdCategoryId]
,[node]
,[model]
,[lcmProdId]
,[date]
,[qty]
,[floor]
,[isMass]
,[updateUser]
,[updateTime])
VALUES
(@process
,@mtdCategoryId
,@node
,@model
,@lcmProdId
,@date
,@qty
,@floor
,@isMass
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

        public int UpdateSchedule(MTDProductionScheduleDao updMTDProdSchedule)
        {
            string sql = @" update mtd_production_schedule 
  set lcmProdId = @LcmProdId,
      qty = @Qty,
      isMass = @IsMass 
where sn = @Sn ;";

            var dao = _dbHelper.ExecuteNonQuery(sql, updMTDProdSchedule);

            return dao;
        }

    }
}
