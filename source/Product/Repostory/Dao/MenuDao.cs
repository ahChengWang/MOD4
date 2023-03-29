using MOD4.Web.Enum;

namespace MOD4.Web.Repostory.Dao
{
    public class MenuDao
    {
        public int sn { get; set; }
        public string account { get; set; }
        public string name { get; set; }
        public int menu_sn { get; set; }
        public int parent_menu_sn { get; set; }
        public string class_name { get; set; }
        public string page_name { get; set; }
        public string href { get; set; }
    }
}
