using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.Repostory
{
    public class DemandsRepository : BaseRepository, IDemandsRepository
    {

        public List<DemandsDao> SelectByConditions(
            DateTime dateStart
            , DateTime dateEnd
            , string orderSn = ""
            , string orderNo = ""
            , string[] categoryArray = null
            , string[] statusArray = null)
        {
            string sql = "select * from demands where createTime between @dateStart and @dateEnd ";

            if (!string.IsNullOrEmpty(orderSn))
                sql += " and orderSn=@orderSn ";

            if (!string.IsNullOrEmpty(orderNo))
                sql += " and orderNo=@orderNo ";

            if (categoryArray != null)
                sql += " and categoryId in @categoryId ";

            if (statusArray != null)
                sql += " and statusId in @statusId ";

            var dao = _dbHelper.ExecuteQuery<DemandsDao>(sql, new
            {
                dateStart = dateStart,
                dateEnd = dateEnd,
                orderSn = orderSn,
                orderNo = orderNo,
                categoryId = categoryArray,
                statusId = statusArray
            });

            return dao;
        }


        public DemandsDao SelectDetail(int orderSn, string orderNo)
        {
            string sql = "select * from demands where orderSn=@orderSn and orderNo=@orderNo ";

            var dao = _dbHelper.ExecuteQuery<DemandsDao>(sql, new
            {
                orderSn = orderSn,
                orderNo = orderNo
            }).FirstOrDefault();

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


        public int Insert(DemandsDao insDemands)
        {
            string sql = @"INSERT INTO [dbo].[demands]
([orderNo],
[categoryId],
[statusId],
[subject],
[content],
[applicant],
[jobNo],
[uploadFiles],
[createUser],
[createTime],
[updateUser],
[updateTime])
VALUES
(@orderNo,
@categoryId,
@statusId,
@subject,
@content,
@applicant,
@jobNo,
@uploadFiles,
@createUser,
@createTime,
@updateUser,
@updateTime); ";

            var dao = _dbHelper.ExecuteNonQuery(sql, insDemands);

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
