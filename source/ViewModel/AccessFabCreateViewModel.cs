using Microsoft.AspNetCore.Http;
using MOD4.Web.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MOD4.Web.ViewModel
{
    public class AccessFabCreateViewModel
    {
        [Display(Name = "入場性質")]
        public FabInTypeEnum FabInTypeId { get; set; }

        [Display(Name = "其他")]
        public string FabInOtherType { get; set; }

        [Display(Name = "申請人姓名")]
        [RegularExpression(@"^[\u4e00-\u9fa5a-zA-Z.\s]+$", ErrorMessage = "輸入中英文")]
        [Required(ErrorMessage = "必填")]
        public string Applicant { get; set; }

        [Display(Name = "申請人連絡電話")]
        [RegularExpression(@"^[0-9\-,]*$", ErrorMessage = "輸入數字或-或,")]
        public string ApplicantMVPN { get; set; }

        [Display(Name = "申請人工號")]
        [RegularExpression(@"^[0-9]{8}?$", ErrorMessage = "*輸入8位數字")]
        [Required(ErrorMessage = "必填")]
        public string JobId { get; set; }

        [Display(Name = "入場對象")]
        [Required(ErrorMessage = "必填")]
        public FabInCategoryEnum FabInCategoryId { get; set; }

        [Display(Name = "入廠目的描述")]
        [Required(ErrorMessage = "必填")]
        public string Content { get; set; }

        [Display(Name = "入廠動線")]
        [Required(ErrorMessage = "必填")]
        public string Route { get; set; }

        [Display(Name = "入廠日期")]
        [Required(ErrorMessage = "必填")]
        public DateTime FabInDate { get; set; }

        [Display(Name = "離廠日期")]
        [Required(ErrorMessage = "必填")]
        public DateTime FabOutDate { get; set; }

        [Display(Name = "陪同人員")]
        [Required(ErrorMessage = "必填")]
        public string AccompanyingPerson { get; set; }

        [Display(Name = "附件上傳")]
        public List<IFormFile> UploadFile { get; set; }

        public List<AccessFabCreateDetailViewModel> Details { get; set; }
    }
}