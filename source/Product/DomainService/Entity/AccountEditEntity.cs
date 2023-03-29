using System.Collections.Generic;

namespace MOD4.Web.DomainService.Entity
{
    public class AccountEditEntity : AccountInfoEntity
    {
        public int MODId { get; set; }

        public int SectionId { get; set; }

        public int DepartmentId { get; set; }

        public List<MenuPermissionEntity> MenuPermissionList { get; set; }
    }
}
