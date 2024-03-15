using MOD4.Web.Enum;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface ITakeBackWTRepository
    {
        List<TakeBackWTDao> SelectByConditions(DateTime processDate, WTCategoryEnum wtCatgId = 0);

        List<TakeBackWTDetailDao> SelectDetailByConditions(List<int> takeBackWtSn);

        List<TakeBackWTAttendanceDao> SelectAttendanceByConditions(List<int> takeBackWtSn);

        int InsertTakeBackWT(TakeBackWTDao takeBackWt);

        int UpdateTakeBackWT(TakeBackWTDao takeBackWt);

        int InsertTakeBackWTDetail(List<TakeBackWTDetailDao> takeBackWtDetailList);

        int InsertTakeBackWTAttendance(List<TakeBackWTAttendanceDao> takeBackWtAttendanceList);

        int DeleteTakeBackWTInfo(int tackBackWTSn);
    }
}