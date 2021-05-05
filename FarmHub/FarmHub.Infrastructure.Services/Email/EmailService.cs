using System.Net;
using System.Threading.Tasks;
using FarmHub.Application.Services.Infrastructure;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace FarmHub.Domain.Services.Email
{
    public class EmailService : IEmailService
    {
        private EmailOptions _emailConfig;

        public EmailService(IOptions<EmailOptions> emailConfig)
        {
            _emailConfig = emailConfig.Value;
        }
        public async Task<HttpStatusCode> SendAsync(string email, string name, SendGridMessage message)
        {
            var client = new SendGridClient(_emailConfig.ApiKey);
            message.AddTo(new EmailAddress(email, name));
            var response = await client.SendEmailAsync(message);
            return response.StatusCode;
        }
    }

    public class EmailOptions
    {
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public string ApiKey { get; set; }
    }
}