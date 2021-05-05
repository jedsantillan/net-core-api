using AutoMapper;
using FarmHub.API.Controllers;
using FarmHub.API.Models;
using FarmHub.Data.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FarmHub.Application.Services.Repositories;
using Xunit;

namespace FarmHub.API.Tests
{
    public class OrderItemControllerTest
    {
        [Fact]
        public void GetAll_HappyPath_ShouldReturnSuccess()
        {
            List<OrderItem> orderItems = new List<OrderItem>
            {
                new OrderItem { Quantity = 10 },
                new OrderItem { Quantity = 20 },
            };

            var mockedService = new Mock<IOrderItemService>();
            mockedService.Setup(s => s.GetAll()).Returns(orderItems);

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();
            var controller = new OrderItemController(mockedService.Object, mapper);

            var result = controller.Get();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetById_HappyPath_ShouldReturn200()
        {
            // Arrange
            var orderItems = new List<OrderItem>
            {
                new OrderItem() {Id = 1},
                new OrderItem() {Id = 2},
            };

            var orderItemGetRequest = new List<OrderItemViewResponse>
            {
                new OrderItemViewResponse()
                {
                    Quantity = 10
                },
                new OrderItemViewResponse()
                {
                    Quantity = 20
                },
            };

            var mockedService = new Mock<IOrderItemService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(orderItems.FirstOrDefault(h => h.Id == id)));

            // automapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();

            var controller = new OrderItemController(mockedService.Object, mapper);

            // Act
            var result = await controller.GetById(1);
            var actionResult = result.Result;

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>().Subject.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetById_OrderItemNonExistent_ShouldReturn404()
        {
            var orderItems = new List<OrderItem>
            {
                new OrderItem()
                {
                    Id = 1,
                },
                new OrderItem()
                {
                    Id = 2,
                },
            };

            var mockedService = new Mock<IOrderItemService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(orderItems.FirstOrDefault(h => h.Id == id)));

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();

            var controller = new OrderItemController(mockedService.Object, mapper);
            var result = await controller.GetById(3);
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_OrderItemExisting_ShouldReturnNoContent()
        {
            var orderItems = new List<OrderItem>
            {
                new OrderItem()
                {
                    Id = 1,
                    Quantity = 10
                },
                new OrderItem()
                {
                    Id = 2,
                    Quantity = 20
                },
            };

            var mockedService = new Mock<IOrderItemService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(orderItems.FirstOrDefault(h => h.Id == id)));
            mockedService.Setup(s => s.DeleteById(It.IsAny<int>()));

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();

            var controller = new OrderItemController(mockedService.Object, mapper);
            var result = await controller.Delete(1);
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_OrderItemExisting_ShouldReturnNotFound()
        {
            // Arrange
            var orderItems = new List<OrderItem>
            {
                new OrderItem()
                {
                    Id = 1,
                    Quantity = 10
                },
                new OrderItem()
                {
                    Id = 2,
                    Quantity = 20
                },
            };

            var mockedService = new Mock<IOrderItemService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(orderItems.FirstOrDefault(h => h.Id == id)));
            mockedService.Setup(s => s.DeleteById(It.IsAny<int>()));

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();

            var controller = new OrderItemController(mockedService.Object, mapper);

            // Action
            var result = await controller.Delete(3);


            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

    }
}
