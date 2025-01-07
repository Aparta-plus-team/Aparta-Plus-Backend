using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace data_aparta_.Repos.Utils
{
    public  class EmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _fromEmail;
        private readonly string _password;

        public EmailService(IConfiguration configuration)
        {
            _smtpServer = configuration["Email:SmtpServer"];
            _smtpPort = int.Parse(configuration["Email:SmtpPort"]);
            _fromEmail = configuration["Email:FromEmail"];
            _password = configuration["Email:Password"];
        }

        public async Task SendHtmlEmailAsync(string to, string subject, string htmlContent)
        {
            using var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail),
                Subject = subject,
                Body = htmlContent,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(to);

            using var smtpClient = new SmtpClient(_smtpServer, _smtpPort)
            {
                EnableSsl = true,
                Credentials = new System.Net.NetworkCredential(_fromEmail, _password)
            };

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
