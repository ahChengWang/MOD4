namespace MOD4.Web.DomainService.Entity
{
    public class DepartmentEntity
    {
        public int DeptSn { get; set; }
        public int ParentDeptId { get; set; }
        public string DepartmentName { get; set; }
        public int LevelId { get; set; }
        public bool IdDel { get; set; }
    }
}
