using System;
using AutoMapper;
using FarmHub.API.Controllers;
using FarmHub.API.Models;
using FarmHub.Data.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FarmHub.Application.Services.Repositories;
using FarmHub.Application.Services.Services.Interface;
using Microsoft.Extensions.Logging;
using Xunit;

namespace FarmHub.API.Tests
{
    public class OrderControllerTest
    {
        private Mock<IOrderService> _mockedService;
        private Mock<IConfirmationEmailService<Order>> _orderConfirmationService;
        private IMapper _mapper;

        public OrderControllerTest()
        {
            _mockedService = new Mock<IOrderService>();
            _orderConfirmationService = new Mock<IConfirmationEmailService<Order>>();
            var configuration = new MapperConfiguration(cfg => { cfg.AddProfile(new AutoMapperProfile()); });

            _mapper = configuration.CreateMapper();
        }
        [Fact]
        public async Task GetAll_HappyPath_ShouldReturnSuccess()
        {
            var orders = new List<Order>
            {
                new Order() {TotalPrice = 100, DiscountedPrice = 50},
                new Order() {TotalPrice = 200, DiscountedPrice = 100},
            };

            _mockedService.Setup(s => s.GetAll()).Returns(orders);
            var controller = new OrderController(_mockedService.Object, _orderConfirmationService.Object, _mapper, Mock.Of<ILogger<OrderController>>());

            var result = await controller.Get();

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetById_HappyPath_ShouldReturn200()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order() {Id = 1, TotalPrice = 100, DiscountedPrice = 50},
                new Order() {Id = 2, TotalPrice = 200, DiscountedPrice = 100},
            };

            var ordersGetRequest = new List<OrderViewResponse>
            {
                new OrderViewResponse()
                {
                    Id = 1,
                    TotalPrice = 100,
                    DiscountedPrice = 50
                },
                new OrderViewResponse()
                {
                    Id = 2,
                    TotalPrice = 200,
                    DiscountedPrice = 100
                },
            };

            _mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(orders.FirstOrDefault(h => h.Id == id)));

            // auto_mapper configuration
            var mockMapper = new MapperConfiguration(cfg => { cfg.AddProfile(new AutoMapperProfile()); });

            var _mapper = mockMapper.CreateMapper();

            var controller = new OrderController(_mockedService.Object, _orderConfirmationService.Object, _mapper, Mock.Of<ILogger<OrderController>>());

            // Act
            var result = await controller.GetById(1);
            var actionResult = result.Result;

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>().Subject.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetById_OrderNonExistent_ShouldReturn404()
        {
            var orders = new List<Order>
            {
                new Order() {TotalPrice = 100, DiscountedPrice = 50},
                new Order() {TotalPrice = 200, DiscountedPrice = 100},
            };

            _mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(orders.FirstOrDefault(h => h.Id == id)));

            var mockMapper = new MapperConfiguration(cfg => { cfg.AddProfile(new AutoMapperProfile()); });

            var _mapper = mockMapper.CreateMapper();

            var controller = new OrderController(_mockedService.Object, _orderConfirmationService.Object, _mapper, Mock.Of<ILogger<OrderController>>());
            var result = await controller.GetById(3);
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_ValidOrder_ShouldReturnSuccess()
        {
            // Arrange
            _mockedService.Setup(s => s.InsertAsync(It.IsAny<Order>()));

            // auto_mapper configuration

            var controller = new OrderController(_mockedService.Object, _orderConfirmationService.Object, _mapper, Mock.Of<ILogger<OrderController>>());
            var createRequest = new OrderCreateRequest() {TotalPrice = 100, DiscountedPrice = 50};

            // Act
            var result = await controller.Post(createRequest);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>().Subject.Value.Should().NotBeNull()
                .And.Subject.Should().BeOfType<OrderViewResponse>().Should().NotBeNull();
        }

