using System;
using System.Collections.Generic;
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
    public class OrderConfirmationServiceTests : DbContextTestBase
    {
        private Mock<IEmailService> _emailServiceMock;
        private Mock<IEmailMessageFactory> _emailTemplateFactoryMock;
        private Mock<ILogger<OrderConfirmationService>> _iloggerMock;
        private Mock<IOptions<UrlOptions>> _optionsMock;

        public OrderConfirmationServiceTests()
        {
            _emailServiceMock = new Mock<IEmailService>();
            _emailTemplateFactoryMock = new Mock<IEmailMessageFactory>();
            _iloggerMock = new Mock<ILogger<OrderConfirmationService>>();
            _optionsMock = new Mock<IOptions<UrlOptions>>();
        }

        [Fact]
        public async Task SendAccountConfirmation_WhenDataIsValid_ShouldCreateEmailAndCallEmailService()
        {
            _emailTemplateFactoryMock.Setup(f => f.Create(It.IsAny<IEmailTemplate<AccountConfirmationTemplate>>()))
                .Returns(new SendGridMessage());

            _emailServiceMock.Setup(s =>
                    s.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SendGridMessage>()))
                .ReturnsAsync(HttpStatusCode.Accepted);

            _optionsMock.SetupGet(p => p.Value)
                .Returns(new UrlOptions()
                {
                    OrderUrl = @"http:\testurl\confirmation"
                });

            var emailAddress = "hot@dog.ph";

            var order = new Order
            {
                Customer = new Customer()
                {
                    ContactEmail = emailAddress,
                    FirstName = "fname"
                },
                OrderItems = new List<OrderItem>()
                {
                    new OrderItem()
                    {
                        Quantity = 2,
                        CreatedDate = DateTime.Now,
                        ProductPortion = new ProductPortion()
                        {
                            Portion = new Portion("1/4", 0.25),
                            Product = new Product("Ampalayainka", "AK")
                            {
                                UnitOfMeasure = new UnitOfMeasure("kg")
                            }
                        }
                    }
                }
            };
            
            var service = new OrderConfirmationService(_emailServiceMock.Object, _emailTemplateFactoryMock.Object, _optionsMock.Object, _iloggerMock.Object);
            var result = await service.SendConfirmationEmail(order);
            result.Should().NotBe(0);

            _emailTemplateFactoryMock.Verify(f => f.Create(It.IsAny<OrderConfirmationTemplate>()), Times.Once);
            _emailServiceMock.Verify(s => s.SendAsync(It.Is<string>(
                email => email == order.Customer.ContactEmail), It.Is<string>(
                name => name == order.Customer.FirstName), It.IsAny<SendGridMessage>()));
        }

    }
}