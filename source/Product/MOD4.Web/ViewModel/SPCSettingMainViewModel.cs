using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MOD4.Web.ViewModel
{
    public class SPCSettingMainViewModel
    {
        public List<SPCSettingViewModel> SettingList { get; set; }

        public UserPermissionViewModel UserPermission { get; set; }
    }
}
