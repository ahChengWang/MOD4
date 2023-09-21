using MOD4.Web.Enum;
using System;

namespace MOD4.Web.Repostory.Dao
{
    public class PCESCertificationRecordDao
    {
        public string apply_no { get; set; }
        public string apply_name { get; set; }
        public string shift { get; set; }
        public string main_oper { get; set; }
        public string station { get; set; }
        public string type { get; set; }
        public string mtype { get; set; }
        public string class_name { get; set; }
        public string lic_type { get; set; }
        public CertStatusEnum certStatus { get; set; }
        public CertStatusEnum status { get; set; }
        public DateTime? pass_date { get; set; }
        public DateTime? valid_date { get; set; }
        public int? subj_grade { get; set; }
        public int? skill_grade { get; set; }
        public string eng_no { get; set; }
        public string eng_name { get; set; }
        public CertStatusEnum? skill_status { get; set; }
        public string remark { get; set; }
    }
}
