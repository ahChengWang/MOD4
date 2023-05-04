using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.Repostory
{
    public class MESPermissionRepository : BaseRepository, IMESPermissionRepository
    {

        public List<MESPermissionDao> SelectByConditions(
            DateTime? dateStart
            , DateTime? dateEnd
            , int orderSn = 0
            , string[] statusArray = null
            , string[] ordeyTypeAry = null
            , string kw = "")
        {
            string sql = "select * from mes_permission where isCancel = 0 ";

            if (dateStart != null)
                sql += " and createTime >= @dateStart ";

            if (dateEnd != null)
                sql += " and createTime <= @dateEnd ";

            if (orderSn != 0)
                sql += " and orderSn=@orderSn ";

            if (statusArray != null)
                sql += " and statusId in @statusId ";

            if (ordeyTypeAry != null)
                sql += " and mesOrderTypeId in @mesOrderTypeId ";

            if (!string.IsNullOrEmpty(kw))
                sql += $" and (applicant like '%{kw}%' or jobId like '%{kw}%') ";

            var dao = _dbHelper.ExecuteQuery<MESPermissionDao>(sql, new
            {
                dateStart = dateStart,
                dateEnd = dateEnd,
                orderSn = orderSn,
                statusId = statusArray,
                mesOrderTypeId = ordeyTypeAry
            });

            return dao;
        }


        public MESPermissionDao SelectNewOrderSn(string orderNo)
        {
            string sql = "select * from mes_permission where isCancel = 0 and orderNo=@orderNo ";

            var dao = _dbHelper.ExecuteQuery<MESPermissionDao>(sql, new
            {
                orderNo = orderNo
            }).FirstOrDefault();

            return dao;
        }

        public int Insert(MESPermissionDao insMESPerm)
        {
            string sql = @"INSERT INTO [dbo].[mes_permission]
([orderNo]
,[statusId]
,[department]
,[subUnit]
,[applicant]
,[jobId]
,[phone]
,[samePermName]
,[samePermJobId]
,[permissionList]
,[otherPermission]
,[auditAccountSn]
,[applicantAccountSn]
,[createUser]
,[createTime]
,[isCancel]
,[mesOrderTypeId]
,[applicantReason])
VALUES
(@orderNo
,@statusId
,@department
,@subUnit
,@applicant
,@jobId
,@phone
,@samePermName
,@samePermJobId
,@permissionList
,@otherPermission
,@auditAccountSn
,@applicantAccountSn
,@createUser
,@createTime
,@isCancel
,@mesOrderTypeId
,@applicantReason); ";

            var dao = _dbHelper.ExecuteNonQuery(sql, insMESPerm);

            return dao;
        }

        public int Update(MESPermissionDao updDao)
        {
            string sql = @" UPDATE [dbo].[mes_permission]
   SET [statusId] = @statusId
      ,[auditAccountSn] = @auditAccountSn
      ,[updateUser] = @updateUser
      ,[updateTime] = @updateTime
      ,[applicantReason] = @applicantReason
 WHERE orderSn=@orderSn ;";

            var dao = _dbHelper.ExecuteNonQuery(sql, updDao);

            return dao;
        }

        public int UpdateMESOrder(MESPermissionDao updDao)
        {
            string sql = @" UPDATE [dbo].[mes_permission]
   SET [statusId] = @statusId
      ,[department] = @department
      ,[subUnit] = @subUnit
      ,[applicant] = @applicant
      ,[jobId] = @jobId
      ,[phone] = @phone
      ,[samePermName] = @samePermName
      ,[samePermJobId] = @samePermJobId
      ,[permissionList] = @permissionList
      ,[otherPermission] = @otherPermission
      ,[auditAccountSn] = @auditAccountSn
      ,[applicantAccountSn] = @applicantAccountSn
      ,[updateUser] = @updateUser
      ,[updateTime] = @updateTime
      ,[isCancel] = @isCancel 
 WHERE orderSn=@orderSn ;";

            var dao = _dbHelper.ExecuteNonQuery(sql, updDao);

            return dao;
        }
    }
}
