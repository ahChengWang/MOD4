using MOD4.Web.Enum;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.Repostory
{
    public class TakeBackWTRepository : BaseRepository, ITakeBackWTRepository
    {

        public List<TakeBackWTDao> SelectByConditions(DateTime processDate, List<WTCategoryEnum> wtCatgIdList = null)
        {
            string sql = "select * from take_back_wt where processDate = @ProcessDate  ";

            if (wtCatgIdList != null && wtCatgIdList.Any())
                sql += " and wtCategoryId in @WTCategoryId ";

            var dao = _dbHelper.ExecuteQuery<TakeBackWTDao>(sql, new
            {
                ProcessDate = processDate,
                WTCategoryId = wtCatgIdList
            });

            return dao;
        }

        public List<TakeBackWTDao> SelectMonthlyData(int procYear, int procMonth, List<WTCategoryEnum> wtCatgIdList)
        {
            string sql = "select * from take_back_wt where wtCategoryId in @wtCategoryId and DATEPART(YEAR,processDate) = @ProcYear and DATEPART(MONTH,processDate) = @ProcMon; ";

            var dao = _dbHelper.ExecuteQuery<TakeBackWTDao>(sql, new
            {
                wtCategoryId = wtCatgIdList,
                ProcYear = procYear,
                ProcMon = procMonth
            });

            return dao;
        }

        public List<TakeBackWTDetailDao> SelectDetailByConditions(List<int> takeBackWtSn)
        {
            string sql = "select * from take_back_wt_detail where takeBackWtSn in @TakeBackWtSn ; ";

            var dao = _dbHelper.ExecuteQuery<TakeBackWTDetailDao>(sql, new
            {
                TakeBackWtSn = takeBackWtSn
            });

            return dao;
        }

        public List<TakeBackWTAttendanceDao> SelectAttendanceByConditions(List<int> takeBackWtSn)
        {
            string sql = "select * from take_back_wt_attendance where takeBackWtSn in @TakeBackWtSn ; ";

            var dao = _dbHelper.ExecuteQuery<TakeBackWTAttendanceDao>(sql, new
            {
                TakeBackWtSn = takeBackWtSn
            });

            return dao;
        }

        public int InsertTakeBackWT(TakeBackWTDao takeBackWt)
        {
            string sql = @"INSERT INTO [dbo].[take_back_wt]
           ([processDate]
           ,[wtCategoryId]
           ,[bondTakeBack]
           ,[fogTakeBack]
           ,[lamTakeBack]
           ,[assyTakeBack]
           ,[cdpTakeBack]
           ,[ttlTakeBack]
           ,[takeBackPercent]
           ,[createUser]
           ,[createTime])
     VALUES(@processDate
           ,@wtCategoryId
           ,@bondTakeBack
           ,@fogTakeBack
           ,@lamTakeBack
           ,@assyTakeBack
           ,@cdpTakeBack
           ,@ttlTakeBack
           ,@takeBackPercent
           ,@createUser
           ,@createTime); ";

            var dao = _dbHelper.ExecuteNonQuery(sql, takeBackWt);

            return dao;
        }

        public int UpdateTakeBackWT(TakeBackWTDao takeBackWt)
        {
            string sql = @"UPDATE [dbo].[take_back_wt]
   SET [processDate] = @processDate
      ,[wtCategoryId] = @wtCategoryId
      ,[bondTakeBack] = @bondTakeBack
      ,[fogTakeBack] = @fogTakeBack
      ,[lamTakeBack] = @lamTakeBack
      ,[assyTakeBack] = @assyTakeBack
      ,[cdpTakeBack] = @cdpTakeBack
      ,[ttlTakeBack] = @ttlTakeBack
      ,[takeBackPercent] = @takeBackPercent
      ,[updateUser] = @updateUser
      ,[updateTime] = @updateTime 
 WHERE sn = @Sn; ";

            var dao = _dbHelper.ExecuteNonQuery(sql, takeBackWt);

            return dao;
        }

        public int InsertTakeBackWTDetail(List<TakeBackWTDetailDao> takeBackWtDetailList)
        {
            string sql = @"INSERT INTO [dbo].[take_back_wt_detail]
           ([takeBackWtSn]
           ,[processId]
           ,[eqId]
           ,[prod]
           ,[ieStandard]
           ,[ieTT]
           ,[ieWT]
           ,[passQty]
           ,[takeBackWT]
           ,[createUser]
           ,[createTime])
     VALUES(@takeBackWtSn
           ,@processId
           ,@eqId
           ,@prod
           ,@ieStandard
           ,@ieTT
           ,@ieWT
           ,@passQty
           ,@takeBackWT
           ,@createUser
           ,@createTime); ";

            var dao = _dbHelper.ExecuteNonQuery(sql, takeBackWtDetailList);

            return dao;
        }

        public int DeleteTakeBackWTInfo(int takeBackWTSn)
        {
            string sql = @" delete take_back_wt_detail where takeBackWtSn = @TakeBackWtSn; 
delete take_back_wt_attendance where takeBackWtSn = @TakeBackWtSn; ";

            var dao = _dbHelper.ExecuteNonQuery(sql, new 
            {
                TakeBackWtSn = takeBackWTSn
            });

            return dao;
        }

        public int InsertTakeBackWTAttendance(List<TakeBackWTAttendanceDao> takeBackWtAttendanceList)
        {
            string sql = @"INSERT INTO [dbo].[take_back_wt_attendance]
           ([takeBackWtSn]
           ,[countryId]
           ,[shouldPresentCnt]
           ,[overTimeCnt]
           ,[acceptSupCnt]
           ,[haveDayOffCnt]
           ,[offCnt]
           ,[support]
           ,[presentCnt]
           ,[totalWorkTime]
           ,[createUser]
           ,[createTime])
     VALUES(@takeBackWtSn
           ,@countryId
           ,@shouldPresentCnt
           ,@overTimeCnt
           ,@acceptSupCnt
           ,@haveDayOffCnt
           ,@offCnt
           ,@support
           ,@presentCnt
           ,@totalWorkTime
           ,@createUser
           ,@createTime); ";

            var dao = _dbHelper.ExecuteNonQuery(sql, takeBackWtAttendanceList);

            return dao;
        }
    }
}
