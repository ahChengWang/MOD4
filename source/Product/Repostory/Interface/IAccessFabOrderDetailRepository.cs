using MOD4.Web.DomainService.Entity;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface IAccessFabOrderDetailRepository
    {
        List<AccessFabOrderDetailDao> SelectList(int accessFabOrderSn = 0, List<int> accessFabSnList = null, string guestName = "");

        int Insert(List<AccessFabOrderDetailDao> insAccessFabOrder);

        int Delete(List<int> delSnList);

        int Update(List<AccessFabOrderDetailDao> updAccessFabOrder);
    }
}
