﻿using Microsoft.AspNetCore.Mvc.Rendering;
using MOD4.Web.Enum;
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
        [Required(ErrorMessage = "必填")]
        public string Comment { get; set; }

        [Display(Name = "當機時間")]
        public string StartTime { get; set; }

        [Display(Name = "修復時間")]
        public decimal RepairedTime { get; set; }

        [Display(Name = "日期")]
        public string MFGDay { get; set; }
        public string MFGHr { get; set; }

        [Display(Name = "班別")]
        [Required(ErrorMessage = "必填")]
        public int Shift { get; set; }

        [Display(Name = "Process")]
        [Required(ErrorMessage = "必填")]
        public int ProcessId { get; set; }

        [Display(Name = "Unit")]
        [Required(ErrorMessage = "必填")]
        public int EqUnitId { get; set; }

        [Display(Name = "Unit-Part")]
        [Required(ErrorMessage = "必填")]
        public int EqUnitPartId { get; set; }


        [Display(Name = "維修人員")]
        [Required(ErrorMessage = "必填")]
        public string MntUser { get; set; }

        [Display(Name = "維修記錄")]
        [Required(ErrorMessage = "必填")]
        public string MntMinutes { get; set; }

        [Display(Name = "不良品數")]
        //[Required(ErrorMessage = "必填")]
        public int DefectQty { get; set; }

        [Display(Name = "不良率")]
        //[Required(ErrorMessage ="必填")]
        [RegularExpression(@"^(\+)?\d+(\.\d+)?$", ErrorMessage = "請輸入正確數值")]
        public string DefectRate { get; set; }

        [Display(Name = "ME\\PE")]
        [Required(ErrorMessage = "必填")]
        public int TypeId { get; set; }
        public string TypeDesc { get; set; }

        [Display(Name = "Y")]
        [Required(ErrorMessage = "必填")]
        public int YId { get; set; }
        public string YDesc { get; set; }

        [Display(Name = "y")]
        [Required(ErrorMessage = "必填")]
        public int SubYId { get; set; }
        public string SubYDesc { get; set; }

        [Display(Name = "X")]
        [Required(ErrorMessage = "必填")]
        public int XId { get; set; }
        public string XDesc { get; set; }

        [Display(Name = "x")]
        [Required(ErrorMessage = "必填")]
        public int SubXId { get; set; }
        public string SubXDesc { get; set; }

        [Display(Name = "R")]
        [Required(ErrorMessage = "必填")]
        public int RId { get; set; }
        public string RDesc { get; set; }


        [Display(Name = "工程師")]
        [Required(ErrorMessage = "必填")]
        public string Engineer { get; set; }

        [Display(Name = "分類")]
        [Required(ErrorMessage = "必填")]
        public int PriorityId { get; set; }


        [Display(Name = "機種")]
        public string Product { get; set; }

        [Display(Name = "簡稱")]
        public string ProductShortName { get; set; }

        [Display(Name = "Model Name")]
        public string ModelName { get; set; }

        [Display(Name = "memo")]
        public string Memo { get; set; }


        [Display(Name = "ME\\PE")]
        [Required(ErrorMessage = "必填")]
        public int ENGTypeId { get; set; }
        public string ENGTypeDesc { get; set; }

        [Display(Name = "Y")]
        [Required(ErrorMessage = "必填")]
        public int ENGYId { get; set; }
        public string ENGYDesc { get; set; }

        [Display(Name = "y")]
        [Required(ErrorMessage = "必填")]
        public int ENGSubYId { get; set; }
        public string ENGSubYDesc { get; set; }

        [Display(Name = "X")]
        [Required(ErrorMessage = "必填")]
        public int ENGXId { get; set; }
        public string ENGXDesc { get; set; }

        [Display(Name = "x")]
        [Required(ErrorMessage = "必填")]
        public int ENGSubXId { get; set; }
        public string ENGSubXDesc { get; set; }

        [Display(Name = "R")]
        [Required(ErrorMessage = "必填")]
        public int ENGRId { get; set; }
        public string ENGRDesc { get; set; }


        public int IsPMProcess { get; set; }
        public int IsEngineerProcess { get; set; }
        public EqIssueStatusEnum StatusId { get; set; }
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

        // Even Code
        public SelectList EvenCodeOptionList { get; set; }

        // Even Code Y option
        public SelectList EvenCodeYOptionList { get; set; }

        // Even Code subY option
        public SelectList EvenCodeSubYOptionList { get; set; }

        // Even Code X option
        public SelectList EvenCodeXOptionList { get; set; }

        // Even Code subX option
        public SelectList EvenCodeSubXOptionList { get; set; }

        // Even Code R option
        public SelectList EvenCodeROptionList { get; set; }
    }
}
