using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public class AccountInfoRepository : BaseRepository, IAccountInfoRepository
    {

        public List<AccountInfoDao> SelectByConditions(string account, string password)
        {
            string sql = "select * from account_info where account=@account";

            var dao = _dbHelper.ExecuteQuery<AccountInfoDao>(sql, new
            {
                account = account,
                //password = password
            });

            return dao;
        }
    }
}
