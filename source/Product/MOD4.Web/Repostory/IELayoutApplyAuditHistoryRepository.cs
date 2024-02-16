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
    }
}
