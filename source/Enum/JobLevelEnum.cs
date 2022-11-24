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

        [Description("員工")]
        Employee = 4,
    }
}
