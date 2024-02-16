using MOD4.Web.Enum;
using System;
using System.Collections.Generic;

namespace MOD4.Web.DomainService.Entity
{
    public class IELayoutCreateEntity
    {
        public string Name { get; set; }
        public string Department { get; set; }
        public string Phone { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ApplyDate { get; set; }
        public List<OptionEntity> FactoryList { get; set; }
        public List<OptionEntity> ProcessAreaList { get; set; }
        public List<OptionEntity> FormatTypeList { get; set; }
        public List<OptionEntity> ReasonTypeList { get; set; }
        public List<OptionEntity> LayerTypeList { get; set; }
        public string IssueRemark { get; set; }
    }
}
