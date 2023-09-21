using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.Repostory
{
    public class PCESCertificationRepository : BaseRepository, IPCESCertificationRepository
    {

        public List<PCESCertificationRecordDao> SelectByConditions(List<string> oprList = null, List<string> stationList = null, List<string> certStatusList = null, string jobId = "")
        {
            string sql = "select * from pces_certification_record where 1=1 ";

            if (oprList != null && oprList.Any())
                sql += " and main_oper IN @Main_oper ";

            if (stationList != null && stationList.Any())
                sql += " and station IN @Station ";

            if (certStatusList != null && certStatusList.Any())
                sql += " and certStatus IN @CertStatus ";

            if (!string.IsNullOrEmpty(jobId))
                sql += " and apply_no = @Apply_no ";

            var dao = _dbHelper.ExecuteQuery<PCESCertificationRecordDao>(sql, new
            {
                Main_oper = oprList,
                Station = stationList,
                CertStatus = certStatusList,
                Apply_no = jobId
            });

            return dao;
        }
    }
}
