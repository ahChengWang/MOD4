using MOD4.Web.Repostory.Dao;

namespace MOD4.Web.Repostory
{
    public interface IMPSUploadHistoryRepository
    {
        int Insert(MPSUploadHistoryDao insDap);
        MPSUploadHistoryDao SelectLatest();
    }
}