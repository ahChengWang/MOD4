using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface IDemandsRepository
    {
        List<DemandsDao> SelectByConditions(string orderSn = "", string orderNo = "");
        int Insert(DemandsDao insDemands);
    }
}
