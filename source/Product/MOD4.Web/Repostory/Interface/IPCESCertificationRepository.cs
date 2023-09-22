using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface IPCESCertificationRepository
    {
        List<PCESCertificationRecordDao> SelectByConditions(List<string> oprList = null, 
                List<string> stationList = null, 
                List<string> certStatusList = null, 
                string jobId = "",
                string className = "",
                string mtype = "",
                string licType = "");

        int UpdateCertSkill(PCESCertificationRecordDao updDao);
    }
}