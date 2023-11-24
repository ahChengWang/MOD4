using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public class MonitorSettingRepository : BaseRepository, IMonitorSettingRepository
    {

        public List<MonitorSettingDao> SelectSettings()
        {
            try
            {
                string sql = "select * from monitor_setting ; ";

                var dao = _dbHelper.ExecuteQuery<MonitorSettingDao>(sql);

                return dao;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int Update(List<MonitorSettingDao> updMonitorList)
        {
            try
            {
                string sql = @"
UPDATE [dbo].[monitor_setting]
   SET [defTopRate] = @defTopRate
      ,[defLeftRate] = @defLeftRate
      ,[defWidth] = @defWidth
      ,[defHeight] = @defHeight
      ,[border] = @border
      ,[background] = @background
      ,[updateTime] = @updateTime
      ,[updateUser] = @updateUser
 WHERE [node]=@node and [eqNumber]=@eqNumber ";

                var dao = _dbHelper.ExecuteNonQuery(sql, updMonitorList);

                return dao;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public int Insert(List<MonitorSettingDao> insSettingList)
        {
            try
            {
                string sql = $@"INSERT INTO [dbo].[monitor_setting]
           ([node]
           ,[eqNumber]
           ,[defTopRate]
           ,[defLeftRate]
           ,[defWidth]
           ,[defHeight]
           ,[border]
           ,[background]
           ,[floor]
           ,[updateTime]
           ,[updateUser])
     VALUES
           (@node
           ,@eqNumber
           ,@defTopRate
           ,@defLeftRate
           ,@defWidth
           ,@defHeight
           ,@border
           ,@background
           ,@floor
           ,@updateTime
           ,@updateUser) ";

                var dao = _dbHelper.ExecuteNonQuery(sql, insSettingList);

                return dao;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int Delete()
        {
            try
            {
                string sql = $@" Truncate table monitor_setting ;";

                var dao = _dbHelper.ExecuteNonQuery(sql);

                return dao;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
