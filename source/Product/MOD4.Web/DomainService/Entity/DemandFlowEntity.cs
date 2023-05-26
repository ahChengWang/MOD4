using MOD4.Web.Repostory;
using MOD4.Web.Repostory.Dao;

namespace MOD4.Web.DomainService.Entity
{
    public class DemandFlowEntity
    {
        public DemandEntity InEntity { get; set; }
        public UserEntity UserInfo { get; set; }
        public DemandsDao OldDemandOrder { get; set; }
        public DemandsDao UpdateDemandOrder { get; set; }
        public IDemandsRepository DemandsRepository { get; set; }
        public MailService MailService { get; set; }
        public IAccountDomainService AccountDomainService { get; set; }
        public IMAppDomainService MAppDomainService { get; set; }
        public string UploadUrl { get; set; }
    }
}
