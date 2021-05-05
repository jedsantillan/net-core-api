using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Adyen.Model.Checkout;
using Adyen.Service;
using AutoMapper;
using FarmHub.API.Controllers;
using FarmHub.API.Models;
using FarmHub.Application.Services.Infrastructure;
using FarmHub.Application.Services.Repositories;
using FarmHub.Data.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FarmHub.API.Tests
{
    public class AdyenControllerTests
    {
        [Fact]
        public async Task GetPaymentMethods_Success_ShouldReturnCode200()
        {
            var mockAdyenService = new Mock<IAdyenPaymentService>();
            mockAdyenService.Setup(s =>
                    s.PaymentMethodsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>(),
                        PaymentMethodsRequest.ChannelEnum.Web))
                .ReturnsAsync(new PaymentMethodsResponse());

            var controller = new AdyenController(mockAdyenService.Object, Mock.Of<IOrderService>(), Mock.Of<IMapper>(),
                Mock.Of<ILogger<AdyenController>>());

            var result = await controller.GetPaymentMethods(Mock.Of<AdyenGetPaymentMethodsRequest>());
            result.Should().BeOfType<OkObjectResult>().Subject.Value.Should().NotBeNull().And.Subject.Should()
                .BeOfType<PaymentMethodsResponse>().Should().NotBeNull();
        }

        [Fact]
        public void GetPaymentMethods_Failure_ShouldThrowException()
        {
            var mockAdyenService = new Mock<IAdyenPaymentService>();
            mockAdyenService.Setup(s =>
                    s.PaymentMethodsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>(),
                        PaymentMethodsRequest.ChannelEnum.Web))
                .Throws(new Exception());

            var controller = new AdyenController(mockAdyenService.Object, Mock.Of<IOrderService>(), Mock.Of<IMapper>(),
                Mock.Of<ILogger<AdyenController>>());
            var exception = Record
                .ExceptionAsync(() => controller.GetPaymentMethods(Mock.Of<AdyenGetPaymentMethodsRequest>())).Result;

            exception.Should().BeOfType<Exception>();
        }

        [Fact]
        public async Task SubmitOneTimePayment_Success_ShouldReturnCode200()
        {
            var mockAdyenService = new Mock<IAdyenPaymentService>();

            mockAdyenService.Setup(s =>
                    s.PaymentsAsync(It.IsAny<string>(), 
                        It.IsAny<long>(), 
                        It.IsAny<int>(), 
                        It.IsAny<string>(), 
                        It.IsAny<MayaniPaymentMethod>(),
                        It.IsAny<DefaultPaymentMethodDetails>(),
                        It.IsAny<BrowserInfo>(),
                        It.IsAny<bool>()))
                .ReturnsAsync(new PaymentResponse()
                {
                    ResultCode = PaymentResponse.ResultCodeEnum.Authorised,
                    PaymentData = It.IsAny<string>(),
                    PspReference = It.IsAny<string>(),
                    RefusalReason = It.IsAny<string>()
                });

            var mockOrderService = new Mock<IOrderService>();
            
            var controller = new AdyenController(mockAdyenService.Object, mockOrderService.Object, Mock.Of<IMapper>(),
                Mock.Of<ILogger<AdyenController>>());
            
            var result = await controller.SubmitOneTimePayment(Mock.Of<AdyenSubmitPaymentOneTimeRequest>());
            
            mockOrderService.Verify(os => os.UpdatePaymentStatusAsync(It.IsAny<int>(), PaymentStatus.Success, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            result.Should().BeOfType<OkObjectResult>().Subject.Value.Should().NotBeNull().And.Subject.Should()
                .BeOfType<PaymentResponse>().Should().NotBeNull();
        }
        [Fact]
        
        public void SubmitOneTimePayment_Failure_ShouldReturnException()
        {
            var mockAdyenService = new Mock<IAdyenPaymentService>();
            mockAdyenService.Setup(s =>
                    s.PaymentsAsync(It.IsAny<string>(), 
                        It.IsAny<long>(), 
                        It.IsAny<int>(), 
                        It.IsAny<string>(), 
                        It.IsAny<MayaniPaymentMethod>(),
                        It.IsAny<DefaultPaymentMethodDetails>(),
                        It.IsAny<BrowserInfo>(),
                        It.IsAny<bool>()))
                .Throws(new Exception());

            var mockOrderService = new Mock<IOrderService>();
            
            var controller = new AdyenController(mockAdyenService.Object, mockOrderService.Object, Mock.Of<IMapper>(),
                Mock.Of<ILogger<AdyenController>>());
            
            var exception = Record
                .ExceptionAsync(() => controller.SubmitOneTimePayment(Mock.Of<AdyenSubmitPaymentOneTimeRequest>())).Result;

            exception.Should().BeOfType<Exception>();
        }

        [Fact]
        public async Task SubmitAdditionalDetails_SuccessWithAuthorisedPayment_ShouldReturn200()
        {
            var mockAdyenService = new Mock<IAdyenPaymentService>();
            mockAdyenService.Setup(s =>
                    s.PaymentDetailsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new PaymentResponse
                {
                    ResultCode = PaymentResponse.ResultCodeEnum.Authorised,
                    PaymentData = It.IsAny<string>(),
                    PspReference = It.IsAny<string>(),
                    RefusalReason = It.IsAny<string>()
                });
            
            
            var mockedOrder = new Mock<Order>();
            mockedOrder.SetupGet(o => o.CardPayments).Returns(new List<CardPayment>(){new CardPayment()
            {
                Id = 1,
                PaymentData = "FakePaymentData",
                CreatedDate = DateTime.Now
            }});
            
            var mockOrderService = new Mock<IOrderService>();
            mockOrderService.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(mockedOrder.Object);
            
            var controller = new AdyenController(mockAdyenService.Object, mockOrderService.Object, Mock.Of<IMapper>(),
                Mock.Of<ILogger<AdyenController>>());
            
            var result = await controller.SubmitAdditionalDetails(Mock.Of<AdyenSubmitAdditionalDetailsRequest>());
            mockOrderService.Verify(os => os.UpdatePaymentStatusAsync(It.IsAny<int>(), PaymentStatus.Success, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            result.Should().BeOfType<OkObjectResult>().Subject.Value.Should().NotBeNull().And.Subject.Should()
                .BeOfType<PaymentResponse>().Should().NotBeNull();
        }
        
        [Fact]
        public async Task SubmitAdditionalDetails_SuccessWithRefusedStatus_ShouldReturn200()
        {
            var mockAdyenService = new Mock<IAdyenPaymentService>();
            mockAdyenService.Setup(s =>
                    s.PaymentDetailsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new PaymentResponse
                {
                    ResultCode = PaymentResponse.ResultCodeEnum.Refused,
                    PaymentData = It.IsAny<string>(),
                    PspReference = It.IsAny<string>(),
                    RefusalReason = It.IsAny<string>()
                });
            
            
            var mockedOrder = new Mock<Order>();
            mockedOrder.SetupGet(o => o.CardPayments).Returns(new List<CardPayment>(){new CardPayment()
            {
                Id = 1,
                PaymentData = "FakePaymentData",
                CreatedDate = DateTime.Now
            }});
            
            var mockOrderService = new Mock<IOrderService>();
            mockOrderService.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(mockedOrder.Object);
            
            var controller = new AdyenController(mockAdyenService.Object, mockOrderService.Object, Mock.Of<IMapper>(),
                Mock.Of<ILogger<AdyenController>>());
            
            var result = await controller.SubmitAdditionalDetails(Mock.Of<AdyenSubmitAdditionalDetailsRequest>());
            mockOrderService.Verify(os => os.UpdatePaymentStatusAsync(It.IsAny<int>(), PaymentStatus.Failed, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            result.Should().BeOfType<OkObjectResult>().Subject.Value.Should().NotBeNull().And.Subject.Should()
                .BeOfType<PaymentResponse>().Should().NotBeNull();
        }
        
        [Fact]
        public async Task SubmitAdditionalDetails_PaymentDataIsNullOrEmpty_ShouldReturn400()
        {
            var mockAdyenService = new Mock<IAdyenPaymentService>();
            mockAdyenService.Setup(s =>
                    s.PaymentDetailsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new PaymentResponse
                {
                    ResultCode = PaymentResponse.ResultCodeEnum.Authorised,
                    PaymentData = It.IsAny<string>(),
                    PspReference = It.IsAny<string>(),
                    RefusalReason = It.IsAny<string>()
                });
            
            
            var mockedOrder = new Mock<Order>();
            mockedOrder.SetupGet(o => o.CardPayments).Returns(new List<CardPayment>(){new CardPayment()
            {
                Id = 1,
                PaymentData = null,
                CreatedDate = DateTime.Now
            }});
            
            var mockOrderService = new Mock<IOrderService>();
            mockOrderService.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(mockedOrder.Object);
            
            var controller = new AdyenController(mockAdyenService.Object, mockOrderService.Object, Mock.Of<IMapper>(),
                Mock.Of<ILogger<AdyenController>>());
            
            var result = await controller.SubmitAdditionalDetails(Mock.Of<AdyenSubmitAdditionalDetailsRequest>());
            result.Should().BeOfType<BadRequestResult>();
        }
        
        [Fact]
        public async Task SubmitAdditionalDetails_OrderDoesNotExist_ShouldReturn400()
        {
            var mockAdyenService = new Mock<IAdyenPaymentService>();
            mockAdyenService.Setup(s =>
                    s.PaymentDetailsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new PaymentResponse
                {
                    ResultCode = PaymentResponse.ResultCodeEnum.Authorised,
                    PaymentData = It.IsAny<string>(),
                    PspReference = It.IsAny<string>(),
                    RefusalReason = It.IsAny<string>()
                });

            var mockOrderService = new Mock<IOrderService>();
            mockOrderService.Setup(s => s.GetByIdAsync(It.IsAny<int>())).Returns(Task.FromResult<Order>(null));
            
            var controller = new AdyenController(mockAdyenService.Object, mockOrderService.Object, Mock.Of<IMapper>(),
                Mock.Of<ILogger<AdyenController>>());
            
            var result = await controller.SubmitAdditionalDetails(Mock.Of<AdyenSubmitAdditionalDetailsRequest>());
            result.Should().BeOfType<BadRequestResult>();
        }
        
        [Fact]
        public void SubmitAdditionalDetails_ErrorInPaymentService_ShouldReturnException()
        {
            var mockAdyenService = new Mock<IAdyenPaymentService>();
            mockAdyenService.Setup(s =>
                    s.PaymentDetailsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception());
            
            
            var mockOrderService = new Mock<IOrderService>();
            var mockedOrder = new Mock<Order>();
            mockedOrder.SetupGet(o => o.CardPayments).Returns(new List<CardPayment>(){new CardPayment()
            {
                Id = 1,
                PaymentData = "FakePaymentData",
                CreatedDate = DateTime.Now
            }});
            
            mockOrderService.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(mockedOrder.Object);
            
            var controller = new AdyenController(mockAdyenService.Object, mockOrderService.Object, Mock.Of<IMapper>(),
                Mock.Of<ILogger<AdyenController>>());
            
            var exception = Record
                .ExceptionAsync(() =>  controller.SubmitAdditionalDetails(Mock.Of<AdyenSubmitAdditionalDetailsRequest>())).Result;
           
            exception.Should().BeOfType<Exception>();
        }
    }
}