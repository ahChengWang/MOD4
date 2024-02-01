using MOD4.Web.Enum;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.Repostory
{
    public class LightingLogRepository : BaseRepository, ILightingLogRepository
    {

        public List<LightingLogDao> SelectByConditions(List<int> snList = null, DateTime? startDate = null, DateTime? endDate = null, string panelId = "", LightingCategoryEnum? categoryId = null)
        {
            string sql = "select * from lighting_log where 1=1 ";

            if (snList != null && snList.Any())
                sql += " and panelSn in @PanelSn ";
            if (startDate != null && endDate != null)
                sql += " and panelDate between @StartDate and @EndDate ";
            if (!string.IsNullOrEmpty(panelId))
                sql += " and panelId = @PanelId ";
            if (categoryId != null)
                sql += " and categoryId = @CategoryId ";


            var dao = _dbHelper.ExecuteQuery<LightingLogDao>(sql, new
            {
                PanelSn = snList,
                StartDate = startDate,
                EndDate = endDate,
                PanelId = panelId,
                CategoryId = categoryId
            });

            return dao;
        }

        public int InsertLightingLog(List<LightingLogDao> daoList)
        {
            string sql = @"INSERT INTO [carUX_2f].[dbo].[lighting_log]
([categoryId]
,[panelId]
,[panelDate]
,[statusId]
,[defectCatgId]
,[defectCode]
,[createDate]
,[createUser]
,[floor]
,[updateDate]
,[updateUser])
VALUES
(@categoryId
,@panelId
,@panelDate
,@statusId
,@defectCatgId
,@defectCode
,@createDate
,@createUser
,@floor
,@updateDate
,@updateUser); ";

            var dao = _dbHelper.ExecuteNonQuery(sql, daoList);

            return dao;
        }

        public int UpdateRWLog(List<LightingLogDao> updList)
        {
            string sql = @"update [carUX_2f].[dbo].[lighting_log] set 
                statusId = @statusId ,
                defectCatgId = @defectCatgId ,
                defectCode = @defectCode ,
                updateDate = @updateDate ,
                updateUser = @updateUser   
                where panelSn = @panelSn ";

            var response = _dbHelper.ExecuteNonQuery(sql, updList);

            return response;
        }

    }
}
