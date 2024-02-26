using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.Repostory
{
    public class DefinitionDepartmentRepository : BaseRepository, IDefinitionDepartmentRepository
    {

        public List<DefinitionDepartmentDao> SelectDeptName(List<int> deptSnList)
        {
            string sql = @"select a.deptSn, ISNULL(b.departmentName, '') + a.departmentName 'departmentName' from definition_department a 
left join definition_department b 
on a.parentDeptId = b.deptSn 
and b.levelId != 1 
where a.deptSn in @DeptSn ; ";

            var dao = _dbHelper.ExecuteQuery<DefinitionDepartmentDao>(sql, new 
            {
                DeptSn = deptSnList
            });

            return dao;
        }

        public List<DefinitionDepartmentDao> SelectDeptOptions()
        {
            string sql = @"select dept.deptSn 'parentDeptId',sec.deptSn,dept.departmentName + '/' + sec.departmentName as 'departmentName' from definition_department dept
join definition_department sec
on dept.deptSn = sec.parentDeptId
and ((dept.deptSn between 3 and 8) or dept.deptSn = 37 ) ; ";

            var dao = _dbHelper.ExecuteQuery<DefinitionDepartmentDao>(sql);

            return dao;
        }

        public List<DefinitionDepartmentDao> SelectByDeptList(List<int> deptSnList = null)
        {
            string sql = @"select dept.deptSn 'parentDeptId',sec.deptSn,dept.departmentName + '/' + sec.departmentName as 'departmentName' from definition_department dept
join definition_department sec
on dept.deptSn = sec.parentDeptId  ";

            if (deptSnList != null && deptSnList.Any())
            {
                sql += " and sec.deptSn in @DeptSn ";
            }

            var dao = _dbHelper.ExecuteQuery<DefinitionDepartmentDao>(sql, new
            {
                DeptSn = deptSnList
            });

            return dao;
        }
    }
}
