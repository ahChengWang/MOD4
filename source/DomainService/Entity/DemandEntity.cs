using Microsoft.AspNetCore.Http;
using MOD4.Web.Enum;
using System;
using System.Collections.Generic;

namespace MOD4.Web.DomainService.Entity
{
    public class DemandEntity
    {
        public int OrderSn { get; set; }
        public string OrderNo { get; set; }
        public DemandCategoryEnum CategoryId { get; set; }
        public string Category { get; set; }
        public DemandStatusEnum StatusId { get; set; }
        public string Status { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string Applicant { get; set; }
        public string JobNo { get; set; }
        public string UploadFiles { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateTimeStr { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateTime { get; set; }
        public List<IFormFile> UploadFileList { get; set; }
        public string UploadFile1 { get; set; }
        public string UploadFile2 { get; set; }
        public string UploadFile3 { get; set; }

        public string RejectReason { get; set; }
    }
}
