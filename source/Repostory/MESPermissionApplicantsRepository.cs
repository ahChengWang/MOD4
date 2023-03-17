using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public class MESPermissionApplicantsRepository : BaseRepository, IMESPermissionApplicantsRepository
    {

        public List<MESPermissionApplicantsDao> SelectByConditions(List<int> mesOrderSn)
        {
            string sql = "select * from mes_permission_applicants where mesPermissionSn in @mesPermissionSn ";

            var dao = _dbHelper.ExecuteQuery<MESPermissionApplicantsDao>(sql, new
            {
                mesPermissionSn = mesOrderSn
            });

            return dao;
        }

        public int Insert(List<MESPermissionApplicantsDao> insMESDetail)
        {
            string sql = @"INSERT INTO [dbo].[mes_permission_applicants]
([mesPermissionSn]
,[applicantName]
,[applicantJobId]
,[createUser]
,[createTime])
VALUES
(@mesPermissionSn
,@applicantName
,@applicantJobId
,@createUser
,@createTime); ";

            var dao = _dbHelper.ExecuteNonQuery(sql, insMESDetail);

            return dao;
        }

        public int Delete(List<int> mesOrderSnList)
        {
            string sql = @"
DELETE [dbo].[mes_permission_applicants] WHERE mesPermissionSn in @mesPermissionSn; ";

            var dao = _dbHelper.ExecuteNonQuery(sql, new 
            {
                mesPermissionSn = mesOrderSnList
            });

            return dao;
        }
    }
}
