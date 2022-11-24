namespace MOD4.Web.Repostory.Dao
{
    public class DefinitionDepartmentDao
    {
        public int DeptSn { get; set; }
        public int ParentDeptId { get; set; }
        public string DepartmentName { get; set; }
        public int LevelId { get; set; }
        public int IsDel { get; set; }
    }
}
