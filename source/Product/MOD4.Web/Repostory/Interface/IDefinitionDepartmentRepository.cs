using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface IDefinitionDepartmentRepository
    {
        List<DefinitionDepartmentDao> SelectDeptName(List<int> deptSnList);

        List<DefinitionDepartmentDao> SelectDeptOptions();

        List<DefinitionDepartmentDao> SelectByDeptList(List<int> deptSnList = null);
    }
}