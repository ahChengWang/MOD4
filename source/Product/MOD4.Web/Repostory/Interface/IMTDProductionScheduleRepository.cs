using MOD4.Web.Enum;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface IMTDProductionScheduleRepository
    {
        List<MTDProductionScheduleDao> SelectByConditions(int floor = 0, bool? isMass = null, MTDCategoryEnum? mtdCategoryId = null, DateTime? dateStart = null, DateTime? dateEnd = null);

        List<MTDProductionScheduleDao> SelectForMTDDashboard(int floor, bool isMass, DateTime dateStart, DateTime dateEnd);

        List<MTDProductionScheduleDao> SelectMonthPlanQty(string year, string month, int floor, bool isMass);

        MTDProductionScheduleDao SelectNewOrderSn(string orderNo);

        MTDScheduleUpdateHistoryDao SelectHistory(int floor, int ownerId);

        int InsertSchedule(List<MTDProductionScheduleDao> insMTDSchedule);

        int DeleteSchedule(int ownerId);

        int InsertScheduleHistory(MTDScheduleUpdateHistoryDao insHisDao);
    }
}
