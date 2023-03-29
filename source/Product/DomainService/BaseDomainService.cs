namespace MOD4.Web.DomainService
{
    public class BaseDomainService
    {
        protected MailService _mailServer;

        public BaseDomainService()
        {
            _mailServer = new MailService();
        }
    }
}
