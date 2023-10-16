using MOD4.Web.Enum;

namespace MOD4.Web.ViewModel
{
    public class PCESCertRecordViewModel
    {
        public string ApplyNo { get; set; }
        public string ApplyName { get; set; }
        public string Shift { get; set; }
        public string MainOperation { get; set; }
        public string Station { get; set; }
        public string StationType { get; set; }
        public string Mtype { get; set; }
        public string ClassName { get; set; }
        public string LicType { get; set; }
        public CertStatusEnum CertStatusId { get; set; }
        public string CertStatus { get; set; }
        public CertStatusEnum StatusId { get; set; }
        public string Status { get; set; }
        public string PassDate { get; set; }
        public string ValidDate { get; set; }
        public int SubjGrade { get; set; }
        public int SkillGrade { get; set; }
        public string EngNo { get; set; }
        public string EngName { get; set; }
        public string SkillStatus { get; set; }
        public CertStatusEnum SkillStatusId { get; set; }
        public string Remark { get; set; }

        // 給前端判斷按鈕狀態用
        public bool IsGeneealClass { get; set; }
        //public bool IsCancelDisabled { get; set; } = true;
        //public bool IsSaveDisabled { get; set; } = true;
    }
}
