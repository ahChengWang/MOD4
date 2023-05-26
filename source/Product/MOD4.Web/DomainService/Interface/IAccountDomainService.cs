using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using System.Collections.Generic;

namespace MOD4.Web.DomainService
{
    public interface IAccountDomainService
    {
        List<AccountInfoEntity> GetAllAccountInfo();

        List<AccountInfoEntity> GetAccountInfo(List<int> accountSnList);

        AccessFabOrderFlowEntity GetAuditFlowInfo(UserEntity userEntity);

        List<AccountInfoEntity> GetAccountInfoByConditions(List<RoleEnum> roleIdList, string name, string jobId, string account);

        AccountInfoEntity GetAccInfoByDepartment(UserEntity userEntity);

        List<AccountMenuInfoEntity> GetUserAllMenuPermission(int userAccountSn);

        List<AccountDeptEntity> GetAccountDepartmentList();

        AccountEditEntity GetAccountAndMenuInfo(int accountSn);
        string GetToken(string account, string password);

        string VerifyToken(InxSSOEntity inxSSOEntity);

        (bool, AccountInfoEntity) DoUserVerify(LoginEntity loginEntity, string shaKey, bool requiredVerify);

        string Create(AccountCreateEntity createEntity);

        string Update(AccountCreateEntity updateEntity);

        //void InsertUpdateAccountInfo(AccountInfoEntity accInfoEntity);

        bool VerifyInxSSO(string account, string password);
    }
}
