using System.Collections.Generic;
using System.Linq;
using FarmHub.Application.Services.Email;
using FarmHub.Application.Services.Infrastructure;
using Microsoft.Extensions.Options;
using SendGrid.Helpers.Mail;

namespace FarmHub.Domain.Services.Email
{
    
    public class SendGridMessageFactory : IEmailMessageFactory
    {
        private readonly SendGridOptions _sendGridConfig;
        private readonly EmailOptions _emailConfig;

        public SendGridMessageFactory(IOptions<EmailOptions> emailConfig, IOptions<SendGridOptions> sendGridConfig)
        {
            _sendGridConfig = sendGridConfig.Value;
            _emailConfig = emailConfig.Value;
        }

        public SendGridMessage Create<T>(IEmailTemplate<T> emailTemplate)
        {
           var sgm =  new SendGridMessage
            {
                From = new EmailAddress(_emailConfig.FromEmail, _emailConfig.FromName),
                TemplateId = _sendGridConfig.Templates[emailTemplate.TemplateName],
            };
           
           sgm.SetTemplateData(emailTemplate.TemplateData);
           return sgm;
        }
    }
}