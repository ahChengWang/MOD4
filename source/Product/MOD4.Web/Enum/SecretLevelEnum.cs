using System;
using System.ComponentModel;

namespace MOD4.Web.Enum
{
    public enum SecretLevelEnum
    {
        [Description("一般")]
        Normal = 1,

        [Description("機密")]
        Secret = 2,

        [Description("極機密")]
        ExSecret = 3,
    }
}
