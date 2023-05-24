using System.ComponentModel.DataAnnotations;

namespace MOD4.Web.ViewModel
{
    public class MESApplicantModel
    {
        [Display(Name = "姓名")]
        public string ApplicantName { get; set; }

        [Display(Name = "工號")]
        public string ApplicantJobId { get; set; }
    }
}
