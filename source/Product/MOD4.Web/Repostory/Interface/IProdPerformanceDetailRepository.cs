using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface IProdPerformanceDetailRepository
    {
        List<ProdPerformanceDetailDao> SelectByConditions(DateTime srcStartDate, DateTime endStartDate);

        List<ProdPerformanceDetailDao> SelectByEqNumber(string eqNo, DateTime srcStartDate, DateTime endStartDate);
    }
}