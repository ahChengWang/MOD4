namespace MOD4.Web.DomainService
{
    public class BaseDomainService
    {
        protected MailService _mailServer;
        protected FTPService _ftpService;

        public BaseDomainService()
        {
            _mailServer = new MailService();
            _ftpService = new FTPService();
        }
    }
}
