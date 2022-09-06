using System;

namespace MOD4.Web.DomainService.Entity
{
    public class EquipmentEditEntity
    {
        public int sn { get; set; }
        public string Equipment { get; set; }
        public string Code { get; set; }
        public string CodeDesc { get; set; }
        public string Operator { get; set; }
        public string Comments { get; set; }
        public string MFGDay { get; set; }
        public string MFGHr { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public string Shift { get; set; }
        public string EqUnit { get; set; }
        public int DefectQty { get; set; }
        public string DefectRate { get; set; }
        public string Engineer { get; set; }
        public string Category { get; set; }
        public string Memo { get; set; }
        public string MntUser { get; set; }
        public string MntMinutes { get; set; }
        public int IsPMProcess { get; set; }
        public int IsEngineerProcess { get; set; }
    }
}
