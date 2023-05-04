using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface IMESPermissionRepository
    {
        List<MESPermissionDao> SelectByConditions(DateTime? dateStart, DateTime? dateEnd, int orderSn = 0, string[] statusArray = null, string[] ordeyTypeAry = null, string kw = "");

        MESPermissionDao SelectNewOrderSn(string orderNo);

        int Insert(MESPermissionDao insMESPerm);

        int Update(MESPermissionDao updDao);

        int UpdateMESOrder(MESPermissionDao updDao);
    }
}
