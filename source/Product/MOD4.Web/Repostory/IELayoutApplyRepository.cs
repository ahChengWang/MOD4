using MOD4.Web.Enum;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.Repostory
{
    public class IELayoutApplyRepository : BaseRepository, IIELayoutApplyRepository
    {

        public List<IELayoutApplyDao> SelectByConditions(
            List<int> snList = null, 
            string orderNo = "", 
            AuditStatusEnum auditStatusId = 0, 
            DateTime? startDate = null, 
            DateTime? endDate = null, 
            int applicantAccSn = 0, 
            int auditAccSn = 0,
            string createUser = "")
        {
            string sql = "select * from ie_layout_apply where statusId != 4 ";

            if (snList != null && snList.Any())
                sql += " and orderSn in @OrderSn ";
            if (!string.IsNullOrEmpty(orderNo))
                sql += " and orderNo = @OrderNo ";

            if (auditStatusId != 0)
                sql += " and statusId = @StatusId ";

            if (applicantAccSn != 0)
                sql += " and (applicantAccountSn = @ApplicantAccountSn ";
            else
                sql += " and (1=1 ";

            if (auditAccSn != 0)
                sql += " or auditAccountSn = @AuditAccountSn) ";
            else
                sql += ")";
            if (startDate != null)
                sql += " and convert(date,createTime) >= @StartDate ";
            if (endDate != null)
                sql += " and convert(date,createTime) <= @EndDate ";

            if (!string.IsNullOrEmpty(createUser))
                sql += $" and createUser like '%{createUser}%' ";

            var dao = _dbHelper.ExecuteQuery<IELayoutApplyDao>(sql, new
            {
                OrderSn = snList,
                OrderNo = orderNo,
                StatusId = auditStatusId,
                StartDate = startDate,
                EndDate = endDate,
                ApplicantAccountSn = applicantAccSn,
                AuditAccountSn = auditAccSn
            });

            return dao;
        }

        public int Insert(IELayoutApplyDao insOrderDao)
        {
            string sql = @"INSERT INTO [dbo].[ie_layout_apply]
           ([orderNo]
           ,[applicantAccountSn]
           ,[department]
           ,[phone]
           ,[applyDate]
           ,[statusId]
           ,[factoryFloor]
           ,[processArea]
           ,[partRemark]
           ,[formatType]
           ,[reasonTypeId]
           ,[reason]
           ,[layerTypeId]
           ,[issueRemark]
           ,[auditAccountSn]
           ,[createUser]
           ,[createTime]
           ,[isIEFlow])
           VALUES
           (@orderNo
           ,@applicantAccountSn
           ,@department
           ,@phone
           ,@applyDate
           ,@statusId
           ,@factoryFloor
           ,@processArea
           ,@partRemark
           ,@formatType
           ,@reasonTypeId
           ,@reason
           ,@layerTypeId
           ,@issueRemark
           ,@auditAccountSn
           ,@createUser
           ,@createTime
           ,@isIEFlow); ";

            var dao = _dbHelper.ExecuteNonQuery(sql, insOrderDao);

            return dao;
        }

        public int Update(IELayoutApplyDao updOrderDao)
        {
            string sql = @"UPDATE [dbo].[ie_layout_apply]
   SET [statusId] = @StatusId
      ,[auditAccountSn] = @AuditAccountSn
      ,[secretLevelId] = @SecretLevelId
      ,[exptOutputDate] = @ExptOutputDate
      ,[version] = @Version
      ,[updateUser] = @UpdateUser
      ,[updateTime] = @UpdateTime
      ,[isIEFlow] = @IsIEFlow 
 WHERE orderSn = @OrderSn ; ";

            var dao = _dbHelper.ExecuteNonQuery(sql, updOrderDao);

            return dao;
        }

        public int UpdateResend(IELayoutApplyDao updOrderDao)
        {
            string sql = @"UPDATE [dbo].[ie_layout_apply]
   SET [department] = @Department
      ,[phone] = @Phone
      ,[applyDate] = @ApplyDate
      ,[statusId] = @StatusId
      ,[factoryFloor] = @FactoryFloor
      ,[processArea] = @ProcessArea
      ,[partRemark] = @PartRemark
      ,[formatType] = @FormatType
      ,[reasonTypeId] = @ReasonTypeId
      ,[reason] = @Reason
      ,[layerTypeId] = @LayerTypeId
      ,[issueRemark] = @IssueRemark
      ,[auditAccountSn] = @AuditAccountSn
      ,[updateUser] = @UpdateUser
      ,[updateTime] = @UpdateTime
      ,[isIEFlow] = @IsIEFlow
 WHERE orderSn = @OrderSn ; ";

            var dao = _dbHelper.ExecuteNonQuery(sql, updOrderDao);

            return dao;
        }

    }
}
