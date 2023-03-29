using MOD4.Web.DomainService.Entity;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.Repostory
{
    public class MESPermissionAuditHistoryRepository : BaseRepository, IMESPermissionAuditHistoryRepository
    {

        public List<MESPermissionAuditHistoryDao> SelectList(int mesOrderSn)
        {
            string sql = "select * from mes_permission_audit_history where mesPermissionSn=@mesPermissionSn and isDel = 0 ";

            var dao = _dbHelper.ExecuteQuery<MESPermissionAuditHistoryDao>(sql, new
            {
                mesPermissionSn = mesOrderSn
            });

            return dao;
        }

        public int Insert(List<MESPermissionAuditHistoryDao> insMESPermissionAuditHisList)
        {
            string sql = @"INSERT INTO [dbo].[mes_permission_audit_history]
([mesPermissionSn]
,[auditSn]
,[auditAccountSn]
,[auditAccountName]
,[statusId]
,[receivedTime]
,[mail]
,[isDel])
VALUES
(@mesPermissionSn
,@auditSn
,@auditAccountSn
,@auditAccountName
,@statusId
,@receivedTime
,@mail
,@isDel); ";

            var dao = _dbHelper.ExecuteNonQuery(sql, insMESPermissionAuditHisList);

            return dao;
        }

        public int Update(List<MESPermissionAuditHistoryDao> updMESOrderAuditHisList)
        {
            string sql = @"Update [dbo].[mes_permission_audit_history] set 
 statusId = @statusId
,receivedTime = @receivedTime 
,auditTime = @auditTime
,auditRemark = @auditRemark
where mesPermissionSn = @mesPermissionSn and auditSn = @auditSn ;";

            var dao = _dbHelper.ExecuteNonQuery(sql, updMESOrderAuditHisList);

            return dao;
        }

        public int Delete(List<int> mesPermSnList)
        {
            string sql = @"
DELETE [dbo].[mes_permission_audit_history] WHERE mesPermissionSn in @mesPermissionSn ;";

            var dao = _dbHelper.ExecuteNonQuery(sql, new {
                mesPermissionSn = mesPermSnList
            });

            return dao;
        }
    }
}
