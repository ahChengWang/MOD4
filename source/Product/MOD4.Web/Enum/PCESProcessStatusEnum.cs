using System.ComponentModel;

namespace MOD4.Web.Enum
{
    public enum CertStatusEnum
    {
        [Description("通過")]
        Pass = 1,
        [Description("未通過")]
        Failed = 2,
        [Description("認證中")]
        InProgress = 3,
        [Description("過期")]
        Expied = 4,
        [Description("取消")]
        Cancel = 5
    }
}