        [Fact]
        public async Task Put_OrderExisting_ShouldReturn200()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order() {Id = 1, TotalPrice = 100, DiscountedPrice = 50},
                new Order() {Id = 1, TotalPrice = 200, DiscountedPrice = 100},
            };

            var ordersGetRequest = new OrderUpdateRequest
            {
                TotalPrice = 100,
                DiscountedPrice = 50
            };

            _mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(orders.FirstOrDefault(h => h.Id == id)));
            _mockedService.Setup(s => s.Update(It.IsAny<Order>()));
            
            var controller = new OrderController(_mockedService.Object, _orderConfirmationService.Object, _mapper, Mock.Of<ILogger<OrderController>>());

            // Act
            var result = await controller.Put(1, ordersGetRequest);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Put_OrderExistent_ShouldReturnBadRequestResult()
        {
            var orders = new List<Order>
            {
                new Order() {TotalPrice = 100, DiscountedPrice = 50},
                new Order() {TotalPrice = 200, DiscountedPrice = 100},
            };

            _mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(orders.FirstOrDefault(h => h.Id == id)));

            _mockedService.Setup(s => s.Update(It.IsAny<Order>()));

            var controller = new OrderController(_mockedService.Object, _orderConfirmationService.Object, _mapper, Mock.Of<ILogger<OrderController>>());
            var orderUpdateRequest = new OrderUpdateRequest();

            var result = await controller.Put(3, orderUpdateRequest);
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task Delete_OrderExisting_ShouldReturnNoContent()
        {
            var orders = new List<Order>
            {
                new Order() {Id = 1, TotalPrice = 100, DiscountedPrice = 50},
                new Order() {Id = 2, TotalPrice = 200, DiscountedPrice = 100},
            };

            _mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(orders.FirstOrDefault(h => h.Id == id)));
            _mockedService.Setup(s => s.DeleteById(It.IsAny<int>()));

            var controller = new OrderController(_mockedService.Object, _orderConfirmationService.Object, _mapper, Mock.Of<ILogger<OrderController>>());
            var result = await controller.Delete(1);
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_OrderNotExisting_ShouldReturnNotFound()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order() {TotalPrice = 100, DiscountedPrice = 50},
                new Order() {TotalPrice = 200, DiscountedPrice = 100},
            };

            _mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(orders.FirstOrDefault(h => h.Id == id)));
            _mockedService.Setup(s => s.DeleteById(It.IsAny<int>()));

            var controller = new OrderController(_mockedService.Object, _orderConfirmationService.Object, _mapper, Mock.Of<ILogger<OrderController>>());

            // Action
            var result = await controller.Delete(3);


            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task ConfirmOrder_OrderIsExisting_ShouldCallOrderServiceAndReturn200()
        {
            var order = new Order();
            _mockedService.Setup(s => s.FirstOrDefaultAsync(It.IsAny<Expression<Func<Order, bool>>>()))
                .ReturnsAsync(order);
            _mockedService.Setup(s => s.ConfirmOrderById(It.IsAny<int>()))
                .ReturnsAsync(true);

            var controller = new OrderController(_mockedService.Object, _orderConfirmationService.Object, _mapper, Mock.Of<ILogger<OrderController>>());
            var result = await controller.ConfirmById(1);
            
            _mockedService.Verify(s => s.ConfirmOrderById(It.IsAny<int>()), Times.Once);
        }
        
        [Fact]
        public async Task ConfirmOrder_OrderIsNonExistent_ShouldReturnNotFound()
        {
            _mockedService.Setup(s => s.FirstOrDefaultAsync(It.IsAny<Expression<Func<Order, bool>>>()))
                .ReturnsAsync((Order) null);
            _mockedService.Setup(s => s.ConfirmOrderById(It.IsAny<int>()))
                .ReturnsAsync(true);

            var controller = new OrderController(_mockedService.Object, _orderConfirmationService.Object, _mapper, Mock.Of<ILogger<OrderController>>());
            var result = await controller.ConfirmById(1);
            result.Should().BeOfType<NotFoundResult>();
            
            _mockedService.Verify(s => s.ConfirmOrderById(It.IsAny<int>()), Times.Never);
        }
    }
}