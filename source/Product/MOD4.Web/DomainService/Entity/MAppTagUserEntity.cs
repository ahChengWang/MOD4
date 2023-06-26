using System.Collections.Generic;

namespace MOD4.Web.DomainService.Entity
{
    public class MAppTagUserEntity
    {
        public string url {
            get 
            {
                return "http://mapp.local/teamplus_innolux/EIM/Messenger/MessengerMain.aspx";
            } 
            set 
            {
                value = "http://mapp.local/teamplus_innolux/EIM/Messenger/MessengerMain.aspx";
            } 
        }
        public string chatId { get; set; }
        public string account { get; set; }

        private string _psw = string.Empty;
        public string password { get; set; }
        public List<MAppTagDetailEntity> sendInfo { get; set; }
    }
}
