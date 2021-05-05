using System.Net;
using System.Threading.Tasks;
using SendGrid.Helpers.Mail;

namespace FarmHub.Application.Services.Infrastructure
{
    public interface IEmailService
    {
        Task<HttpStatusCode> SendAsync(string email, string name, SendGridMessage message);
    }
}