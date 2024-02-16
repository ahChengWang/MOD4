using MOD4.Web.Enum;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.Repostory
{
    public class IELayoutApplyRepository : BaseRepository, IIELayoutApplyRepository
    {

        public List<IELayoutApplyDao> SelectByConditions(List<int> snList = null, string orderNo = "", DateTime? startDate = null, DateTime? endDate = null, int applicantAccSn = 0)
        {
            string sql = "select * from ie_layout_apply where 1=1 ";

            if (snList != null && snList.Any())
                sql += " and orderSn in @OrderSn ";
            if (!string.IsNullOrEmpty(orderNo))
                sql += " and orderNo = @OrderNo ";
            if (applicantAccSn != 0)
                sql += " and applicantAccountSn = @ApplicantAccountSn ";
            if (startDate != null && endDate != null)
                sql += " and applyDate between @StartDate and @EndDate ";

            var dao = _dbHelper.ExecuteQuery<IELayoutApplyDao>(sql, new
            {
                OrderSn = snList,
                OrderNo = orderNo,
                StartDate = startDate,
                EndDate = endDate,
                ApplicantAccountSn = applicantAccSn
            });

            return dao;
        }

        public int Insert(IELayoutApplyDao daoList)
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

            var dao = _dbHelper.ExecuteNonQuery(sql, daoList);

            return dao;
        }

    }
}
