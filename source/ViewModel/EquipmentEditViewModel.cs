using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MOD4.Web.ViewModel
{
    public class EquipmentEditViewModel
    {
        public int sn { get; set; }

        [Display(Name = "線體")]
        public string ToolId { get; set; }

        [Display(Name = "Code")]
        public string Code { get; set; }

        [Display(Name = "狀態描述")]
        public string Codedesc { get; set; }

        [Display(Name = "人員")]
        public string Operator { get; set; }

        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Display(Name = "當機時間")]
        public string StartTime { get; set; }

        [Display(Name = "日期")]
        public string MFGDay { get; set; }
        public string MFGHr { get; set; }

        [Display(Name = "班別")]
        [Required(ErrorMessage = "*必填")]
        public int Shift { get; set; }

        [Display(Name = "Process")]
        [Required(ErrorMessage = "*必填")]
        public int ProcessId { get; set; }

        [Display(Name = "Unit")]
        [Required(ErrorMessage = "*必填")]
        public int EqUnitId { get; set; }

        [Display(Name = "Unit-Part")]
        [Required(ErrorMessage = "*必填")]
        public int EqUnitPartId { get; set; }


        [Display(Name = "維修人員")]
        [Required(ErrorMessage = "*必填")]
        public string MntUser { get; set; }

        [Display(Name = "維修記錄")]
        [Required(ErrorMessage = "*必填")]
        public string MntMinutes { get; set; }

        [Display(Name = "不良品數")]
        [Required(ErrorMessage = "*必填")]
        public int DefectQty { get; set; }

        [Display(Name = "不良率")]
        [Required(ErrorMessage ="*必填")]
        [RegularExpression(@"^[0-9]{1,3}.([0-9]{0,5})?$", ErrorMessage = "請輸入正確的不良率")]
        public string DefectRate { get; set; }


        [Display(Name = "工程師")]
        [Required(ErrorMessage = "*必填")]
        public string Engineer { get; set; }

        [Display(Name = "分類")]
        [Required(ErrorMessage = "*必填")]
        public int PriorityId { get; set; }


        [Display(Name = "機種")]
        public string Product { get; set; }

        [Display(Name = "簡稱")]
        public string ProductShortName { get; set; }

        [Display(Name = "Model Name")]
        public string ModelName { get; set; }


        [Display(Name = "memo")]
        public string Memo { get; set; }
        public int IsPMProcess { get; set; }
        public int IsEngineerProcess { get; set; }
        public string SearchVal { get; set; }

        // 班別
        public SelectList ShiftOptionList { get; set; }

        // 設備-單元
        public SelectList ProcessOptionList { get; set; }

        // 設備-單元
        public SelectList EqUnitOptionList { get; set; }

        // 設備-單元
        public SelectList EqUnitPartOptionList { get; set; }

        // 分類
        public SelectList PriorityOptionList { get; set; }
    }
}
