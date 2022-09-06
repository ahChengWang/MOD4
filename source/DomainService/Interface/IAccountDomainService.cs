using MOD4.Web.DomainService.Entity;

namespace MOD4.Web.DomainService
{
    public interface IAccountDomainService
    {
        AccountInfoEntity GetAccountInfo(string account, string password);
    }
}
