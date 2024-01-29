using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface IAccountInfoRepository
    {

        List<AccountInfoDao> SelectAllAccountInfo();

        List<AccountInfoDao> SelectByConditions(string account = "", 
            string password = "", 
            List<int> accountSnList = null,
            List<int> deptList = null,
            int roleId = 0,
            string name = "",
            string jobId = "",
            int levelId = 0,
            List<string> accountList = null,
            List<string> jobIdList = null);

        AccessFabOrderFlowEntity SelectAccessAuditFlow(int accSn, JobLevelEnum accLevelId, int levelId);

        int UpdateUserAccount(string account, string password);

        int InsertUserAccount(AccountInfoDao insAccountInfo);

        int InsertUserAccountList(List<AccountInfoDao> insAccInfoList);

        int InsertUserPermission(List<AccountMenuInfoDao> insAccountMenuInfo);

        List<DefinitionDepartmentDao> SelectDefinitionDepartment(int deptSn = 0, int parentDeptId = 0);

        List<AccountMenuInfoDao> SelectUserMenuPermission(int accountSn);

        int UpdateUserAccount(AccountInfoDao updAccountInfo);

        int DeleteAccountPermission(int accountSn);

        List<HcmVwEmp01Dao> GetHcmVwEmp01List();

        int DeleteAccountInfo(List<int> accountSnList);

        int DeleteAccountPermissionByList(List<int> accountSnList);

        int DeleteHcm();

        int InsertHcm(List<HcmVwEmp01Dao> hcmList);
    }
}
