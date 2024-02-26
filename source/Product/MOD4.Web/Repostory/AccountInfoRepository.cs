using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.Attributes;

namespace MOD4.Web.Repostory
{
    public class AccountInfoRepository : BaseRepository, IAccountInfoRepository
    {
        [Cache]
        [CacheOption(Second = 60 * 60 * 24 * 3)]
        public virtual List<AccountInfoDao> SelectAllAccountInfo()
        {
            string sql = "select * from account_info where 1=1 ";

            var dao = _dbHelper.ExecuteQuery<AccountInfoDao>(sql);

            return dao;
        }


        public List<AccountInfoDao> SelectByConditions(string account = "",
            string password = "",
            List<int> accountSnList = null,
            List<int> deptList = null,
            int roleId = 0,
            string name = "",
            string jobId = "",
            int levelId = 0,
            List<string> accountList = null,
            List<string> jobIdList = null)
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
            if (roleId != 0)
            {
                sql += " and role = @role ";
            }
            if (!string.IsNullOrEmpty(name))
            {
                sql += " and name = @name ";
            }
            if (!string.IsNullOrEmpty(jobId))
            {
                sql += " and jobId = @jobId ";
            }
            if (levelId != 0)
            {
                sql += " and level_id = @Level_id ";
            }
            if (accountList != null && accountList.Any())
            {
                sql += " and account in @AccountList ";
            }
            if (jobIdList != null && jobIdList.Any())
            {
                sql += " and jobId in @JobIdList ";
            }

