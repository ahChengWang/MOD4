using MOD4.Web.Enum;

namespace MOD4.Web.ViewModel
{
    public class MenuViewModel
    {
        public int sn { get; set; }
        public string Account { get; set; }
        public string Name { get; set; }
        public string Main { get; set; }
        public MenuEnum MainId { get; set; }
        public bool HaveSub { get; set; }
        public string Sub { get; set; }
        public int SubId { get; set; }
    }
}
