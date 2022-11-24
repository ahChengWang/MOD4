using MOD4.Web.DomainService.Entity;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface IAccessFabOrderAuditHistoryRepository
    {
        List<AccessFabOrderAuditHistoryDao> SelectList(int accessFabOrderSn);

        int Insert(List<AccessFabOrderAuditHistoryDao> insAccessFabOrderAuditHistoryList);

        int Update(List<AccessFabOrderAuditHistoryDao> insAccessFabOrderAuditHisList);

        int UpdateAudit(List<AccessFabOrderAuditHistoryDao> updAccessFabOrderAuditHisList);
    }
}
