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
    public class ShippingAddressControllerTest
    {
        [Fact]
        public void GetAll_HappyPath_ShouldReturnSuccess()
        {
            List<ShippingAddress> shippingAddresss = new List<ShippingAddress>
            {
                new ShippingAddress { Id=1, Address = "Fake Address" },
                new ShippingAddress { Id=1, Address = "Fake Address2" }
            };

            var mockedService = new Mock<IShippingAddressService>();
            mockedService.Setup(s => s.GetAll()).Returns(shippingAddresss);

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();
            var controller = new ShippingAddressController(mockedService.Object, mapper);

            var result = controller.Get();
            result.Should().NotBeNull();
        }


        [Fact]
        public async Task GetById_HappyPath_ShouldReturn200()
        {
            // Arrange
            List<ShippingAddress> shippingAddresss = new List<ShippingAddress>
            {
                new ShippingAddress { Id=1, Address = "Fake Address" },
                new ShippingAddress { Id=1, Address = "Fake Address2" }
            };

            var shippingAddressGetRequest = new List<ShippingAddressViewResponse>
            {
                new ShippingAddressViewResponse()
                {
                    Address = "Fake Address"
                },
                new ShippingAddressViewResponse()
                {
                    Address = "Fake Address2"
                },
            };

            var mockedService = new Mock<IShippingAddressService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(shippingAddresss.FirstOrDefault(h => h.Id == id)));

            // automapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();

            var controller = new ShippingAddressController(mockedService.Object, mapper);

            // Act
            var result = await controller.GetById(1);
            var actionResult = result.Result;

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>().Subject.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetById_ShippingAddressNonExistent_ShouldReturn404()
        {
            var shippingAddresss = new List<ShippingAddress>
            {
                new ShippingAddress()
                {
                    Id = 1,
                },
                new ShippingAddress()
                {
                    Id = 2,
                },
            };

            var mockedService = new Mock<IShippingAddressService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(shippingAddresss.FirstOrDefault(h => h.Id == id)));

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();

            var controller = new ShippingAddressController(mockedService.Object, mapper);
            var result = await controller.GetById(3);
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_ValidShippingAddress_ShouldReturnSuccess()
        {
            // Arrange
            var mockedService = new Mock<IShippingAddressService>();
            mockedService.Setup(s => s.InsertAsync(It.IsAny<ShippingAddress>()));

            // automapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });
            var mapper = mockMapper.CreateMapper();

            var controller = new ShippingAddressController(mockedService.Object, mapper);
            var createRequest = new ShippingAddressCreateRequest() { Address = "Fake Address" };

            // Act
            var result = await controller.Post(createRequest);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>().Subject.Value.Should().NotBeNull()
                .And.Subject.Should().BeOfType<ShippingAddress>().Should().NotBeNull();
        }

        [Fact]
        public async Task Put_ShippingAddressExisting_ShouldReturn200()
        {
            // Arrange
            var shippingAddresss = new List<ShippingAddress>
            {
                new ShippingAddress()
                {
                    Id = 1,
                    Address = "Fake Address"
                },
                new ShippingAddress()
                {
                    Id = 2,
                    Address = "Fake Address2"
                },
            };

            var shippingAddresssGetRequest = new ShippingAddressUpdateRequest
            {

                Address = "Fake Address",
                City = "Fake City"
            };

            var mockedService = new Mock<IShippingAddressService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(shippingAddresss.FirstOrDefault(h => h.Id == id)));
            mockedService.Setup(s => s.Update(It.IsAny<ShippingAddress>()));

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });
            var mapper = mockMapper.CreateMapper();

            var controller = new ShippingAddressController(mockedService.Object, mapper);

            // Act
            var result = await controller.Put(1, shippingAddresssGetRequest);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }


        [Fact]
        public async Task Put_ShippingAddressNonExistent_ShouldReturnBadRequestResult()
        {
            var shippingAddresss = new List<ShippingAddress>
            {
                new ShippingAddress()
                {
                    Id = 1,
                    Address = "Fake Address"
                },
                new ShippingAddress()
                {
                    Id = 2,
                    Address = "Fake Address2"
                },
            };

            var mockedService = new Mock<IShippingAddressService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(shippingAddresss.FirstOrDefault(h => h.Id == id)));

            mockedService.Setup(s => s.Update(It.IsAny<ShippingAddress>()));

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();

            var controller = new ShippingAddressController(mockedService.Object, mapper);
            var shippingAddressUpdateRequest = new ShippingAddressUpdateRequest();

            var result = await controller.Put(3, shippingAddressUpdateRequest);
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task Delete_ShippingAddressExisting_ShouldReturnNoContent()
        {
            var shippingAddresss = new List<ShippingAddress>
            {
                new ShippingAddress()
                {
                    Id = 1,
                    Address = "Fake Address"
                },
                new ShippingAddress()
                {
                    Id = 2,
                    Address = "Fake Address2"
                },
            };

            var mockedService = new Mock<IShippingAddressService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(shippingAddresss.FirstOrDefault(h => h.Id == id)));
            mockedService.Setup(s => s.DeleteById(It.IsAny<int>()));

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();

            var controller = new ShippingAddressController(mockedService.Object, mapper);
            var result = await controller.Delete(1);
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ShippingAddressExisting_ShouldReturnNotFound()
        {
            // Arrange
            var shippingAddresss = new List<ShippingAddress>
            {
                new ShippingAddress()
                {
                    Id = 1,
                    Address = "Fake Address"
                },
                new ShippingAddress()
                {
                    Id = 2,
                    Address = "Fake Address2"
                },
            };

            var mockedService = new Mock<IShippingAddressService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(shippingAddresss.FirstOrDefault(h => h.Id == id)));
            mockedService.Setup(s => s.DeleteById(It.IsAny<int>()));

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();

            var controller = new ShippingAddressController(mockedService.Object, mapper);

            // Action
            var result = await controller.Delete(3);


            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
