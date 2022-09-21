using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public class AccountInfoRepository : BaseRepository, IAccountInfoRepository
    {

        public List<AccountInfoDao> SelectByConditions(string account, string password = "")
        {
            string sql = "select * from account_info where account=@account ";

            if (!string.IsNullOrEmpty(password))
            {
                sql += " and password=@password ";
            }

            var dao = _dbHelper.ExecuteQuery<AccountInfoDao>(sql, new
            {
                account = account,
                password = password
            });

            return dao;
        }

        public int UpdateUserAccount(string account, string password)
        {
            string sql = "Update account_info set password=@password where account=@account ";

            var dao = _dbHelper.ExecuteNonQuery(sql, new
            {
                account = account,
                password = password
            });

            return dao;
        }


        public int InsertUserAccount(AccountInfoDao insAccountInfo)
        {
            string sql = @"INSERT INTO [dbo].[account_info]
([account],
[password],
[name],
[role],
[level_id])
VALUES
(@account,
@password,
@name,
@role,
@level_id); ";

            var dao = _dbHelper.ExecuteNonQuery(sql, insAccountInfo);

            return dao;
        }


        public int InsertUserPermission(List<AccountMenuInfoDao> insAccountMenuInfo)
        {
            string sql = @"INSERT INTO [dbo].[account_menu_info]
           ([account_sn],
            [menu_sn],
            [menu_group_sn])
           VALUES
           (@account_sn,
            @menu_sn,
            @menu_group_sn);";

            var dao = _dbHelper.ExecuteNonQuery(sql, insAccountMenuInfo);

            return dao;
        }
    }
}
