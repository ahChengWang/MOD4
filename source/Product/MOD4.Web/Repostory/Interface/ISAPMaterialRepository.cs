using MOD4.Web.Enum;
using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface ISAPMaterialRepository
    {
        List<SAPWorkOrderDao> SelectSAPwoByConditions(List<string> workOrder = null, string prodNo = "", List<string> sapNode = null, List<string> matrlNo = null);

        List<DefinitionMaterialDao> SelectAllMatlDef(MatlCodeTypeEnum? codeTypeId = null);

        int InsertSAPwo(List<SAPWorkOrderDao> insSAPMatlList);

        int UpdateSAPwo(List<SAPWorkOrderDao> updSAPwo);

        int TruncateSAPwo();

        List<MaterialSettingDao> SelectMatlAllSetting(List<int> matlSnList);

        int UpdateMatlSetting(List<MaterialSettingDao> updDao);

        int InsertMatlSettingHistory(List<MaterialSettingHistoryDao> insMatlHis);

        int InsertMatlSetting(List<MaterialSettingDao> insMTDSchedule);

        int DeleteMatlSetting(MatlCodeTypeEnum codeTypeId);
        
    }
}