using Microsoft.AspNetCore.Http;
using MOD4.Web.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MOD4.Web.ViewModel
{
    public class AccessFabDetailPageViewModel
    {
        public int OrderSn { get; set; }

        [Display(Name = "單號")]
        public string AccessFabOrderNo { get; set; }

        [Display(Name = "狀態")]
        public string OrderStatus { get; set; }
                
        public FabInOutStatusEnum OrderStatusId { get; set; }

        [Display(Name = "入場性質")]
        public FabInTypeEnum FabInTypeId { get; set; }

        [Display(Name = "其他")]
        public string FabInOtherType { get; set; }

        [Display(Name = "開單人")]
        public string FillOutPerson { get; set; }

        [Display(Name = "申請人姓名")]
        public string Applicant { get; set; }

        [Display(Name = "申請人連絡電話")]
        public string ApplicantMVPN { get; set; }

        [Display(Name = "申請人工號")]
        public string JobId { get; set; }

        [Display(Name = "入場對象")]
        public FabInCategoryEnum FabInCategoryId { get; set; }

        [Display(Name = "入廠目的描述")]
        public string Content { get; set; }

        [Display(Name = "入廠動線")]
        public string Route { get; set; }

        [Display(Name = "入廠日期")]
        public DateTime FabInDate { get; set; }

        [Display(Name = "離廠日期")]
        public DateTime FabOutDate { get; set; }

        [Display(Name = "陪同人員")]
        public string AccompanyingPerson { get; set; }

        [Display(Name = "陪同人連絡電話")]
        public string AccompanyingPersonMVPN { get; set; }

        [Display(Name = "附件上傳")]
        public List<IFormFile> UploadFile { get; set; }

        [Display(Name = "申請日")]
        public string CreateTimeStr { get; set; }

        [Display(Name = "備註")]
        public string Remark { get; set; }

        public List<AccessFabPersonsViewModel> AccessPersonList { get; set; }

        public List<AccessFabAuditHistoryViewModel> AuditHistory { get; set; }
    }
}