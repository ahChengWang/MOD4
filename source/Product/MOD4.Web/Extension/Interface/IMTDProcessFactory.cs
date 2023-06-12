using MOD4.Web.Enum;

namespace MOD4.Web.Extension.Demand
{
    public interface IMTDProcessFactory
    {
        string GetProcess(MTDCategoryEnum mtdCategoryId);
    }
}