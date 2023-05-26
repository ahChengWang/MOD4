using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface ITargetSettingRepository
    {
        List<TargetSettingDao> SelectByConditions(List<int> prodSn, List<string> nodeList);

        int Update(List<TargetSettingDao> updSettingList);

        int Insert(List<TargetSettingDao> insSettingList);
    }
}
