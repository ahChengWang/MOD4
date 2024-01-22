using System.ComponentModel.DataAnnotations;

namespace MOD4.Web.ViewModel
{
    public class EquipmentDetailViewModel
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
        public string Shift { get; set; }

        [Display(Name = "Process")]
        public string Process { get; set; }

        [Display(Name = "Unit")]
        public string EqUnit { get; set; }

        [Display(Name = "Unit-Part")]
        public string EqUnitPart { get; set; }

        [Display(Name = "ME\\PE")]
        public string Type { get; set; }

        [Display(Name = "Y")]
        public string Y { get; set; }

        [Display(Name = "y")]
        public string SubY { get; set; }

        [Display(Name = "X")]
        public string X { get; set; }

        [Display(Name = "x")]
        public string SubX { get; set; }

        [Display(Name = "R")]
        public string R { get; set; }

        [Display(Name = "維修人員")]
        public string MntUser { get; set; }

        [Display(Name = "維修記錄")]
        public string MntMinutes { get; set; }

        [Display(Name = "不良品數")]
        public int DefectQty { get; set; }

        [Display(Name = "不良率")]
        public string DefectRate { get; set; }

        [Display(Name = "ME\\PE")]
        public string ENGType { get; set; }

        [Display(Name = "Y")]
        public string ENGY { get; set; }

        [Display(Name = "y")]
        public string ENGSubY { get; set; }

        [Display(Name = "X")]
        public string ENGX { get; set; }

        [Display(Name = "x")]
        public string ENGSubX { get; set; }

        [Display(Name = "R")]
        public string ENGR { get; set; }

        [Display(Name = "工程師")]
        public string Engineer { get; set; }

        [Display(Name = "分類")]
        public string Priority { get; set; }

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
    }
}
