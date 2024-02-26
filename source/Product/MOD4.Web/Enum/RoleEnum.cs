using System;
using System.ComponentModel;

namespace MOD4.Web.Enum
{
    [Flags]
    public enum RoleEnum
    {
        [Description("系統管理者")]
        SysAdmin = 1,

        [Description("管理者")]
        Manager = 2,

        [Description("課長")]
        Leader = 4,

        [Description("工程師")]
        Engineer = 8,

        [Description("設備副工程師")]
        PM = 16,

        [Description("User")]
        User = 32,

        [Description("需求系統管理者")]
        DemandSysMgr = 64,

        [Description("管制口人員")]
        GateEmployee = 128,

        [Description("TA")]
        TechnicalAssistant = 256,

        [Description("組長")]
        TeamLeader = 512,

        [Description("領班")]
        Foreman = 1024,

        [Description("IE layout 審核")]
        IELayoutOpr = 2048,
    }
}
