using Microsoft.AspNetCore.Http;
using MOD4.Web.Enum;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MOD4.Web.ViewModel
{
    public class DemanEditViewModel
    {
        public int OrderSn { get; set; }

        [Display(Name = "單號")]
        public string OrderId { get; set; }

        [Display(Name = "系統類型")]
        //public string DemandCategory { get; set; }
        public DemandCategoryEnum DemandCategoryId { get; set; }

        [Display(Name = "狀態")]
        public string DemandStatus { get; set; }
        public int DemandStatusId { get; set; }

        [Display(Name = "主旨")]
        public string Subject { get; set; }

        [Display(Name = "需求內容")]
        public string Content { get; set; }

        [Display(Name = "申請人姓名")]
        public string Applicant { get; set; }

        [Display(Name = "申請人工號")]
        public string JobNo { get; set; }

        [Display(Name = "申請日")]
        public string CreateDate { get; set; }

        [Display(Name = "申請人上傳檔案")]
        public string UploadFile1 { get; set; }

        public string UploadFile2 { get; set; }

        public string UploadFile3 { get; set; }



        [Display(Name = "駁回原因")]
        public string RejectReason { get; set; }

        [Display(Name = "上傳檔案")]
        public List<IFormFile> UploadFile { get; set; }


        [Display(Name = "備註")]
        public string Remark { get; set; }

        [Display(Name = "負責人上傳檔案")]
        public string CompleteUploadFile1 { get; set; }

        public string CompleteUploadFile2 { get; set; }

        public string CompleteUploadFile3 { get; set; }
    }
}