using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface IDemandsRepository
    {
        List<DemandsDao> SelectByConditions(DateTime dateStart, DateTime dateEnd, string orderSn = "", string orderNo = "", string[] categoryArray = null, string[] statusArray = null);

        DemandsDao SelectDetail(int orderSn, string orderNo = "");

        int Insert(DemandsDao insDemands);

        int UpdateToProcess(DemandsDao updDao);

        int UpdateToReject(DemandsDao updDao);

        int UpdateToComplete(DemandsDao updDao);

        int UpdateToPending(DemandsDao updDao);

        int UpdateToCancel(DemandsDao updDao);
    }
}
