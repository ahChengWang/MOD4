using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface IIELayoutApplyAuditHistoryRepository
    {
        int InsertHistory(List<IELayoutApplyAuditHistoryDao> daoHisList);
        List<IELayoutApplyAuditHistoryDao> SelectByConditions(int layoutSn);
        int Update(List<IELayoutApplyAuditHistoryDao> daoHisList);
        int DeleteHistory(int orderSn);
    }
}