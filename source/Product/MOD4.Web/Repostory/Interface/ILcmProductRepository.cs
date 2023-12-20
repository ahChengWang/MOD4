using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface ILcmProductRepository
    {
        List<LcmProductDao> SelectByConditions(List<int> snList = null, List<string> prodNoList = null);

        LcmProductDao Insert(LcmProductDao prodDao);
    }
}
