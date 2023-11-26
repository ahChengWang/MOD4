using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface IAlarmXmlRepository
    {
        List<string> SelectToolList();
        List<AlarmXmlDao> SelectByConditions(string date, List<string> toolIdList);
        List<AlarmXmlDao> SelectUnrepaired();
        List<AlarmXmlDao> SelectDayTopRepaired(string mfgDay);
        List<ProdXmlTmpDao> SelectProdInfo(string mfgDay);
        List<AlarmXmlDao> SelectForMTD(DateTime date, List<string> toolIdList, List<string> prodList);
    }
}
