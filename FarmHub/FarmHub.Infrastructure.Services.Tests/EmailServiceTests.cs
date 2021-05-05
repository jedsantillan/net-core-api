using System.Threading.Tasks;
using FarmHub.Domain.Services.Email;
using Microsoft.Extensions.Options;
using Moq;
using SendGrid.Helpers.Mail;
using Xunit;

namespace FarmHub.Infrastructure.Services
{
    public class EmailServiceTests
    {
        // This is an integration test that requires SendGrid ID to work
        //[Fact] 
        public async Task SendAsync_Test()
        {
            var options = new Mock<IOptions<EmailOptions>>();
            options.SetupGet(p => p.Value)
                .Returns(new EmailOptions()
                {
                    // Add a valid key here
                    ApiKey = "",
                    FromEmail = "andrei@mayani.ph",
                    FromName = "Mayani Tech"
                });
            
            var sut = new EmailService(options.Object);
            var response = await sut.SendAsync("andrei.delacruz@gmail.com", "Andrei", new SendGridMessage()
            {
                Subject = "HELLO",
                From = new EmailAddress("andrei@mayani.ph", "Mayani Tech"),
                PlainTextContent = "HOTDOG"
            });
        }
    }
}