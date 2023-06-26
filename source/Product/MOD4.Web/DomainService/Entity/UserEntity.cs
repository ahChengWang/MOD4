using MOD4.Web.Enum;
using System.Collections.Generic;

namespace MOD4.Web.DomainService.Entity
{
    public class UserEntity
    {
        public int sn { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public RoleEnum RoleId { get; set; }
        public JobLevelEnum Level_id { get; set; }
        public int DeptSn { get; set; }
        public string Mail { get; set; }
        public string JobId { get; set; }
        public List<AccountMenuInfoEntity> UserMenuPermissionList { get; set; }
        public List<MenuEntity> UserMenuList { get; set; }
    }
}
