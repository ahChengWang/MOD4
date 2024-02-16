using System;
using System.ComponentModel;

namespace MOD4.Web.Enum
{
    public enum ReasonTypeEnum
    {
        [Description("Move-in Path")]
        MoveInPath = 1,

        [Description("自己部門參考")]
        Internal = 2,

        [Description("給予 Vendor")]
        Vendor = 3,

        [Description("其他")]
        Other = 4,
    }
}
