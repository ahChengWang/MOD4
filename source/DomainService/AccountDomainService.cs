using MOD4.Web.DomainService.Entity;
using MOD4.Web.Repostory;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

namespace MOD4.Web.DomainService
{
    public class AccountDomainService : IAccountDomainService
    {
        private readonly IAccountInfoRepository _accountInfoRepository;

        public AccountDomainService(IAccountInfoRepository accountInfoRepository)
        {
            _accountInfoRepository = accountInfoRepository;
        }


        public AccountInfoEntity GetAccountInfo(string account, string password)
        {
            var dao = _accountInfoRepository.SelectByConditions(account, password).Where(w => w.Password == password).FirstOrDefault();


            if (dao == null)
                return null;

            return new AccountInfoEntity
            {
                sn = dao.sn,
                Name = dao.Name,
                Account = dao.Account,
                Password = dao.Password,
                RoleId = dao.RoleId
            };
        }
    }
}
