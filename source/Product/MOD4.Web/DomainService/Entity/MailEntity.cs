using System.Collections.Generic;

namespace MOD4.Web.DomainService.Entity
{
    public class MailEntity
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public List<string> CCUserList { get; set; }
        public string Content { get; set; }
    }
}
