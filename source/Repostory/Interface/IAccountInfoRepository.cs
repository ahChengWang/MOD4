using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface IAccountInfoRepository
    {
        List<AccountInfoDao> SelectByConditions(string account = "", 
            string password = "", 
            List<int> accountSnList = null, 
            int deptSn = 0, 
            List<RoleEnum> roleIdList = null,
            string name = "",
            string jobId = "");

        AccessFabOrderFlowEntity SelectAccessAuditFlow(int accSn, JobLevelEnum accLevelId, int levelId);

        int UpdateUserAccount(string account, string password);

        int InsertUserAccount(AccountInfoDao insAccountInfo);

        int InsertUserPermission(List<AccountMenuInfoDao> insAccountMenuInfo);

        List<DefinitionDepartmentDao> SelectDefinitionDepartment(int deptSn = 0, int parentDeptId = 0);

        List<AccountMenuInfoDao> SelectUserMenuPermission(int accountSn);

        int UpdateUserAccount(AccountInfoDao updAccountInfo);

        int DeleteAccountPermission(int accountSn);
    }
}
