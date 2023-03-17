using MOD4.Web.DomainService.Entity;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface IMESPermissionAuditHistoryRepository
    {
        List<MESPermissionAuditHistoryDao> SelectList(int mesOrderSn);

        int Insert(List<MESPermissionAuditHistoryDao> insMESPermissionAuditHisList);

        int Update(List<MESPermissionAuditHistoryDao> updMESOrderAuditHisList);

        int Delete(List<int> mesPermSnList);

    }
}
