using Microsoft.Extensions.Configuration;
using MOD4.Web.DomainService.Entity;
using System;
using System.Net.Mail;
using System.Security.Policy;

namespace MOD4.Web.DomainService
{
    public class MailService
    {
        private static string _host = string.Empty;
        private static int _port = 0;
        private static string _mailAddress = string.Empty;

        public MailService(IConfiguration config)
        {
            _host = config.GetSection("SMTPServer").GetValue<string>("ServerIP");
            _port = Convert.ToInt16(config.GetSection("SMTPServer").GetValue<string>("Port"));
            _mailAddress = config.GetSection("SMTPServer").GetValue<string>("MailAddress");
        }

        public MailService()
        {
        }

        public void Send(MailEntity mailEntity)
        {
            try
            {
                using (SmtpClient smtpClient = new SmtpClient())
                {
                    smtpClient.Host = _host;
                    smtpClient.Port = _port;

                    //create sender address
                    MailAddress from = new MailAddress(_mailAddress, "CarUX Info Center");

                    //create receiver address
                    MailAddress receiver = new MailAddress(mailEntity.To);

                    MailMessage Mymessage = new MailMessage(from, receiver);

                    Mymessage.Subject = mailEntity.Subject;
                    Mymessage.IsBodyHtml = true;
                    Mymessage.Body = mailEntity.Content;

                    //sends the email
                    smtpClient.Send(Mymessage);
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
