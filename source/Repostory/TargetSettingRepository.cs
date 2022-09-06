using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.Repostory
{
    public class TargetSettingRepository : BaseRepository, ITargetSettingRepository
    {

        public List<TargetSettingDao> SelectByConditions(List<string> nodeList)
        {
            string sql = "select * from Target_Setting where 1=1 ";

            if (nodeList != null && nodeList.Any())
            {
                sql += " and Node in @Node ";
            }

            var dao = _dbHelper.ExecuteQuery<TargetSettingDao>(sql, new
            {
                Node = nodeList
            });

            return dao;
        }

        public int Update(List<TargetSettingDao> updSettingList)
        {
            string sql = @"UPDATE [dbo].[Target_Setting]
   SET [Time0730] = @Time0730
      ,[Time0830] = @Time0830
      ,[Time0930] = @Time0930
      ,[Time1030] = @Time1030
      ,[Time1130] = @Time1130
      ,[Time1230] = @Time1230
      ,[Time1330] = @Time1330
      ,[Time1430] = @Time1430
      ,[Time1530] = @Time1530
      ,[Time1630] = @Time1630
      ,[Time1730] = @Time1730
      ,[Time1830] = @Time1830
      ,[Time1930] = @Time1930
      ,[Time2030] = @Time2030
      ,[Time2130] = @Time2130
      ,[Time2230] = @Time2230
      ,[Time2330] = @Time2330
      ,[Time0030] = @Time0030
      ,[Time0130] = @Time0130
      ,[Time0230] = @Time0230
      ,[Time0330] = @Time0330
      ,[Time0430] = @Time0430
      ,[Time0530] = @Time0530
      ,[Time0630] = @Time0630
      ,[UpdateUser] = @UpdateUser
      ,[UpdateTime] = @UpdateTime
 WHERE [Node] = @Node ";

            var dao = _dbHelper.ExecuteNonQuery(sql, updSettingList);

            return dao;
        }
    }
}
