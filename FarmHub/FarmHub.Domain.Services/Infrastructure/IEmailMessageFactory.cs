using System.Collections.Generic;
using System.Collections.ObjectModel;
using SendGrid.Helpers.Mail;

namespace FarmHub.Application.Services.Infrastructure
{
    public interface IEmailMessageFactory
    {
        SendGridMessage Create<T>(IEmailTemplate<T> emailTemplate);
    }

    public interface IEmailTemplate<out T>
    {
        public string TemplateName { get; }
        public T TemplateData { get; }
        
    }
    
}