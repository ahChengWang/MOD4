using System.ComponentModel;

namespace MOD4.Web.Enum
{
    public enum JobTitleEnum
    {
        [Description("副理")]
        M01 = 1,
        [Description("資深副理")]
        M02 = 2,
        [Description("經理")]
        M03 = 3,

        [Description("工程師")]
        E03 = 21,
        [Description("高級工程師")]
        E04 = 22,
        [Description("主任工程師")]
        E05 = 23,
        [Description("技術副理")]
        E07 = 24,

        [Description("助理技術員")]
        T01 = 41,
        [Description("技術員")]
        T02 = 42,
        [Description("高級技術員")]
        T03 = 43,
        [Description("正技術員")]
        T04 = 44,
        [Description("工程技術員")]
        T17 = 45,
        [Description("高級工程技術員")]
        T18 = 46,
        [Description("正工程技術員")]
        T19 = 47,
        [Description("工程領班")]
        T21 = 48,
        [Description("設備領班")]
        T23 = 49,
        [Description("設備副工程師")]
        T28 = 50,
        [Description("高級設備副工程師")]
        T29 = 51,
        [Description("資深設備副工程師")]
        T30 = 52,

        [Description("領班")]
        T_29 = 71,
        [Description("資深領班")]
        T_30 = 72,
    }
}
