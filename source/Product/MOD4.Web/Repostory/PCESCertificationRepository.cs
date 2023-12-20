using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.Repostory
{
    public class PCESCertificationRepository : BaseRepository, IPCESCertificationRepository
    {

        public List<PCESCertificationRecordDao> SelectByConditions(List<string> oprList = null, 
                List<string> stationList = null, 
                List<string> certStatusList = null,
                List<string> statusList = null,
                string jobId = "",
                string className = "",
                string mtype = "",
                string licType = "")
        {
            string sql = "select * from pces_certification_record where 1=1 ";

            if (oprList != null && oprList.Any())
                sql += " and main_oper IN @Main_oper ";

            if (stationList != null && stationList.Any())
                sql += " and station IN @Station ";

            if (certStatusList != null && certStatusList.Any())
                sql += " and certStatus IN @CertStatus ";

            if (statusList != null && statusList.Any())
                sql += " and status IN @Status ";

            if (!string.IsNullOrEmpty(jobId))
                sql += " and apply_no = @Apply_no ";

            if (!string.IsNullOrEmpty(className))
                sql += " and class_name = @class_name ";

            if (!string.IsNullOrEmpty(licType))
                sql += " and lic_type = @lic_type ";

            if (!string.IsNullOrEmpty(mtype))
                sql += " and mtype = @mtype ";

            var dao = _dbHelper.ExecuteQuery<PCESCertificationRecordDao>(sql, new
            {
                Main_oper = oprList,
                Station = stationList,
                CertStatus = certStatusList,
                Status = statusList,
                Apply_no = jobId,
                class_name = className,
                lic_Type = licType,
                mtype = mtype
            });

            return dao;
        }

        public int UpdateCertSkill(PCESCertificationRecordDao updDao)
        {
            try
            {
                string sql = @"UPDATE [dbo].[pces_certification_record]
   SET [certStatus] = @certStatus
      ,[skill_grade] = @skill_grade
      ,[eng_no] = @eng_no
      ,[eng_name] = @eng_name
      ,[skill_status] = @skill_status
      ,[remark] = @remark
  WHERE [apply_no] = @apply_no and [main_oper] = @main_oper and [station] = @station and [type] = @type and [mtype] = @mtype and [class_name] = @class_name and [lic_type] = @lic_type ";

                var dao = _dbHelper.ExecuteNonQuery(sql, updDao);

                return dao;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public int InsertPCESRecord(List<PCESCertificationRecordDao> daoList)
        {
            string sql = @"INSERT INTO [carUX_2f].[dbo].[pces_certification_record]
([apply_no]
,[apply_name]
,[shift]
,[main_oper]
,[station]
,[type]
,[mtype]
,[class_name]
,[lic_type]
,[certStatus]
,[status]
,[pass_date]
,[valid_date]
,[subj_grade]
,[skill_grade]
,[eng_no]
,[eng_name]
,[skill_status])
VALUES
(@apply_no
,@apply_name
,@shift
,@main_oper
,@station
,@type
,@mtype
,@class_name
,@lic_type
,@certStatus
,@status
,@pass_date
,@valid_date
,@subj_grade
,@skill_grade
,@eng_no
,@eng_name
,@skill_status); ";

            var dao = _dbHelper.ExecuteNonQuery(sql, daoList);

            return dao;
        }

        public int UpdatePCESRecord(List<PCESCertificationRecordDao> daoList)
        {
            string sql = @"Update [carUX_2f].[dbo].[pces_certification_record] set 
 [certStatus] = @CertStatus
,[status] = @Status
,[pass_date] = @pass_date
,[valid_date] = @valid_date 
,[subj_grade] = @subj_grade 
,[skill_grade] = @skill_grade 
,[eng_no] = @eng_no 
,[eng_name] = @eng_name 
,[skill_status] = @skill_status 
Where apply_no=@apply_no and apply_name=@apply_name and main_oper=@main_oper and station=@station and class_name=@class_name and lic_type=@lic_type ; ";

            var dao = _dbHelper.ExecuteNonQuery(sql, daoList);

            return dao;
        }
    }
}
