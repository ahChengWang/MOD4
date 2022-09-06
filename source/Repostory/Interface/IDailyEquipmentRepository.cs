using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface IDailyEquipmentRepository
    {
        List<DailyEquipmentDao> SelectByConditions(string mfgDay, string shift, List<string> node);
    }
}
