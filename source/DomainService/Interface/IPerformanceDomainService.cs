using MOD4.Web.DomainService.Entity;
using System.Collections.Generic;

namespace MOD4.Web.DomainService
{
    public interface IPerformanceDomainService
    {
        List<PassQtyEntity> GetList(string _mfgDTE = "", string _shift = "", string _nodeList = "");
    }
}
