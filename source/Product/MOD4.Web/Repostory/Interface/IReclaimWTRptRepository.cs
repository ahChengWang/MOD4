using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface IReclaimWTRptRepository
    {
        List<ReclaimWTRptDao> SelectByConditions(List<string> eqIdList, List<string> prodList);
    }
}