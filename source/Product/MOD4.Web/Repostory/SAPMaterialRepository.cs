using MOD4.Web.Enum;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public class SAPMaterialRepository : BaseRepository, ISAPMaterialRepository
    {
        public List<MaterialSettingDao> SelectMatlAllSetting(MatlCodeTypeEnum? codeTypId = MatlCodeTypeEnum.Code5)
        {
            string sql = "select * from [dbo].material_setting where 1=1 ";

            if (codeTypId != null)
            {
                sql += " and codeTypeId=@CodeTypeId ";
            }

            var dao = _dbHelper.ExecuteQuery<MaterialSettingDao>(sql,new {
                CodeTypeId = codeTypId
            });

            return dao;
        }

        public int UpdateMatlSetting(List<MaterialSettingDao> updDao)
        {
            try
            {
                string sql = @"UPDATE [dbo].[material_setting] SET [lossRate] = @lossRate WHERE [matlNo]=@matlNo ;";

                var dao = _dbHelper.ExecuteNonQuery(sql, updDao);

                return dao;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int InsertMatlSettingHistory(List<MaterialSettingHistoryDao> insMatlHis)
        {
            try
            {
                string sql = @"
INSERT INTO [dbo].[material_setting_history]
([matlNo]
,[lossRate]
,[UpdateUser]
,[UpdateTime])
VALUES
(@matlNo
,@lossRate
,@UpdateUser
,@UpdateTime); ";

                var dao = _dbHelper.ExecuteNonQuery(sql, insMatlHis);

                return dao;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SAPWorkOrderDao> SelectSAPwoByConditions()
        {
            string sql = "select * from [dbo].sap_work_order where 1=1 ";

            var dao = _dbHelper.ExecuteQuery<SAPWorkOrderDao>(sql);

            return dao;
        }

        public int UpdateSAPwo(List<SAPWorkOrderDao> updSAPwo)
        {
            string sql = @"UPDATE [dbo].[sap_work_order]
   SET prodQty = @prodQty
      ,storageQty = @storageQty
      ,startDate = @startDate
      ,finishDate = @finishDate
      ,unit = @unit
      ,exptQty = @exptQty
      ,disburseQty = @disburseQty
      ,returnQty = @returnQty
      ,actStorageQty = @actStorageQty
      ,scrapQty = @scrapQty
      ,diffQty = @diffQty
      ,diffRate = @diffRate
      ,overDisburse = @overDisburse
      ,diffDisburse = @diffDisburse
      ,woPremiumOut = @woPremiumOut
      ,cantNegative = @cantNegative
      ,opiWOStatus = @opiWOStatus
      ,woType = @woType 
 WHERE [sn]=@sn ;";

            var dao = _dbHelper.ExecuteNonQuery(sql, updSAPwo);

            return dao;
        }

        public int InsertSAPwo(List<SAPWorkOrderDao> insSAPMatlList)
        {
            try
            {
                string sql = @"
INSERT INTO [dbo].[sap_work_order]
([order]
,[prod]
,[materialSpec]
,[materialNo]
,[materialName]
,[sapNode]
,[dept]
,[prodQty]
,[storageQty]
,[startDate]
,[finishDate]
,[unit]
,[exptQty]
,[disburseQty]
,[returnQty]
,[actStorageQty]
,[scrapQty]
,[diffQty]
,[diffRate]
,[overDisburse]
,[diffDisburse]
,[woPremiumOut]
,[cantNegative]
,[opiWOStatus]
,[woType])
VALUES
(@order
,@prod
,@materialSpec
,@materialNo
,@materialName
,@sapNode
,@dept
,@prodQty
,@storageQty
,@startDate
,@finishDate
,@unit
,@exptQty
,@disburseQty
,@returnQty
,@actStorageQty
,@scrapQty
,@diffQty
,@diffRate
,@overDisburse
,@diffDisburse
,@woPremiumOut
,@cantNegative
,@opiWOStatus
,@woType); ";

                var dao = _dbHelper.ExecuteNonQuery(sql, insSAPMatlList);

                return dao;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int InsertMatlSetting(List<MaterialSettingDao> insMTDSchedule)
        {
            string sql = @"
INSERT INTO [dbo].[material_setting]
([matlNo]
,[codeTypeId]
,[matlName]
,[matlCatg]
,[useNode]
,[lossRate]
,[updateUser]
,[updateTime])
VALUES
(@matlNo
,@codeTypeId
,@matlName
,@matlCatg
,@useNode
,@lossRate
,@updateUser
,@updateTime); ";

            var dao = _dbHelper.ExecuteNonQuery(sql, insMTDSchedule);

            return dao;
        }

        public int DeleteMatlSetting(MatlCodeTypeEnum codeTypeId)
        {
            string sql = @"
 Delete [dbo].[material_setting] where codeTypeId = @CodeTypeId ; ";

            var dao = _dbHelper.ExecuteNonQuery(sql, new
            {
                CodeTypeId = codeTypeId
            });

            return dao;
        }
    }
}
