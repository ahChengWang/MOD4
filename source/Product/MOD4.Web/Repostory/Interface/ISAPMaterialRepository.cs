using MOD4.Web.Enum;
using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface ISAPMaterialRepository
    {
        int InsertSAPwo(List<SAPWorkOrderDao> insSAPMatlList);
        List<MaterialSettingDao> SelectMatlAllSetting(MatlCodeTypeEnum? codeTypId = MatlCodeTypeEnum.Code5);
        int UpdateMatlSetting(List<MaterialSettingDao> updDao);
        int InsertMatlSettingHistory(List<MaterialSettingHistoryDao> insMatlHis);
        List<SAPWorkOrderDao> SelectSAPwoByConditions();
        int UpdateSAPwo(List<SAPWorkOrderDao> updSAPwo);

        int InsertMatlSetting(List<MaterialSettingDao> insMTDSchedule);

        int DeleteMatlSetting(MatlCodeTypeEnum codeTypeId);
    }
}