using System;

namespace MOD4.Web.Enum
{
    [Flags]
    public enum PermissionEnum
    {
        Create = 1,

        Update = 2,

        Audit = 4,

        Reject = 8,

        Cancel = 16,

        Management = 32
    }
}
