﻿using MOD4.Web.Enum;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.Repostory
{
    public class MTDProductionScheduleRepository : BaseRepository, IMTDProductionScheduleRepository
    {
        public List<MTDProductionScheduleDao> SelectByConditions(
            int sn = 0,
            int floor = 0,
            bool? isMass = null,
            MTDCategoryEnum? mtdCategoryId = null,
            DateTime? dateStart = null,
            DateTime? dateEnd = null,
            int prodId = 0)
        {
            string _sql = "select * from mtd_production_schedule where 1 = 1 ";

            if (sn != 0)
                _sql += " and sn = @Sn ";
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
                Sn = sn,
                Floor = floor,
                IsMass = isMass,
                MTDCategoryId = mtdCategoryId,
                StartDate = dateStart,
                EndDate = dateEnd,
                LcmProdId = prodId
            });

            return dao;
        }


        public List<MTDProductionScheduleDao> SelectMTDTodayPlan(
            int floor,
            bool isMass,
            DateTime dateStart, 
            DateTime dateEnd)
        {
            string _sql = @" select * from mtd_production_schedule 
where date between @DateStart and @DateEnd and floor = @Floor and isMass = @IsMass;";

            var dao = _dbHelper.ExecuteQuery<MTDProductionScheduleDao>(_sql, new
            {
                Floor = floor,
                IsMass = isMass,
                DateStart = dateStart,
                DateEnd = dateEnd,
                year = dateStart.Year,
                month = dateStart.Month
            });

            return dao;
        }

        public List<MTDProductionScheduleDao> SelectMTDMonHavePlan(
            int floor,
            bool isMass,
            DateTime dateStart,
            DateTime dateEnd)
        {
            string _sql = @" select * from mtd_production_schedule 
where DATEPART(YEAR, date) = @Year and DATEPART(MONTH, date) = @Month and date != @DateStart and floor = @Floor and isMass = @IsMass ; ";

            var dao = _dbHelper.ExecuteQuery<MTDProductionScheduleDao>(_sql, new
            {
                Floor = floor,
                IsMass = isMass,
                DateStart = dateStart,
                DateEnd = dateEnd,
                Year = dateStart.Year,
                Month = dateStart.Month
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

        public int DeleteSchedule(int sn)
        {
            string sql = @"
 Delete [dbo].[mtd_production_schedule] where sn = @Sn; ";

            var dao = _dbHelper.ExecuteNonQuery(sql,new {
                Sn = sn
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
    set date = @Date,
        lcmProdId = @LcmProdId,
        qty = @Qty,
        isMass = @IsMass 
where sn = @Sn ;";

            var dao = _dbHelper.ExecuteNonQuery(sql, updMTDProdSchedule);

            return dao;
        }

    }
}
