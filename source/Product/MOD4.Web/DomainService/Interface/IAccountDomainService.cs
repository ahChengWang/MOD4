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

        List<AccountInfoEntity> GetAccountInfoByConditions(List<RoleEnum> roleIdList, string name, string jobId, string account, int levelId = 0, List<string> accountList = null);

        AccountInfoEntity GetAccInfoByDepartment(UserEntity userEntity);

        List<AccountInfoEntity> GetAccInfoListByDepartment(List<int> deptList);

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

        string Decrypt(string password, string key = "MOD4_Saikou");

        string SyncDLEmp(string shaKey);
    }
}
