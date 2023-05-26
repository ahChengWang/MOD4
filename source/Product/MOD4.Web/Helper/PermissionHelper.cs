using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.Helper
{
    public static class PermissionHelper
    {
        public static bool CheckPermission<T>(this List<T> value, MenuEnum menuId, PermissionEnum permissionId) where T : AccountMenuInfoEntity
        {
            return Convert.ToBoolean(value.FirstOrDefault(menu => menu.MenuSn == menuId).AccountPermission & (int)permissionId);
        }

    }
}
