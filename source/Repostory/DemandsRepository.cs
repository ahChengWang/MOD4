﻿using MOD4.Web.Repostory.Dao;
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
            string sql = "select * from demands where isCancel = 0 and createTime between @dateStart and @dateEnd ";

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


        public DemandsDao SelectDetail(int orderSn, string orderNo = "")
        {
            string sql = "select * from demands where isCancel = 0 and orderSn=@orderSn ";

            if (!string.IsNullOrEmpty(orderNo))
            {
                sql += " and orderNo=@orderNo ";
            }

            var dao = _dbHelper.ExecuteQuery<DemandsDao>(sql, new
            {
                orderSn = orderSn,
                orderNo = orderNo
            }).FirstOrDefault();

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
[updateTime],
[isCancel])
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
@updateTime,
@isCancel); ";

            var dao = _dbHelper.ExecuteNonQuery(sql, insDemands);

            return dao;
        }


        public int UpdateToProcess(DemandsDao updDao)
        {
            string sql = @" UPDATE [dbo].[demands]
   SET [statusId] = @statusId
      ,[updateUser] = @updateUser
      ,[updateTime] = @updateTime
 WHERE orderSn=@orderSn and orderNo=@orderNo 
 ";

            var dao = _dbHelper.ExecuteNonQuery(sql, updDao);

            return dao;
        }

        public int UpdateToReject(DemandsDao updDao)
        {
            string sql = @" UPDATE [dbo].[demands]
   SET [statusId] = @statusId
      ,[updateUser] = @updateUser
      ,[updateTime] = @updateTime
      ,[rejectReason] = @rejectReason 
 WHERE orderSn=@orderSn and orderNo=@orderNo 
 ";

            var dao = _dbHelper.ExecuteNonQuery(sql, updDao);

            return dao;
        }

        public int UpdateToComplete(DemandsDao updDao)
        {
            string sql = @" UPDATE [dbo].[demands]
   SET [statusId] = @statusId
      ,[updateUser] = @updateUser
      ,[updateTime] = @updateTime
      ,[completeFiles] = @completeFiles
      ,[remark] = @remark
 WHERE orderSn=@orderSn and orderNo=@orderNo 
 ";

            var dao = _dbHelper.ExecuteNonQuery(sql, updDao);

            return dao;
        }

        public int UpdateToPending(DemandsDao updDao)
        {
            string sql = @" UPDATE [dbo].[demands]
   SET [statusId] = @statusId
      ,[categoryId] = @categoryId
      ,[subject] = @subject
      ,[content] = @content
      ,[applicant] = @applicant
      ,[jobNo] = @jobNo
      ,[updateUser] = @updateUser
      ,[updateTime] = @updateTime
      ,[uploadFiles] = @uploadFiles 
 WHERE orderSn=@orderSn and orderNo=@orderNo 
 ";

            var dao = _dbHelper.ExecuteNonQuery(sql, updDao);

            return dao;
        }


        public int UpdateToCancel(DemandsDao updDao)
        {
            string sql = @" UPDATE [dbo].[demands]
   SET [statusId] = @statusId
      ,[isCancel] = @isCancel
      ,[updateUser] = @updateUser
      ,[updateTime] = @updateTime
 WHERE orderSn=@orderSn and orderNo=@orderNo 
 ";

            var dao = _dbHelper.ExecuteNonQuery(sql, updDao);

            return dao;
        }
    }
}