using MOD4.Web.Enum;
using System;
using System.Collections.Generic;

namespace MOD4.Web.DomainService.Entity
{
    public class BulletinEntity
    {
        public int SerialNo { get; set; }
        public string Date { get; set; }
        public string AuthorName { get; set; }
        public string Subject { get; set; }
        public string Status { get; set; }
        public string Content { get; set; }
        public int ReadCount { get; set; }
        public string FileName { get; set; }
        public List<string> TargetSections { get; set; }
        public string TargetSectionStr { get; set; }
        public bool IsNewPost { get; set; }

        // detail 判斷是否更新已讀
        public bool IsNeedUpdate { get; set; }
    }
}
