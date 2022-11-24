using MOD4.Web.DomainService.Entity;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.Repostory
{
    public class AccessFabOrderRepository : BaseRepository, IAccessFabOrderRepository
    {


        public List<AccessFabOrderDao> SelectList(
            int orderSn = 0, 
            string orderNo = "",
            int statusId = 0,
            int fabInTypeId = 0,
            string applicant = "",
            int applicantAccountSn = 0,
            int auditAccountSn = 0,
            DateTime? startTime = null,
            DateTime? endTime = null
            )
        {
            string sql = "select * from access_fab_order where 1=1 ";

            if (orderSn != 0)
            {
                sql += " and orderSn = @OrderSn ";
            }
            if (!string.IsNullOrEmpty(orderNo))
            {
                sql += " and orderNo = @OrderNo ";
            }
            if (statusId != 0)
            {
                sql += " and statusId = @StatusId ";
            }
            if (fabInTypeId != 0)
            {
                sql += " and fabInTypeId = @FabInTypeId ";
            }
            if (!string.IsNullOrEmpty(applicant))
            {
                sql += " and applicant = @Applicant ";
            }
            if (startTime != null)
            {
                sql += " and createTime >= @CreateStartTime ";
            }
            if (endTime != null)
            {
                sql += " and createTime < @CreateEndTime ";
            }
            if (applicantAccountSn != 0)
            {
                sql += " and applicantAccountSn = @ApplicantAccountSn ";
            }
            if (auditAccountSn != 0)
            {
                sql += " and auditAccountSn = @AuditAccountSn ";
            }

            var dao = _dbHelper.ExecuteQuery<AccessFabOrderDao>(sql, new
            {
                OrderSn = orderSn,
                OrderNo = orderNo,
                StatusId = statusId,
                FabInTypeId = fabInTypeId,
                Applicant = applicant,
                CreateStartTime = startTime,
                CreateEndTime = endTime,
                ApplicantAccountSn = applicantAccountSn,
                AuditAccountSn = auditAccountSn
            });

            return dao;
        }


        public int Insert(AccessFabOrderDao insAccessFabOrder)
        {
            string sql = @"INSERT INTO [dbo].[access_fab_order]
([orderNo]
,[fabInTypeId]
,[fabInOtherType]
,[categoryId]
,[statusId]
,[applicant]
,[jobId]
,[applicantMVPN]
,[fabInDate]
,[fabOutDate]
,[content]
,[route]
,[accompanyingPerson]
,[auditAccountSn]
,[applicantAccountSn]
,[uploadFileName]
,[createUser]
,[createTime]
,[updateUser]
,[updateTime]
,[remark])
VALUES
(@orderNo
,@fabInTypeId
,@fabInOtherType
,@categoryId
,@statusId
,@applicant
,@jobId
,@applicantMVPN
,@fabInDate
,@fabOutDate
,@content
,@route
,@accompanyingPerson
,@auditAccountSn
,@applicantAccountSn
,@uploadFileName
,@createUser
,@createTime
,@updateUser
,@updateTime
,@remark); ";

            var dao = _dbHelper.ExecuteNonQuery(sql, insAccessFabOrder);

            return dao;
        }


        public int Update(AccessFabOrderDao updAccessFabOrder)
        {
            string sql = @"Update [dbo].[access_fab_order] set 
 fabInTypeId = @fabInTypeId
,fabInOtherType = @fabInOtherType
,categoryId = @categoryId
,statusId = @statusId
,applicant = @applicant
,jobId = @jobId
,applicantMVPN = @applicantMVPN
,fabInDate = @fabInDate
,fabOutDate = @fabOutDate
,content = @content
,route = @route
,accompanyingPerson = @accompanyingPerson
,auditAccountSn = @auditAccountSn
,applicantAccountSn = @applicantAccountSn
,updateUser = @updateUser
,updateTime = @updateTime
where OrderSn = @OrderSn ; ";

            var dao = _dbHelper.ExecuteNonQuery(sql, updAccessFabOrder);

            return dao;
        }

        public int UpdateAudit(AccessFabOrderDao updAccessFabOrder)
        {
            string sql = @"Update [dbo].[access_fab_order] set 
 statusId = @statusId
,auditAccountSn = @auditAccountSn
,updateUser = @updateUser
,updateTime = @updateTime 
where OrderSn = @OrderSn ; ";

            var dao = _dbHelper.ExecuteNonQuery(sql, updAccessFabOrder);

            return dao;
        }

        public int UpdateCancel(AccessFabOrderDao updAccessFabOrder)
        {
            string sql = @"Update [dbo].[access_fab_order] set 
 statusId = @statusId
,updateUser = @updateUser
,updateTime = @updateTime
where OrderSn = @OrderSn ; ";

            var dao = _dbHelper.ExecuteNonQuery(sql, updAccessFabOrder);

            return dao;
        }
    }
}
