using System.ComponentModel;

namespace MOD4.Web.Enum
{
    public enum DemandCategoryEnum
    {
        [Description("設定")]
        Setting = 1,

        [Description("新增")]
        NewItems = 2,

        [Description("其他")]
        Other = 3
    }
}
