using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface IAlarmXmlRepository
    {
        List<string> SelectToolList();
        List<AlarmXmlDao> SelectByConditions(string date, List<string> toolIdList, int sn, bool isRepaired, List<string> statusList, bool showAuto);
        List<AlarmXmlDao> SelectUnrepaired();
        List<AlarmXmlDao> SelectRepaired(string mfgDay);
        List<AlarmXmlDao> SelectDayTopRepaired(string mfgDay);
        List<ProdXmlTmpDao> SelectProdInfo(string mfgDay);
        List<AlarmXmlDao> SelectForMTD(DateTime date, List<string> toolIdList, List<string> prodList);
        int UpdateAlarmInfo(AlarmXmlDao updDao);
        int UpdateAlarmInfoByENG(AlarmXmlDao updDao);
        int InsertAlarmXml(AlarmXmlDao updDao);
    }
}
