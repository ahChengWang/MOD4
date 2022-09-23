using MOD4.Web.DomainService.Entity;
using System.Collections.Generic;

namespace MOD4.Web.DomainService
{
    public interface IAccountDomainService
    {
        List<AccountInfoEntity> GetAllAccountInfo();

        AccountInfoEntity GetAccountInfo(string account, string password);

        void InsertUpdateAccountInfo(string account, string password);

        bool VerifyInxSSO(string account, string password);
    }
}
