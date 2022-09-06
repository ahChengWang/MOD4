using System;

namespace MOD4.Web.DomainService.Entity
{
    public class EquipmentEntity
    {
        public int sn { get; set; }
        public string ToolId { get; set; }
        public string ToolStatus { get; set; }
        public string StatusCdsc { get; set; }
        public string UserId { get; set; }
        public string Comment { get; set; }
        public DateTime LmTime { get; set; }
        public string XMLTime { get; set; }
        public string MFGDay { get; set; }
        public string MFGHr { get; set; }
        public string PostTime { get; set; }
        public string RepairTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Remark4 { get; set; }
        public string Remark5 { get; set; }
        public DateTime UpdateTime { get; set; }
        public string eq_unit { get; set; }
        public int defect_qty { get; set; }
        public decimal defect_rate { get; set; }
        public string engineer { get; set; }
        public int category { get; set; }
        public string memo { get; set; }
        public bool IsPMProcess { get; set; }
        public bool IsEngineerProcess { get; set; }
    }
}
