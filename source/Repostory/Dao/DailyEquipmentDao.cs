using System;

namespace MOD4.Web.Repostory.Dao
{
    public class DailyEquipmentDao
    {
        public int NodeNo { get; set; }
        public DateTime MFG_Day { get; set; }
        public string Node { get; set; }
        public string Node_Name { get; set; }
        public string Shift { get; set; }
        public string Equipments { get; set; }
    }
}
