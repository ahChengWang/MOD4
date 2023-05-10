using System;

namespace MOD4.Web.Repostory.Dao
{
    public class EquipMappingDao
    {
        public string EQUIP_NBR { get; set; }
        public string EQUIP_DESC { get; set; }
        public string EQUIP_GROUP { get; set; }
        public string OPERATION { get; set; }
        public string EQUIP_NBR_M { get; set; }
        public string AREA { get; set; }
        public int ENABLE { get; set; }
        public decimal MTBFTarget { get; set; }
        public decimal MTTRTarget { get; set; }
        public DateTime UpdateTime { get; set; }
        public string UpdateUser { get; set; }
    }
}
