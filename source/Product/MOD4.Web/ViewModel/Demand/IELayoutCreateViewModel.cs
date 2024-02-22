using MOD4.Web.DomainService.Entity;
using System;
using System.Collections.Generic;

namespace MOD4.Web.ViewModel
{
    public class IELayoutCreateViewModel
    {
        public int OrderSn { get; set; }
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