            var dao = _dbHelper.ExecuteQuery<AccountInfoDao>(sql, new
            {
                account = account,
                password = password,
                SnList = accountSnList,
                deptList = deptList,
                role = roleId,
                name = name,
                jobId = jobId,
                Level_id = levelId,
                AccountList = accountList,
                JobIdList = jobIdList
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
            else if (levelId == 3 && (accJobLevelId == JobLevelEnum.Employee || accJobLevelId == JobLevelEnum.DL))
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
   on dd3.deptSn = ai4.deptSn and (ai4.level_id = 4 or ai4.level_id = 5)
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

        public int InsertUserAccountList(List<AccountInfoDao> insAccInfoList)
        {
            try
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

                var dao = _dbHelper.ExecuteNonQuery(sql, insAccInfoList);

                return dao;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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

        public List<HcmVwEmp01Dao> GetHcmVwEmp01List()
        {
            try
            {
                //PKTXT not like '%IDL%' and 
                string sql = @" select * from hcm_vw_emp01 where STAT2TXT = '在職中' and ((PTEXT = '群豐駿南科'
 and OSHORT in ('9O431500','9O432500','9O433500','9J410500','9J410500','9P128500',
'9O431501','9O431502','9O431503','9O431504','9O432501','9O432502',
'9O432503','9O432504','9O433501','9O433502','9O434501','9O434502',
'9O435501','9O435502','9O436501','9O436502','9O434503','9O432505','9O431505','9O073504')) or OSHORT = '9J412504') ";

                var dao = _oracleDBHelper.ExecuteQuery<HcmVwEmp01Dao>(sql);

                return dao;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int DeleteAccountInfo(List<int> accountSnList)
        {
            string sql = "DELETE [dbo].[account_info] WHERE sn in @Sn ; ";

            var dao = _dbHelper.ExecuteNonQuery(sql, new
            {
                Sn = accountSnList
            });

            return dao;
        }

        public int DeleteAccountPermissionByList(List<int> accountSnList)
        {
            string sql = "DELETE [dbo].[account_menu_info] WHERE account_sn IN @account_sn ; ";

            var dao = _dbHelper.ExecuteNonQuery(sql, new
            {
                account_sn = accountSnList
            });

            return dao;
        }

        public int DeleteHcm()
        {
            try
            {
                string sql = @"
DELETE [dbo].[hcm_vw_emp01];";

                var dao = _dbHelper.ExecuteNonQuery(sql);

                return dao;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int InsertHcm(List<HcmVwEmp01Dao> hcmList)
        {
            try
            {
                string sql = @"
INSERT INTO [dbo].[hcm_vw_emp01]
           ([PERNR]
           ,[STAT2TXT]
           ,[PKTXT]
           ,[CSHORT]
           ,[CSHORTID]
           ,[CSTEXT]
           ,[OSHORT]
           ,[OSTEXT]
           ,[NACHN]
           ,[VORNA]
           ,[NATIO]
           ,[COMID2]
           ,[SCHKZ])
     VALUES(@PERNR
           ,@STAT2TXT
           ,@PKTXT
           ,@CSHORT
           ,@CSHORTID
           ,@CSTEXT
           ,@OSHORT
           ,@OSTEXT
           ,@NACHN
           ,@VORNA
           ,@NATIO
           ,@COMID2
           ,@SCHKZ);";

                var dao = _dbHelper.ExecuteNonQuery(sql, hcmList);

                return dao;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<AccountInfoDao> SelectIEApplyFlow(string jobId, int levelId, int applyFloor)
        {
            string _factoryScript = "";
            int _lastestNo = 3;

            switch (applyFloor)
            {
                case 1:
                    _factoryScript = @"select 3 as 'no', sn, name, jobId, mail, deptSn
  from account_info where deptSn = 2 ";
                    break;
                case 2:
                    _factoryScript = @"select 3 as 'no', sn, name, jobId, mail, deptSn
  from account_info where deptSn = 1 ";
                    break;
                case 3:
                    _factoryScript = @"select 3 as 'no', sn, name, jobId, mail, deptSn
  from account_info where deptSn = 2
union
select 4 as 'no', sn, name, jobId, mail, deptSn
  from account_info where deptSn = 1 ";
                    _lastestNo = 4;
                    break;
                default:
                    break;
            }

            string sql = $@" 
WITH base as ( 
select eai.sn 'empSn', eai.name 'empName', eai.jobId 'empJobId', eai.mail 'empMail', eai.level_id 'empLevelId', eai.deptSn 'empDeptSn'
     , 1 as 'no', ISNULL(dai.sn, 0)'sn', ISNULL(dai.name, '')'name', ISNULL(dai.jobId, '')'jobId', ISNULL(dai.mail, '')'mail', ISNULL(dai.deptSn, '')'deptSn'
  from account_info eai
  left join account_info dai 
on eai.deptSn = dai.deptSn 
and dai.level_id < @level_id 
where eai.level_id = @level_id and eai.jobId = @JobId
),
mgnt as (
select 2 as 'no', ISNULL(ai.sn, 0)'sn', ISNULL(ai.name, '')'name', ISNULL(ai.jobId, '')'jobId', ISNULL(ai.mail, '')'mail', ISNULL(ai.deptSn, '')'deptSn'
  from base b 
join definition_department d 
on b.empDeptSn = d.deptSn 
join account_info ai 
on d.parentDeptId = ai.deptSn 
where b.empLevelId > ai.level_id  
),
factoryMgnt as ( {_factoryScript}
),
ieDept as (
select (ROW_NUMBER() OVER(ORDER BY level_id DESC)) + {_lastestNo} 'no' ,sn, name, jobId, mail, deptSn from account_info  
where jobId = '12109416' or (deptSn = 44 and level_id < 4 )
)
select no,sn,name,jobId,mail,deptSn from base 
union 
select * from mgnt 
union 
select * from factoryMgnt 
union 
select * from ieDept 
union 
select (3 + {_lastestNo}) 'no',sn, name, jobId, mail, deptSn from account_info 
where jobId = '20010946' ; ";

            var dao = _dbHelper.ExecuteQuery<AccountInfoDao>(sql, new
            {
                level_id = levelId,
                JobId = jobId
            });

            return dao;
        }

    }
}
