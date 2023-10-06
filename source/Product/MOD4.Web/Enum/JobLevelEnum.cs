using System.ComponentModel;

namespace MOD4.Web.Enum
{
    public enum JobLevelEnum
    {
        [Description("廠長")]
        FactoryManager = 1,

        [Description("部門經理")]
        DepartmentManager = 2,

        [Description("課長")]
        SectionManager = 3,

        [Description("工程師")]
        Employee = 4,

        [Description("DL")]
        DL = 5,
    }
}
