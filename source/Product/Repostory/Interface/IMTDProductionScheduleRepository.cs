using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface IMTDProductionScheduleRepository
    {
        List<MTDProductionScheduleDao> SelectByConditions(int floor, DateTime? dateStart, DateTime? dateEnd);

        List<MTDProductionScheduleDao> SelectMonthPlanQty(string year, string month, int floor);

        MTDProductionScheduleDao SelectNewOrderSn(string orderNo);

        MTDScheduleUpdateHistoryDao SelectHistory(int floor);

        int InsertSchedule(List<MTDProductionScheduleDao> insMTDSchedule);

        int DeleteSchedule();

        int InsertScheduleHistory(MTDScheduleUpdateHistoryDao insHisDao);
    }
}
