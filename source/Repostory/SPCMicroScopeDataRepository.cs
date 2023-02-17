using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public class SPCMicroScopeDataRepository : BaseRepository, ISPCMicroScopeDataRepository
    {

        public List<SPCMicroScopeDataDao> SelectByConditions(string equip, DateTime startDate, DateTime endDate, string prodId, string dataGroup)
        {
            string sql = "select * from [CarUXReport].[dbo].SPC_MicroScope_Data where 1=1 ";
            string subSql = "select TOP 1 * from [CarUXReport].[dbo].SPC_MicroScope_Data where 1=1 ";

            if (!string.IsNullOrEmpty(equip))
            {
                sql += " and EquipmentId=@EquipmentId ";
                subSql += " and EquipmentId=@EquipmentId ";
            }

            if (!string.IsNullOrEmpty(prodId))
            {
                sql += " and ProductId=@ProductId ";
                subSql += " and ProductId=@ProductId ";
            }

            if (!string.IsNullOrEmpty(dataGroup))
            {
                sql += " and DataGroup=@DataGroup ";
                subSql += " and DataGroup=@DataGroup ";
            }

            sql += " and MeasureDate >= @StartDate and MeasureDate <= @EndDate ";
            subSql += " and MeasureDate < @StartDate ";

            sql += $"union {subSql} Order by MeasureDate asc ";

            var dao = _dbHelper.ExecuteQuery<SPCMicroScopeDataDao>(sql, new
            {
                EquipmentId = equip,
                ProductId = prodId,
                DataGroup = dataGroup,
                StartDate = startDate,
                EndDate = endDate
            });

            return dao;
        }
    }
}
