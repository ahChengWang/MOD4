using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface IEqEvanCodeMappingRepository
    {
        List<EqEvanCodeMappingDao> SelectList(int typeId = 0, int yId = 0, int subyId = 0, int xId = 0, int subxId = 0, int rId = 0);
    }
}
