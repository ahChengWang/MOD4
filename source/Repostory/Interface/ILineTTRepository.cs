using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface ILineTTRepository
    {
        List<LineTTDao> SelectByConditions(string mfgDay, List<string> lineList, string shift);
    }
}
