using System.Collections.Generic;
using FarmHub.Application.Services.Email;
using FarmHub.Application.Services.Email.Templates;
using FarmHub.Domain.Services.Email;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using SendGrid.Helpers.Mail;
using Xunit;

namespace FarmHub.Infrastructure.Services
{
    public class SendGridMessageFactoryTests
    {
        private Mock<IOptions<EmailOptions>> _emailOptions;
        private Mock<IOptions<SendGridOptions>> _sendGridOptions;

        public SendGridMessageFactoryTests()
        {
            _emailOptions = new Mock<IOptions<EmailOptions>>();
            _emailOptions.SetupGet(p => p.Value)
                .Returns(new EmailOptions()
                {
                    // Add a valid key here
                    ApiKey = "",
                    FromEmail = "andrei@mayani.ph",
                    FromName = "Mayani Tech"
                });


            _sendGridOptions = new Mock<IOptions<SendGridOptions>>();
            _sendGridOptions.SetupGet(p => p.Value)
                .Returns(new SendGridOptions()
                {
                    // Add a valid key here
                    Templates = new Dictionary<string, string>()
                    {
                        {"AccountConfirmation", "12312312"}
                    }
                });
        }

        [Fact]
        public void Create_WhenAccountConfirmationTemplateIsPassed_ShouldCreateSendGridMessageWithPersonalizationWithTemplate()
        {
            const string name = "NameTest";
            const string url = "UrlTest";
            var template = new AccountConfirmationTemplate()
            {
                TemplateData = new AccountConfirmationEmail()
                {
                    Name = name,
                    Url = url
                }
            };

            var sut = new SendGridMessageFactory(_emailOptions.Object, _sendGridOptions.Object);
            var sgm = sut.Create(template);

            sgm.Should().NotBeNull()
                .And.BeOfType<SendGridMessage>();

            sgm.Personalizations.Should().NotBeEmpty();
            sgm.Personalizations[0].TemplateData
                .Should().BeOfType<AccountConfirmationEmail>()
                .Which.Should().BeEquivalentTo(new AccountConfirmationEmail()
                {
                    Name = name,
                    Url = url,
                });
            sgm.From.Email.Should().Be(_emailOptions.Object.Value.FromEmail);
            sgm.From.Name.Should().Be(_emailOptions.Object.Value.FromName);
        }
    }
}