using MOD4.Web.Enum;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.Repostory
{
    public class SAPMaterialRepository : BaseRepository, ISAPMaterialRepository
    {

        public List<SAPWorkOrderDao> SelectSAPwoByConditions(List<string> workOrder = null, string prodNo = "", List<string> sapNode = null, List<string> matrlNo = null)
        {
            try
            {
                string sql = @"select sap.*,def.useNode from sap_work_order sap 
left join definition_material def 
on sap.materialNo = def.matlNo where 1=1 ";

                if (workOrder != null && workOrder.Any())
                    sql += " and sap.[order] IN @Order ";

                if (!string.IsNullOrEmpty(prodNo))
                    sql += " and sap.prod=@Prod ";

                if (sapNode != null && sapNode.Any())
                    sql += " and sap.sapNode IN @SAPNode ";

                if (matrlNo != null && matrlNo.Any())
                    sql += " and sap.materialNo IN @MaterialNo ";

                var dao = _dbHelper.ExecuteQuery<SAPWorkOrderDao>(sql, new
                {
                    Order = workOrder,
                    Prod = prodNo,
                    SAPNode = sapNode,
                    MaterialNo = matrlNo
                });

                return dao;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<DefinitionMaterialDao> SelectAllMatlDef(MatlCodeTypeEnum? codeTypeId = null)
        {
            try
            {
                string sql = "select * from [dbo].[definition_material] where 1=1 ";

                if (codeTypeId != null)
                    sql += " and codeTypeId = @CodeTypeId ";

                var dao = _dbHelper.ExecuteQuery<DefinitionMaterialDao>(sql, new
                {
                    CodeTypeId = codeTypeId
                });

                return dao;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<MaterialSettingDao> SelectMatlAllSetting(List<int> matlSnList)
        {
            try
            {
                string sql = "select * from [dbo].material_setting where 1=1 ";

                if (matlSnList != null && matlSnList.Any())
                {
                    sql += " and matlSn IN @MatlSn ";
                }

                var dao = _dbHelper.ExecuteQuery<MaterialSettingDao>(sql, new
                {
                    MatlSn = matlSnList
                });

                return dao;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdateMatlSetting(List<MaterialSettingDao> updDao)
        {
            try
            {
                string sql = @"UPDATE [dbo].[material_setting] SET [lossRate] = @lossRate WHERE [matlSn]=@matlSn ;";

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
([matlSn]
,[lossRate]
,[UpdateUser]
,[UpdateTime])
VALUES
(@matlSn
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

        public int TruncateSAPwo()
        {
            string sql = @"Truncate table [dbo].[sap_work_order]; ";

            var dao = _dbHelper.ExecuteNonQuery(sql);

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
,[matlShortName]
,[woType]
,[woComment]
,[mesScrap]
,[icScrap])
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
,@matlShortName
,@woType
,@woComment
,@mesScrap
,@icScrap); ";

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
([matlSn]
,[lossRate]
,[updateUser]
,[updateTime])
VALUES
(@matlSn
,@lossRate
,@updateUser
,@updateTime); ";

            var dao = _dbHelper.ExecuteNonQuery(sql, insMTDSchedule);

            return dao;
        }

        public int DeleteMatlSetting(MatlCodeTypeEnum codeTypeId)
        {
            string sql = @" Delete matl
 from material_setting matl
 join definition_material def
   on matl.matlSn = def.sn  
where def.codeTypeId = @CodeTypeId ;";

            var dao = _dbHelper.ExecuteNonQuery(sql, new
            {
                CodeTypeId = codeTypeId
            });

            return dao;
        }
    }
}
