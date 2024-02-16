using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface IIELayoutApplyRepository
    {
        List<IELayoutApplyDao> SelectByConditions(List<int> snList = null, string orderNo = "", DateTime? startDate = null, DateTime? endDate = null, int applicantAccSn = 0);
        int Insert(IELayoutApplyDao daoList);
    }
}