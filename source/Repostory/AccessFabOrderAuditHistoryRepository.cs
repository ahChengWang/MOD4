using MOD4.Web.DomainService.Entity;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.Repostory
{
    public class AccessFabOrderAuditHistoryRepository : BaseRepository, IAccessFabOrderAuditHistoryRepository
    {

        public List<AccessFabOrderAuditHistoryDao> SelectList(int accessFabOrderSn)
        {
            string sql = "select * from access_fab_order_audit_history where accessFabOrderSn=@AccessFabOrderSn and isDel = 0 ";

            var dao = _dbHelper.ExecuteQuery<AccessFabOrderAuditHistoryDao>(sql, new
            {
                AccessFabOrderSn = accessFabOrderSn
            });

            return dao;
        }


        public int Insert(List<AccessFabOrderAuditHistoryDao> insAccessFabOrderAuditHisList)
        {
            string sql = @"INSERT INTO [dbo].[access_fab_order_audit_history]
([accessFabOrderSn]
,[auditSn]
,[auditAccountSn]
,[auditAccountName]
,[statusId]
,[receivedTime]
,[isDel])
VALUES
(@accessFabOrderSn
,@auditSn
,@auditAccountSn
,@auditAccountName
,@statusId
,@receivedTime
,@isDel); ";

            var dao = _dbHelper.ExecuteNonQuery(sql, insAccessFabOrderAuditHisList);

            return dao;
        }

        public int Update(List<AccessFabOrderAuditHistoryDao> updAccessFabOrderAuditHisList)
        {
            string sql = @"Update [dbo].[access_fab_order_audit_history] set 
 statusId = @statusId
,auditTime = @auditTime
,auditRemark = @auditRemark
,receivedTime = @receivedTime 
where accessFabOrderSn = @AccessFabOrderSn and auditSn = @AuditSn ;";

            var dao = _dbHelper.ExecuteNonQuery(sql, updAccessFabOrderAuditHisList);

            return dao;
        }

        public int UpdateAudit(List<AccessFabOrderAuditHistoryDao> updAccessFabOrderAuditHisList)
        {
            string sql = @"Update [dbo].[access_fab_order_audit_history] set 
 statusId = @statusId
,auditTime = @auditTime
,auditRemark = @auditRemark 
,receivedTime = @receivedTime 
where accessFabOrderSn = @AccessFabOrderSn and auditSn = @AuditSn ;";

            var dao = _dbHelper.ExecuteNonQuery(sql, updAccessFabOrderAuditHisList);

            return dao;
        }

    }
}
