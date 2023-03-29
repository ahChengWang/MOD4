using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface IMESPermissionApplicantsRepository
    {
        List<MESPermissionApplicantsDao> SelectByConditions(List<int> mesOrderSn);

        int Insert(List<MESPermissionApplicantsDao> insDaoList);

        int Delete(List<int> mesOrderSnList);
    }
}