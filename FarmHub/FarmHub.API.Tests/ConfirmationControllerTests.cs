using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FarmHub.API.Controllers;
using FarmHub.Application.Services.Repositories;
using FarmHub.Application.Services.Services;
using FarmHub.Application.Services.Services.Interface;
using FarmHub.Data.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FarmHub.API.Tests
{
    public class ConfirmationControllerTests
    {
        private Mock<IConfirmationService> _confirmationService;
        private Mock<ICustomerService> _customerService;
        private Mock<IConfirmationEmailService<Customer>> _accountConfirmationService;

        public ConfirmationControllerTests()
        {
            _confirmationService = new Mock<IConfirmationService>();
            _customerService = new Mock<ICustomerService>();
            _accountConfirmationService = new Mock<IConfirmationEmailService<Customer>>();
        }

        [Fact]
        public async Task ConfirmAccount_GivenAGuid_ShouldCallEmailServiceAndReturn200()
        {
            _confirmationService.Setup(s => s.ValidateGuid(It.IsAny<Guid>(), It.IsAny<ConfirmationEmailType>()))
                .ReturnsAsync(ConfirmationResult.Success);
            
            var controller = new ConfirmationController(_confirmationService.Object, _customerService.Object, _accountConfirmationService.Object);
            var actionResult = await controller.ConfirmAccountAsync(Guid.NewGuid().ToString());
            
            actionResult.Should().BeOfType<OkObjectResult>().Subject.StatusCode.Should().Be(200);
            actionResult.Should().BeOfType<OkObjectResult>().Subject.Value.Should().Be(ConfirmationResult.Success);
            _confirmationService.Verify(s => s.ValidateGuid(It.IsAny<Guid>(), ConfirmationEmailType.AccountCreation), Times.Once);

        }
        
        [Fact]
        public async Task ConfirmAccount_GivenAnInvalidGuid_ShouldReturnBadRequestAndNotCallConfirmationService()
        {
            _confirmationService.Setup(s => s.ValidateGuid(It.IsAny<Guid>(), It.IsAny<ConfirmationEmailType>()))
                .ReturnsAsync(ConfirmationResult.Success);
            
            var controller = new ConfirmationController(_confirmationService.Object, _customerService.Object, _accountConfirmationService.Object);
            var actionResult = await controller.ConfirmAccountAsync("iam-a-fake-guid");
            
            actionResult.Should().BeOfType<BadRequestResult>().Subject.StatusCode.Should().Be(400);
            _confirmationService.Verify(s => s.ValidateGuid(It.IsAny<Guid>(), It.IsAny<ConfirmationEmailType>()), Times.Never);
        }
        
        
        [Fact]
        public async Task ConfirmForgetPassword_GivenAGuid_ShouldCallEmailServiceWithForgetPasswordTypeAndReturn200()
        {
            _confirmationService.Setup(s => s.ValidateGuid(It.IsAny<Guid>(), It.IsAny<ConfirmationEmailType>()))
                .ReturnsAsync(ConfirmationResult.Success);
            
            var controller = new ConfirmationController(_confirmationService.Object, _customerService.Object, _accountConfirmationService.Object);
            var actionResult = await controller.ConfirmForgetPasswordAsync(Guid.NewGuid().ToString());
            
            actionResult.Should().BeOfType<OkObjectResult>().Subject.StatusCode.Should().Be(200);
            actionResult.Should().BeOfType<OkObjectResult>().Subject.Value.Should().Be(ConfirmationResult.Success);
            _confirmationService.Verify(s => s.ValidateGuid(It.IsAny<Guid>(), ConfirmationEmailType.ForgotPassword), Times.Once);
        }

        [Fact]
        public async Task ResendConfirmationAsync_WhenGuidIsExisting_ShouldCallAccountConfirmationService()
        {
            _confirmationService
                .Setup(s => s.FirstOrDefaultAsync(It.IsAny<Expression<Func<ConfirmationEmail, bool>>>()))
                .ReturnsAsync(new ConfirmationEmail());

            _customerService.Setup(c => c.FirstOrDefaultAsync(It.IsAny<Expression<Func<Customer, bool>>>()))
                .ReturnsAsync(new Customer());

            _accountConfirmationService.Setup(ac => ac.SendConfirmationEmail(It.IsAny<Customer>()))
                .ReturnsAsync(1);
            
            var controller = new ConfirmationController(_confirmationService.Object, _customerService.Object, _accountConfirmationService.Object);
            var result = await controller.ResendConfirmationEmail(Guid.NewGuid().ToString());

            result.Should().NotBe(0);
            _accountConfirmationService.Verify(ac => ac.SendConfirmationEmail(It.IsAny<Customer>()), Times.Once);
        }
        
        [Fact]
        public async Task ResendConfirmationAsync_WhenGuidIsNonExistent_ShouldNOTCallAccountConfirmationService()
        {
            _confirmationService
                .Setup(s => s.FirstOrDefaultAsync(It.IsAny<Expression<Func<ConfirmationEmail, bool>>>()))
                .ReturnsAsync((ConfirmationEmail) null);

            _customerService.Setup(c => c.FirstOrDefaultAsync(It.IsAny<Expression<Func<Customer, bool>>>()))
                .ReturnsAsync(new Customer());

            _accountConfirmationService.Setup(ac => ac.SendConfirmationEmail(It.IsAny<Customer>()))
                .ReturnsAsync(1);
            
            var controller = new ConfirmationController(_confirmationService.Object, _customerService.Object, _accountConfirmationService.Object);
            var result = await controller.ResendConfirmationEmail(Guid.NewGuid().ToString());

            result.Should().NotBe(0);
            _accountConfirmationService.Verify(ac => ac.SendConfirmationEmail(It.IsAny<Customer>()), Times.Never);
        }
    }
}