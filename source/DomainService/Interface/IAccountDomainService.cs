using MOD4.Web.DomainService.Entity;
using System.Collections.Generic;

namespace MOD4.Web.DomainService
{
    public interface IAccountDomainService
    {
        List<AccountInfoEntity> GetAllAccountInfo();

        List<AccountInfoEntity> GetAccountInfo(List<int> accountSnList);

        AccessFabOrderFlowEntity GetAuditFlowInfo(UserEntity userEntity);

        AccountInfoEntity GetAccInfoByDepartment(UserEntity userEntity);

        List<AccountMenuInfoEntity> GetUserAllMenuPermission(int userAccountSn);

        void InsertUpdateAccountInfo(string account, string password);

        bool VerifyInxSSO(string account, string password);
    }
}
