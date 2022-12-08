using MOD4.Web.DomainService.Entity;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface IAccessFabOrderRepository
    {
        List<AccessFabOrderDao> SelectList(int orderSn = 0, string orderNo = "", int statusId = 0,
            int fabInTypeId = 0, string applicant = "", int applicantAccountSn = 0,
            int auditAccountSn = 0, DateTime? startTime = null, DateTime? endTime = null,
            DateTime? startFabInTime = null, DateTime? endFabInTime = null, List<int> orderSnList = null);

        int Insert(AccessFabOrderDao insAccessFabOrder);

        int Update(AccessFabOrderDao updAccessFabOrder);

        int UpdateAudit(AccessFabOrderDao updAccessFabOrder);

        int UpdateCancel(AccessFabOrderDao updAccessFabOrder);
    }
}
