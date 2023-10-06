using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MOD4.Web.Repostory
{
    public class AccountInfoRepository : BaseRepository, IAccountInfoRepository
    {

        public List<AccountInfoDao> SelectByConditions(string account = "", 
            string password = "", 
            List<int> accountSnList = null, 
            List<int> deptList = null,
            List<RoleEnum> roleIdList = null,
            string name = "",
            string jobId = "")
        {
            string sql = "select * from account_info where 1=1 ";

            if (!string.IsNullOrEmpty(account))
            {
                sql += " and account=@account ";
            }
            if (!string.IsNullOrEmpty(password))
            {
                sql += " and password=@password ";
            }
            if (accountSnList != null)
            {
                sql += " and sn in @SnList ";
            }
            if (deptList != null && deptList.Any())
            {
                sql += " and deptSn in @deptList ";
            }
            if (roleIdList != null)
            {
                sql += " and role in @role ";
            }
            if (!string.IsNullOrEmpty(name))
            {
                sql += " and name = @name ";
            }
            if (!string.IsNullOrEmpty(jobId))
            {
                sql += " and jobId = @jobId ";
            }

            var dao = _dbHelper.ExecuteQuery<AccountInfoDao>(sql, new
            {
                account = account,
                password = password,
                SnList = accountSnList,
                deptSn = deptList,
                role = roleIdList,
                name = name,
                jobId = jobId
            });

            return dao;
        }

        public AccessFabOrderFlowEntity SelectAccessAuditFlow(int accSn, JobLevelEnum accJobLevelId, int levelId)
        {
            string sql = @"
select 
 dd1.departmentName 'topTitleName',ai1.sn 'topAccSn', ai1.mail 'topMail', ai1.name 'topName' {0}{1}
from definition_department dd1 
left join account_info ai1 
on dd1.deptSn = ai1.deptSn and ai1.level_id = 1 ";

            // 部級
            if (levelId == 2)
            {
                sql += @" 
 join definition_department dd2 on dd1.deptSn = dd2.parentDeptId 
  and dd1.levelId = 1 and dd2.levelId = 2 
 left join account_info ai2 
   on dd2.deptSn = ai2.deptSn and ai2.level_id = 2 
where ai2.sn = @accSn ";
                sql = string.Format(sql, "", "");
            }
            // 課級 & 主管
            else if (levelId == 3 && accJobLevelId == JobLevelEnum.SectionManager)
            {
                sql += @" 
 join definition_department dd2 on dd1.deptSn = dd2.parentDeptId 
  and dd1.levelId = 1 and dd2.levelId = 2 
 left join account_info ai2 
   on dd2.deptSn = ai2.deptSn and ai2.level_id = 2 
 join definition_department dd3 on dd2.deptSn = dd3.parentDeptId 
  and dd3.levelId = 3 
 left join account_info ai3 
   on dd3.deptSn = ai3.deptSn and ai3.level_id = 3  
where ai3.sn = @accSn ";
                sql = string.Format(sql, ", dd2.departmentName 'deptTitleName',ai2.sn 'deptAccSn', ai2.mail 'deptMail', ai2.name 'deptName' ", "");
            }
            // 課級 & 員工
            else if (levelId == 3 && accJobLevelId == JobLevelEnum.Employee)
            {
                sql += @" 
 join definition_department dd2 on dd1.deptSn = dd2.parentDeptId 
  and dd1.levelId = 1 and dd2.levelId = 2 
 left join account_info ai2 
   on dd2.deptSn = ai2.deptSn and ai2.level_id = 2 
 join definition_department dd3 on dd2.deptSn = dd3.parentDeptId 
  and dd3.levelId = 3 
 left join account_info ai3 
   on dd3.deptSn = ai3.deptSn and ai3.level_id = 3 
 left join account_info ai4
   on dd3.deptSn = ai4.deptSn and ai4.level_id = 4 
where ai4.sn = @accSn ";
                sql = string.Format(sql, ", dd2.departmentName 'deptTitleName',ai2.sn 'deptAccSn', ai2.mail 'deptMail', ai2.name 'deptName' ",
                    ", dd3.departmentName 'sectionTitleName',ai3.sn 'sectionAccSn', ai3.mail 'sectionMail', ai3.name 'sectionName' ");
            }
            else
            {
                sql = string.Format(sql, "", "");
            }

            var dao = _dbHelper.ExecuteQuery<AccessFabOrderFlowEntity>(sql, new
            {
                accSn = accSn
            }).FirstOrDefault();

            return dao;
        }

        public int UpdateUserAccount(string account, string password)
        {
            string sql = "Update account_info set password=@password where account=@account ";

            var dao = _dbHelper.ExecuteNonQuery(sql, new
            {
                account = account,
                password = password
            });

            return dao;
        }

        public int InsertUserAccount(AccountInfoDao insAccountInfo)
        {
            string sql = @"INSERT INTO [dbo].[account_info]
([account],
[password],
[name],
[role],
[level_id],
[jobId],
[apiKey],
[deptSn],
[mail])
VALUES
(@account,
@password,
@name,
@role,
@level_id,
@jobId,
@apiKey,
@deptSn,
@mail); ";

            var dao = _dbHelper.ExecuteNonQuery(sql, insAccountInfo);

            return dao;
        }

        public int InsertUserPermission(List<AccountMenuInfoDao> insAccountMenuInfo)
        {
            try
            {
                string sql = @"
INSERT INTO [dbo].[account_menu_info] 
([account_sn],
[menu_sn],
[menu_group_sn],
[account_permission])
VALUES
(@account_sn,
@menu_sn,
@menu_group_sn,
@account_permission);";

                var dao = _dbHelper.ExecuteNonQuery(sql, insAccountMenuInfo);

                return dao;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<DefinitionDepartmentDao> SelectDefinitionDepartment(int deptSn = 0, int parentDeptId = 0)
        {
            string sql = "select * from definition_department where 1=1 ";

            if (deptSn != 0)
            {
                sql += " and deptSn=@DeptSn ";
            }
            if (parentDeptId != 0)
            {
                sql += " and parentDeptId=@ParentDeptId ";
            }

            var dao = _dbHelper.ExecuteQuery<DefinitionDepartmentDao>(sql, new
            {
                DeptSn = deptSn,
                ParentDeptId = parentDeptId
            });

            return dao;
        }

        public List<AccountMenuInfoDao> SelectUserMenuPermission(int accountSn)
        {
            string sql = @" select menu.page_name,ami.* 
  from menu_info menu 
  join account_menu_info ami on menu.sn = ami.menu_sn 
 where ami.account_sn = @account_sn and menu.href != '#' ; ";

            var dao = _dbHelper.ExecuteQuery<AccountMenuInfoDao>(sql, new
            {
                account_sn = accountSn
            });

            return dao;
        }

        public int UpdateUserAccount(AccountInfoDao updAccountInfo)
        {
            string sql = @"Update [dbo].[account_info] 
SET name = @name,
role = @role,
level_id = @level_id,
jobId = @jobId,
apiKey = @apiKey,
deptSn = @deptSn,
mail = @mail  
where sn = @sn ; ";

            var dao = _dbHelper.ExecuteNonQuery(sql, updAccountInfo);

            return dao;
        }


        public int DeleteAccountPermission(int accountSn)
        {
            string sql = "DELETE [dbo].[account_menu_info] WHERE account_sn = @account_sn and menu_sn != 15; ";

            var dao = _dbHelper.ExecuteNonQuery(sql, new 
            {
                account_sn = accountSn
            });

            return dao;
        }
    }
}
