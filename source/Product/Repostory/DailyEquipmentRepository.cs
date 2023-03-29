using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public class DailyEquipmentRepository : BaseRepository, IDailyEquipmentRepository
    {

        public List<DailyEquipmentDao> SelectByConditions(string mfgDay, string shift, List<string> node)
        {
            string sql = "select * from Daily_Equipment where MFG_Day=@MFG_Day and Shift = @Shift and Node in @Node ";

            var dao = _dbHelper.ExecuteQuery<DailyEquipmentDao>(sql, new
            {
                MFG_Day = mfgDay,
                Shift = shift,
                Node = node
            });

            return dao;
        }
    }
}
