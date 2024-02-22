using MOD4.Web.Enum;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.Repostory
{
    public class IELayoutApplyAuditHistoryRepository : BaseRepository, IIELayoutApplyAuditHistoryRepository
    {

        public List<IELayoutApplyAuditHistoryDao> SelectByConditions(int layoutSn)
        {
            string sql = "select * from ie_layout_apply_audit_history where ieLayoutSn=@IELayoutSn ";

            var dao = _dbHelper.ExecuteQuery<IELayoutApplyAuditHistoryDao>(sql, new
            {
                IELayoutSn = layoutSn
            });

            return dao;
        }

        public int InsertHistory(List<IELayoutApplyAuditHistoryDao> daoHisList)
        {
            string sql = @"INSERT INTO [dbo].[ie_layout_apply_audit_history]
           ([ieLayoutSn]
           ,[auditSn]
           ,[auditAccountSn]
           ,[auditStatusId]
           ,[auditName]
           ,[receivedTime])
     VALUES
           (@ieLayoutSn
           ,@auditSn
           ,@auditAccountSn
           ,@auditStatusId
           ,@auditName
           ,@receivedTime); ";

            var dao = _dbHelper.ExecuteNonQuery(sql, daoHisList);

            return dao;
        }

        public int Update(List<IELayoutApplyAuditHistoryDao> daoHisList)
        {
            string sql = @"UPDATE [dbo].[ie_layout_apply_audit_history]
   SET [auditStatusId] = @AuditStatusId
      ,[receivedTime] = @ReceivedTime
      ,[auditTime] = @AuditTime
      ,[remark] = @Remark
 WHERE ieLayoutSn = @IELayoutSn and auditSn=@AuditSn ; ";

            var dao = _dbHelper.ExecuteNonQuery(sql, daoHisList);

            return dao;
        }

        public int DeleteHistory(int orderSn)
        {
            string sql = @"DELETE [dbo].[ie_layout_apply_audit_history] where ieLayoutSn = @IELayoutSn; ";

            var dao = _dbHelper.ExecuteNonQuery(sql, new 
            {
                IELayoutSn = orderSn
            });

            return dao;
        }
    }
}
