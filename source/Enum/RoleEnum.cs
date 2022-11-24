using System.ComponentModel;

namespace MOD4.Web.Enum
{
    public enum RoleEnum
    {
        [Description("系統管理者")]
        SysAdmin = 1,

        [Description("管理者")]
        Manager = 2,

        [Description("課長")]
        Leader = 3,

        [Description("工程師")]
        Engineer = 4,

        [Description("PM")]
        PM = 5,

        [Description("User")]
        User = 6,

        [Description("需求系統管理者")]
        DemandSysMgr = 7,

        [Description("管制口管理者")]
        ControlSysMgr = 8,
    }
}
