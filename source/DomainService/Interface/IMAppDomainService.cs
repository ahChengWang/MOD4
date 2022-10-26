using MOD4.Web.DomainService.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MOD4.Web.DomainService
{
    public interface IMAppDomainService
    {
        Task SendMsgToOneAsync(string msg, string charSn);
    }
}
