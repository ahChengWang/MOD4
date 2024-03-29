﻿namespace MOD4.Web.DomainService.Entity
{
    public class MaterialSettingEntity
    {
        public int Sn { get; set; }
        public string MatlNo { get; set; }
        public string MatlName { get; set; }
        public string MatlCatg { get; set; }
        public string UseNode { get; set; }
        public decimal LossRate { get; set; }

        // 偷渡修改前"耗損率", for history
        public decimal OldLossRate { get; set; }
    }
}
