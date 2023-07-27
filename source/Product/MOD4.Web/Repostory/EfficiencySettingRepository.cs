using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.Repostory
{
    public class EfficiencySettingRepository : BaseRepository, IEfficiencySettingRepository
    {

        public List<EfficiencySettingDao> SelectByConditions(List<int> prodList = null, int floor = 0)
        {
            string sql = "select * from efficiency_setting where floor=@Floor ";

            if (prodList != null && prodList.Any())
                sql += " and lcmProdSn in @ProdList ";

            var dao = _dbHelper.ExecuteQuery<EfficiencySettingDao>(sql, new
            {
                ProdList = prodList,
                Floor = floor
            });

            return dao;
        }

        public int Insert(List<EfficiencySettingDao> insList)
        {
            string sql = @"
INSERT INTO [dbo].[efficiency_setting]
           ([lcmProdSn]
           ,[processId]
           ,[node]
           ,[wt]
           ,[inlineEmps]
           ,[offlineEmps]
           ,[updateTime]
           ,[updateUser])
     VALUES
           (@lcmProdSn
           ,@processId
           ,@node
           ,@wt
           ,@inlineEmps
           ,@offlineEmps
           ,@updateTime
           ,@updateUser);
";
            var dao = _dbHelper.ExecuteNonQuery(sql, insList);

            return dao;
        }

        public int UpdateSetting(List<EfficiencySettingDao> updList)
        {
            string sql = $@"UPDATE [dbo].[efficiency_setting]
   SET [node] = @node
      ,[wt] = @wt
      ,[inlineEmps] = @inlineEmps
      ,[offlineEmps] = @offlineEmps
      ,[updateTime] = @updateTime
      ,[updateUser] = @updateUser
 WHERE [lcmProdSn]=@lcmProdSn and [processId]=@processId and [shift]=@shift ; ";

            var dao = _dbHelper.ExecuteNonQuery(sql, updList);

            return dao;
        }
    }
}
