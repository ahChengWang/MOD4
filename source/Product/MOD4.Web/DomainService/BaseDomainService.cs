using Utility.Helper;

namespace MOD4.Web.DomainService
{
    public class BaseDomainService
    {
        protected MailService _mailServer;
        protected FTPService _ftpService;
        protected LogHelper _logHelper;
        protected INXReportService _inxReportService;

        public BaseDomainService()
        {
            _mailServer = new MailService();
            _ftpService = new FTPService();
            _logHelper = new LogHelper();
            _inxReportService = new INXReportService();
        }
    }
}
