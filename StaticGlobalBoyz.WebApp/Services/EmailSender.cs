using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.WebApp.Services
{
    public class EmailSender : IEmailSender
    {
        public EmailSender(AuthMessageSenderOptions options)
        {
            this._options = options;
        }

        private AuthMessageSenderOptions _options; 


        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(_options.SendGridKey, subject, message, email);
        }
        public Task SendEmailAsync(string email, string subject, string message, string templateId)
        {
            return Execute(_options.SendGridKey, subject, message, email, templateId);
        }
        private Task Execute(string apiKey, string subject, string message, string email, string templateId = null)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("staticglobalboyz@gmail.com", _options.SendGridUser),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message,
            };
            msg.AddTo(new EmailAddress(email));

            if(templateId != null)
            {
                msg.SetTemplateId(templateId);
            }

            msg.SetClickTracking(false, false);

            return client.SendEmailAsync(msg);
        }
    }
}
