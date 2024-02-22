using MOD4.Web.Enum;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface IIELayoutApplyRepository
    {
        List<IELayoutApplyDao> SelectByConditions(List<int> snList = null, string orderNo = "", AuditStatusEnum auditStatusId = 0, DateTime? startDate = null, DateTime? endDate = null, int applicantAccSn = 0, int auditAccSn = 0, string createUser = "");
        int Insert(IELayoutApplyDao daoList);
        int Update(IELayoutApplyDao updOrderDao);
        int UpdateResend(IELayoutApplyDao updOrderDao);
    }
}