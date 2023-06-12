using MOD4.Web.Enum;
using System.Collections.Generic;
using System.Linq;
using Utility.Helper;

namespace MOD4.Web.Extension.Demand
{
    /// <summary>
    /// status factory
    /// <returns>狀態對應 flow</returns>
    /// </summary>
    public class MTDProcessFactory : IMTDProcessFactory
    {
        private readonly Dictionary<MTDCategoryEnum, string> _dicMTDProcess = new Dictionary<MTDCategoryEnum, string>();

        public MTDProcessFactory()
        {
            _dicMTDProcess = EnumHelper.GetEnumValue<MTDCategoryEnum>().ToDictionary(mtd => mtd, mtd => mtd.GetDescription().Split(" ")[0]);
        }

        public string GetProcess(MTDCategoryEnum mtdCategoryId)
        => _dicMTDProcess[mtdCategoryId];
    }
}
