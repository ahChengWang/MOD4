using MOD4.Web.Enum;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface ITakeBackWTRepository
    {
        List<TakeBackWTDao> SelectByConditions(DateTime processDate, List<WTCategoryEnum> wtCatgIdList = null);

        List<TakeBackWTDao> SelectMonthlyData(int procYear, int procMonth, List<WTCategoryEnum> wtCatgIdList);

        List<TakeBackWTDetailDao> SelectDetailByConditions(List<int> takeBackWtSn);

        List<TakeBackWTAttendanceDao> SelectAttendanceByConditions(List<int> takeBackWtSn);

        int InsertTakeBackWT(TakeBackWTDao takeBackWt);

        int UpdateTakeBackWT(TakeBackWTDao takeBackWt);

        int InsertTakeBackWTDetail(List<TakeBackWTDetailDao> takeBackWtDetailList);

        int InsertTakeBackWTAttendance(List<TakeBackWTAttendanceDao> takeBackWtAttendanceList);

        int DeleteTakeBackWTInfo(int tackBackWTSn);
    }
}