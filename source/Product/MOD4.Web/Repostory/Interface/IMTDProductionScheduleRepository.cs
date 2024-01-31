using MOD4.Web.Enum;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface IMTDProductionScheduleRepository
    {
        List<MTDProductionScheduleDao> SelectByConditions(int floor, int ownerId, int prodSn, DateTime? dateStart, DateTime? dateEnd);

        List<MTDProductionScheduleDao> SelectMTDTodayPlan(int floor, int owner, DateTime dateStart, DateTime dateEnd);

        List<MTDProductionScheduleDao> SelectMTDMonHavePlan(int floor, int owner, DateTime dateStart, DateTime dateEnd);

        List<MTDProductionScheduleDao> SelectMonthPlanQty(string year, string month, int floor, int ownerId);

        MTDProductionScheduleDao SelectNewOrderSn(string orderNo);

        MTDScheduleUpdateHistoryDao SelectHistory(int floor, int ownerId);

        int InsertSchedule(List<MTDProductionScheduleDao> insMTDSchedule);

        int UpdateMTDSchedule(List<MTDProductionScheduleDao> updMTDSchedule);

        int DeleteSchedule(int ownerId, DateTime endTime);

        int InsertScheduleHistory(MTDScheduleUpdateHistoryDao insHisDao);

        List<MTDScheduleSettingDao> SelectSettingByConditions(int prodSn = 0);

        int DeleteSetting(int prodSn);

        int InsertSetting(List<MTDScheduleSettingDao> insSettingDao);
    }
}
