using MOD4.Web.DomainService.Entity;

namespace MOD4.Web.DomainService
{
    public interface IAccountDomainService
    {
        AccountInfoEntity GetAccountInfo(string account, string password);

        void InsertUpdateAccountInfo(string account, string password);

        bool VerifyInxSSO(string account, string password);
    }
}
