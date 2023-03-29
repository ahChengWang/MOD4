using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface IAlarmXmlRepository
    {
        List<string> SelectToolList();
        List<AlarmXmlDao> SelectByConditions(string date, List<string> toolIdList);
    }
}
