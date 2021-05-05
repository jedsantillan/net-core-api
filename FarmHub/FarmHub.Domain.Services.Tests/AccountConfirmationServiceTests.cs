using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using FarmHub.Application.Services.Email.Templates;
using FarmHub.Application.Services.Infrastructure;
using FarmHub.Application.Services.Repositories;
using FarmHub.Application.Services.Services;
using FarmHub.Data.Models;
using FarmHub.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SendGrid.Helpers.Mail;
using Xunit;

namespace FarmHub.Application.Services.Tests
{
    public class AccountConfirmationServiceTests : DbContextTestBase
    {
        private Mock<IEmailService> _emailServiceMock;
        private Mock<IEmailMessageFactory> _emailTemplateFactoryMock;
        private Mock<IConfirmationService> _confirmationServiceMock;
        private Mock<ILogger<AccountConfirmationService>> _iloggerMock;
        private Mock<IOptions<UrlOptions>> _optionsMock;

        public AccountConfirmationServiceTests()
        {
            _emailServiceMock = new Mock<IEmailService>();
            _emailTemplateFactoryMock = new Mock<IEmailMessageFactory>();
            _iloggerMock = new Mock<ILogger<AccountConfirmationService>>();
            _confirmationServiceMock = new Mock<IConfirmationService>();
            _optionsMock = new Mock<IOptions<UrlOptions>>();
        }

        [Fact]
        public async Task SendAccountConfirmation_WhenDataIsValid_ShouldCreateEmailAndCallEmailService()
        {
            _emailTemplateFactoryMock.Setup(f => f.Create(It.IsAny<IEmailTemplate<AccountConfirmationTemplate>>()))
                .Returns(new SendGridMessage());

            _emailServiceMock.Setup(s =>
                    s.SendAsync(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<SendGridMessage>()))
                .ReturnsAsync(HttpStatusCode.Accepted);

            _confirmationServiceMock.Setup(c => c.FilterAsync(It.IsAny<Expression<Func<ConfirmationEmail, bool>>>()))
                .ReturnsAsync(new List<ConfirmationEmail>());

            _confirmationServiceMock.Setup(c => c.InsertAsync(It.IsAny<ConfirmationEmail>()))
                .Callback<ConfirmationEmail>(c => c.Id = 1);
            
            _optionsMock.SetupGet(p => p.Value)
                .Returns(new UrlOptions()
                {
                    ConfirmationUrl = @"http:\testurl\confirmation"
                });
            
            var service = new AccountConfirmationService(_emailServiceMock.Object, _emailTemplateFactoryMock.Object, _confirmationServiceMock.Object, _optionsMock.Object, _iloggerMock.Object);

            var customer = new Customer();
            var result = await service.SendConfirmationEmail(customer);
            result.Should().NotBe(0);

            _emailTemplateFactoryMock.Verify(f => f.Create(It.IsAny<AccountConfirmationTemplate>()), Times.Once);
            // TODO Andrei: Add checking of template contents
            _emailServiceMock.Verify(s => s.SendAsync(It.Is<string>(
                email => email == customer.ContactEmail), It.Is<string>(
                name => name == customer.FirstName), It.IsAny<SendGridMessage>()));
        }
    }
}