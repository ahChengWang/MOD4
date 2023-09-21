using MOD4.Web.Enum;

namespace MOD4.Web.Repostory.Dao
{
    public class DefinitionMaterialDao
    {
        public int Sn { get; set; }
        public string MatlNo { get; set; }
        public MatlCodeTypeEnum CodeTypeId { get; set; }
        public string MatlName { get; set; }
        public string MatlCatg { get; set; }
        public string UseNode { get; set; }
    }
}
