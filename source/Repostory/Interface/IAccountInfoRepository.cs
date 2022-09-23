using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface IAccountInfoRepository
    {
        List<AccountInfoDao> SelectByConditions(string account = "", string password = "");

        int UpdateUserAccount(string account, string password);

        int InsertUserAccount(AccountInfoDao insAccountInfo);

        int InsertUserPermission(List<AccountMenuInfoDao> insAccountMenuInfo);

    }
}
