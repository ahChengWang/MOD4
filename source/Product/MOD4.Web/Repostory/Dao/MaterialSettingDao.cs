using MOD4.Web.Enum;
using System;

namespace MOD4.Web.Repostory.Dao
{
    public class MaterialSettingDao
    {
        public int MatlSn { get; set; }
        //public string MatlNo { get; set; }
        //public MatlCodeTypeEnum CodeTypeId { get; set; }
        //public string MatlName { get; set; }
        //public string MatlCatg { get; set; }
        //public string UseNode { get; set; }
        public decimal LossRate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateTime { get; set; }

        // 偷渡修改前"耗損率", for history
        public decimal OldLossRate { get; set; }
    }
}
