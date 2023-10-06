using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public class DefinitionDepartmentRepository : BaseRepository, IDefinitionDepartmentRepository
    {

        public List<DefinitionDepartmentDao> SelectByConditions()
        {
            string sql = @"select dept.deptSn 'parentDeptId',sec.deptSn,dept.departmentName + '/' + sec.departmentName as 'departmentName' from definition_department dept
join definition_department sec
on dept.deptSn = sec.parentDeptId
and dept.deptSn  between 3 and 8 ; ";

            var dao = _dbHelper.ExecuteQuery<DefinitionDepartmentDao>(sql);

            return dao;
        }
    }
}
