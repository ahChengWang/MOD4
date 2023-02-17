using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface ISPCMicroScopeDataRepository
    {
        List<SPCMicroScopeDataDao> SelectByConditions(string equip, DateTime startDate, DateTime endDate, string prodId, string dataGroup);
    }
}
