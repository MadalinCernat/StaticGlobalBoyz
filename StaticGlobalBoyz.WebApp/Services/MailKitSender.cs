using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using MailKit.Net.Smtp;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.WebApp.Services
{
    public class MailKitSender
    {
        private readonly MailKitOptions _options;

        public MailKitSender(MailKitOptions options)
        {
            _options = options;
        }
        public async Task SendEmailAsync(string to, string subject, bool isBodyHtml, MimeEntity messageBody)
        {
            var _from = new MailboxAddress("StaticGlobalBoyz", "staticglobalboyz@gmail.com");
            var _to = new MailboxAddress("Client", to);
                await Execute(_from, _to, subject, isBodyHtml, messageBody);

        }
        private async Task Execute(InternetAddress from, InternetAddress to, string subject, bool isBodyHtml,  MimeEntity messageBody)
        {
            var client = new SmtpClient();
            await client.ConnectAsync(_options.Host, _options.Port, true);
            await client.AuthenticateAsync(new NetworkCredential(_options.Username, _options.Password));
            
            MimeMessage message = new MimeMessage();
            message.From.Add(from);
            message.To.Add(to);
            message.Subject = subject;

            message.Body = messageBody;

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        
    }
}
