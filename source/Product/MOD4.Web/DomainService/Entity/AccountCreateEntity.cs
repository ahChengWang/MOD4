using MOD4.Web.Enum;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MOD4.Web.DomainService.Entity
{
    public class AccountCreateEntity : AccountInfoEntity
    {
        public int MODId { get; set; }

        public int SectionId { get; set; }

        public int DepartmentId { get; set; }

        public List<AccountMenuInfoEntity> MenuPermissionList { get; set; }
    }
}
