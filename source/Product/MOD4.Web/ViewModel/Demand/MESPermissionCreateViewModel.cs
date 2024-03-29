﻿using Microsoft.AspNetCore.Http;
using MOD4.Web.Enum;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MOD4.Web.ViewModel
{
    public class MESPermissionCreateViewModel
    {
        public int OrderSn { get; set; }

        [Display(Name = "訂單編號")]
        public string OrderNo { get; set; }

        [Display(Name = "部門")]
        [Required(ErrorMessage = "必填")]
        public string Department { get; set; }

        [Display(Name = "單位")]
        [Required(ErrorMessage = "必填")]
        public string SubUnit { get; set; }

        [Display(Name = "類別")]
        [Required(ErrorMessage = "必填")]
        public MESOrderTypeEnum MESOrderTypeId { get; set; }

        public string MESOrderType { get; set; }

        [Display(Name = "申請人")]
        [Required(ErrorMessage = "必填")]
        public string Applicant { get; set; }

        [Display(Name = "工號")]
        [Required(ErrorMessage = "必填")]
        public string JobId { get; set; }

        [Display(Name = "電話")]
        [Required(ErrorMessage = "必填")]
        public string Phone { get; set; }

        [Display(Name = "申請原因")]
        [Required(ErrorMessage = "必填")]
        public string ApplicantReason { get; set; }

        [Display(Name = "人員名單")]
        public IFormFile UploadFile { get; set; }

        [Display(Name = "申請名單")]
        public List<MESApplicantModel> ApplicantList { get; set; }

        public List<MESPermissionModel> PermissionList { get; set; }

        [Display(Name = "其他權限")]
        public string OtherPermission { get; set; }

        [Display(Name = "權限同姓名")]
        public string SameEmpName { get; set; }

        [Display(Name = "權限同工號")]
        public string SameEmpJobId { get; set; }

        [Display(Name = "申請日")]
        public string CreateDate { get; set; }

        public DemandStatusEnum StatusId { get; set; }
    }
}
