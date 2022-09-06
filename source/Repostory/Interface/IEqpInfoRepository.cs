using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface IEqpInfoRepository
    {
        List<string> SelectToolList(string date);
        List<EqpInfoDao> SelectByConditions(string date, List<string> equipmentList, bool isDefault);
        EqpInfoDao SelectEqpinfoByConditions(int sn);
        int UpdateEqpinfo(EqpInfoDao updDao);
    }
}
