using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface IEquipMappingRepository
    {
        List<EquipMappingDao> SelectAll();

        List<EquipMappingDao> SelectEqByConditions(int floor = 0, string equipNo = "", string operation = "");

        int UpdateTarget(EquipMappingDao updDao);
    }
}
