using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MOD4.Web.ViewModel
{
    public class LineViewModel
    {
        public List<LineDetailModel> LineDetailList { get; set; }
    }


    public class LineDetailModel
    {
        public int sn { get; set; }

        [Display(Name = "站點代號")]
        public string Operation { get; set; }

        [Display(Name = "站點")]
        public string Station { get; set; }

        [Display(Name = "線體")]
        public string Line { get; set; }

        [Display(Name = "eq number")]
        public string ToolId { get; set; }

        [Display(Name = "AOI number")]
        public string TargetLine { get; set; }

        [Display(Name = "製程區域")]
        public string Area { get; set; }

        [Display(Name = "匯入分時")]
        public string EnableImport { get; set; }

        [Display(Name = "看板顯示")]
        public string EnableDashboard { get; set; }
    }
}
