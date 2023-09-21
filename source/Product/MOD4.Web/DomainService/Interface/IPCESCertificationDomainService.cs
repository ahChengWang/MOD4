using MOD4.Web.DomainService.Entity;
using System.Collections.Generic;

namespace MOD4.Web.DomainService
{
    public interface IPCESCertificationDomainService
    {
        List<PCESCertRecordEntity> GetRecordPCES(string opr, string station, string prod, string status, string jobId);
    }
}