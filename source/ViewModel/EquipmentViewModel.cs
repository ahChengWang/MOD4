using MOD4.Web.Enum;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MOD4.Web.ViewModel
{
    public class EquipmentViewModel
    {
        public List<EquipmentDetailModel> UnrepairedEqList { get; set; }
        public List<EquipmentDetailModel> RepairedEqInfoList { get; set; }
        public int PMPending { get; set; }
        public int ENGPending { get; set; }
    }

    public class EquipmentDetailModel
    {
        public int sn { get; set; }

        [Display(Name ="線體")]
        public string ToolId { get; set; }

        [Display(Name = "Code")]
        public string ToolStatus { get; set; }

        [Display(Name = "狀態描述")]
        public string StatusCdsc { get; set; }

        [Display(Name = "工程師")]
        public string UserId { get; set; }

        [Display(Name = "備註")]
        public string Comment { get; set; }

        [Display(Name = "當機時間")]
        public string LmTime { get; set; }

        [Display(Name = "修復時間(m)")]
        public string RepairedTime { get; set; }

        [Display(Name = "日期")]
        public string MFGDay { get; set; }
        public string MFGHr { get; set; }
        public string PostTime { get; set; }
        public string EndTime { get; set; }
        public string Remark4 { get; set; }
        public string Remark5 { get; set; }

        public EqIssueStatusEnum StatusId { get; set; }
    }
}
