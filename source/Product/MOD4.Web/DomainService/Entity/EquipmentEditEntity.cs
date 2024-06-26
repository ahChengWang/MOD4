﻿using MOD4.Web.Enum;
using System;

namespace MOD4.Web.DomainService.Entity
{
    public class EquipmentEditEntity
    {
        public int sn { get; set; }
        public string Equipment { get; set; }
        public string Code { get; set; }
        public string CodeDesc { get; set; }
        public int ProductId { get; set; }
        public string Product { get; set; }
        public string ProductName { get; set; }
        public string Operator { get; set; }
        public string DownType { get; set; }
        public string Comments { get; set; }
        public string MFGDay { get; set; }
        public string MFGHr { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public decimal RepairedTime { get; set; }
        public int Shift { get; set; }
        public string ShiftDesc { get; set; }
        public int ProcessId { get; set; }
        public string Process { get; set; }
        public int EqUnitId { get; set; }
        public string EqUnit { get; set; }
        public int EqUnitPartId { get; set; }
        public string EqUnitPart { get; set; }
        public int DefectQty { get; set; }
        public string DefectRate { get; set; }
        public string Engineer { get; set; }
        public int PriorityId { get; set; }
        public string Priority { get; set; }
        public string Memo { get; set; }
        public string MntUser { get; set; }
        public string MntMinutes { get; set; }
        public int TypeId { get; set; }
        public string TypeDesc { get; set; }
        public int YId { get; set; }
        public string YDesc { get; set; }
        public int SubYId { get; set; }
        public string SubYDesc { get; set; }
        public int XId { get; set; }
        public string XDesc { get; set; }
        public int SubXId { get; set; }
        public string SubXDesc { get; set; }
        public int RId { get; set; }
        public string RDesc { get; set; }
        public int ENGTypeId { get; set; }
        public string ENGTypeDesc { get; set; }
        public int ENGYId { get; set; }
        public string ENGYDesc { get; set; }
        public int ENGSubYId { get; set; }
        public string ENGSubYDesc { get; set; }
        public int ENGXId { get; set; }
        public string ENGXDesc { get; set; }
        public int ENGSubXId { get; set; }
        public string ENGSubXDesc { get; set; }
        public int ENGRId { get; set; }
        public string ENGRDesc { get; set; }
        public EqIssueStatusEnum StatusId { get; set; }
        public string ToolName { get; set; }
    }
}
