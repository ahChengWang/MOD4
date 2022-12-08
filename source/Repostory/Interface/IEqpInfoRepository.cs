using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface IEqpInfoRepository
    {
        List<string> SelectToolList(string date);
        List<EqpInfoDao> SelectByConditions(string date, List<string> equipmentList, bool isDefault, bool showAuto, List<int> prodSnList = null);
        EqpInfoDao SelectEqpinfoByConditions(int sn);
        int UpdateEqpinfoByPM(EqpInfoDao updDao);
        int UpdateEqpinfoByENG(EqpInfoDao updDao);
    }
}
