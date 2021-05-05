using System;
using System.Collections.Generic;
using AutoMapper;
using FarmHub.API.Controllers;
using FarmHub.API.Models;
using FarmHub.Application.Services.Repositories;
using FarmHub.Application.Services.Services;
using FarmHub.Application.Services.Services.Interface;
using FarmHub.Data.Models;
using FarmHub.Domain.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FarmHub.API.Tests
{
    public class RegistrationControllerTests
    {
        private readonly Mock<IUserRegistrationService> _userRegistrationService;
        private readonly Mock<IGoogleRecaptchaService> _recaptchaService;
        private readonly Mock<IConfirmationEmailService<Customer>> _accountConfirmationService;

        public RegistrationControllerTests()
        {
            _userRegistrationService = new Mock<IUserRegistrationService>();
            _recaptchaService = new Mock<IGoogleRecaptchaService>();
            _accountConfirmationService = new Mock<IConfirmationEmailService<Customer>>();
        }

        [Fact]
        public async void PostCreateUser_WhenRequestIsNull_ShouldReturnBadRequest()
        {
            _recaptchaService.Setup(s => s.ValidateRecaptchaToken(It.IsAny<string>()))
                .ReturnsAsync(new ReCaptchaResponseModel()
                {
                    Success = true
                });

            var controller = new RegistrationController(_userRegistrationService.Object, _recaptchaService.Object,
                _accountConfirmationService.Object,
                Mock.Of<IMapper>(),
                Mock.Of<ILogger<RegistrationController>>());

            _userRegistrationService.VerifyAll();
            var result = await controller.Post(null);
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async void
            PostCreateUser_WhenAllTheValuesFromTheaRequestsAreValid_ShouldCallUserRegistrationServiceReturn200()
        {
            var myProfile = new AutoMapperProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var autoMapper = new Mapper(configuration);

            _recaptchaService.Setup(s => s.ValidateRecaptchaToken(It.IsAny<string>()))
                .ReturnsAsync(new ReCaptchaResponseModel()
                {
                    Success = true
                });
            
            var controller = new RegistrationController(_userRegistrationService.Object, _recaptchaService.Object,
                _accountConfirmationService.Object,
                autoMapper,
                new Mock<ILogger<RegistrationController>>().Object);
            
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            
            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper
                .Setup(m => m.Link("ConfirmAccount", It.IsAny<string>()))
                .Returns("expectedUrl")
                .Verifiable();

            controller.Url = mockUrlHelper.Object;

            var registrationRequest = new Mock<UserRegistrationRequestCreateModel>();
            registrationRequest.SetupAllProperties();

            _userRegistrationService.Setup(u => u.CreateCustomerAndUserAsync(registrationRequest.Object.Email,
                    registrationRequest.Object.Email, registrationRequest.Object.Password, It.IsAny<Customer>()))
                .ReturnsAsync(new AddOrUpdateUserResult(Mock.Of<AuthUser>(), new List<IdentityError>()));

            _accountConfirmationService
                .Setup(s => s.SendConfirmationEmail(It.IsAny<Customer>()))
                .ReturnsAsync(1);
            
            var result = await controller.Post(registrationRequest.Object);
            result.Should().BeOfType<CreatedAtActionResult>().Subject.StatusCode.Should().Be(201);
            
            _userRegistrationService.VerifyAll();
            _accountConfirmationService.Verify(s => s.SendConfirmationEmail(It.IsAny<Customer>()), Times.Once);
        }

        [Fact]
        public async void PostCreateUser_WhenRecaptchaIsInvalidReturnUnauthorized()
        {
            var myProfile = new AutoMapperProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var autoMapper = new Mapper(configuration);

            _recaptchaService.Setup(s => s.ValidateRecaptchaToken(It.IsAny<string>()))
                .ReturnsAsync(new ReCaptchaResponseModel()
                {
                    Success = false
                });

            var controller = new RegistrationController(_userRegistrationService.Object, _recaptchaService.Object,
                _accountConfirmationService.Object,
                autoMapper,
                new Mock<ILogger<RegistrationController>>().Object);
            
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var registrationRequest = new Mock<UserRegistrationRequestCreateModel>();
            registrationRequest.SetupAllProperties();

            var result = await controller.Post(registrationRequest.Object);
            result.Should().BeOfType<UnauthorizedResult>();
        }
    }
}