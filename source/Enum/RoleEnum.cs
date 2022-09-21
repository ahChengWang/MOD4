using System.ComponentModel;

namespace MOD4.Web.Enum
{
    public enum RoleEnum
    {
        [Description("管理者")]
        Admin = 1,

        [Description("經理")]
        Manager = 2,

        [Description("組長")]
        Leader = 3,

        [Description("工程師")]
        Engineer = 4,

        [Description("PM")]
        PM = 5,

        [Description("User")]
        User = 6,
    }
}
