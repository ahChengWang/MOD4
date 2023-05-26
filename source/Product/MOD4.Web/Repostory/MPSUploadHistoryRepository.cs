using MOD4.Web.Repostory.Dao;
using System.Linq;

namespace MOD4.Web.Repostory
{
    public class MPSUploadHistoryRepository : BaseRepository, IMPSUploadHistoryRepository
    {

        public MPSUploadHistoryDao SelectLatest()
        {
            string sql = "select TOP 1 * from mps_upload_history order by uploadTime desc ";

            var dao = _dbHelper.ExecuteQuery<MPSUploadHistoryDao>(sql);

            return dao.FirstOrDefault();
        }

        public int Insert(MPSUploadHistoryDao insDap)
        {
            string sql = @"INSERT INTO [dbo].[mps_upload_history]
([fileName]
,[uploadUser]
,[uploadTime])
VALUES
(@fileName
,@uploadUser
,@uploadTime); ";

            var cnt = _dbHelper.ExecuteNonQuery(sql, insDap);

            return cnt;
        }
    }
}
