using AutoMapper;
using FarmHub.API.Controllers;
using FarmHub.API.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FarmHub.Application.Services.Repositories;
using FarmHub.Data.Models;
using Xunit;

namespace FarmHub.API.Tests
{
    public class CustomerControllerTest
    {
        [Fact]
        public void GetAll_HappyPath_ShouldReturnSuccess()
        {
            List<Customer> customers = new List<Customer>
            {
                new Customer { FirstName = "Fake Name" },
                new Customer { FirstName = "Fake Name2" }
            };

            var mockedService = new Mock<ICustomerService>();
            mockedService.Setup(s => s.GetAll()).Returns(customers);

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();
            var controller = new CustomerController(mockedService.Object, mapper);

            var result = controller.Get();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetById_HappyPath_ShouldReturn200()
        {
            // Arrange
            var customers = new List<Customer>
            {
                new Customer() {Id = 1},
                new Customer() {Id = 2},
            };

            var customerGetRequest = new List<CustomerViewResponse>
            {
                new CustomerViewResponse()
                {
                    FirstName = "Fake Name"
                },
                new CustomerViewResponse()
                {
                    FirstName = "Fake Name2"
                },
            };

            var mockedService = new Mock<ICustomerService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(customers.FirstOrDefault(h => h.Id == id)));

            // automapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();

            var controller = new CustomerController(mockedService.Object, mapper);

            // Act
            var result = await controller.GetById(1);
            var actionResult = result.Result;

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>().Subject.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetById_CustomerNonExistent_ShouldReturn404()
        {
            var customers = new List<Customer>
            {
                new Customer()
                {
                    Id = 1,
                },
                new Customer()
                {
                    Id = 2,
                },
            };

            var mockedService = new Mock<ICustomerService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(customers.FirstOrDefault(h => h.Id == id)));

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();

            var controller = new CustomerController(mockedService.Object, mapper);
            var result = await controller.GetById(3);
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_ValidCustomer_ShouldReturnSuccess()
        {
            // Arrange
            var mockedService = new Mock<ICustomerService>();
            mockedService.Setup(s => s.InsertAsync(It.IsAny<Customer>()));

            // automapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });
            var mapper = mockMapper.CreateMapper();

            var controller = new CustomerController(mockedService.Object, mapper);
            var createRequest = new CustomerCreateRequest() { FirstName = "FakeName"};

            // Act
            var result = await controller.Post(createRequest);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>().Subject.Value.Should().NotBeNull()
                .And.Subject.Should().BeOfType<Customer>().Should().NotBeNull();
        }

        [Fact]
        public async Task Put_CustomerExisting_ShouldReturn200()
        {
            // Arrange
            var customers = new List<Customer>
            {
                new Customer()
                {
                    Id = 1,
                    FirstName = "Fake Name"
                },
                new Customer()
                {
                    Id = 2,
                    FirstName = "Fake Name2"
                },
            };

            var customersGetRequest = new CustomerUpdateRequest
            {
                
                FirstName = "Fake Name",
                LastName = "Fake Last Name"
            };

            var mockedService = new Mock<ICustomerService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(customers.FirstOrDefault(h => h.Id == id)));
            mockedService.Setup(s => s.Update(It.IsAny<Customer>()));

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });
            var mapper = mockMapper.CreateMapper();

            var controller = new CustomerController(mockedService.Object, mapper);

            // Act
            var result = await controller.Put(1, customersGetRequest);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }


        [Fact]
        public async Task Put_CustomerNonExistent_ShouldReturnBadRequestResult()
        {
            var customers = new List<Customer>
            {
                new Customer()
                {
                    Id = 1,
                    FirstName = "Fake Name"
                },
                new Customer()
                {
                    Id = 2,
                    FirstName = "Fake Name2"
                },
            };

            var mockedService = new Mock<ICustomerService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(customers.FirstOrDefault(h => h.Id == id)));

            mockedService.Setup(s => s.Update(It.IsAny<Customer>()));

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();

            var controller = new CustomerController(mockedService.Object, mapper);
            var customerUpdateRequest = new CustomerUpdateRequest();

            var result = await controller.Put(3, customerUpdateRequest);
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task Delete_CustomerExisting_ShouldReturnNoContent()
        {
            var customers = new List<Customer>
            {
                new Customer()
                {
                    Id = 1,
                    FirstName = "Fake Name"
                },
                new Customer()
                {
                    Id = 2,
                    FirstName = "Fake Name2"
                },
            };

            var mockedService = new Mock<ICustomerService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(customers.FirstOrDefault(h => h.Id == id)));
            mockedService.Setup(s => s.DeleteById(It.IsAny<int>()));

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();

            var controller = new CustomerController(mockedService.Object, mapper);
            var result = await controller.Delete(1);
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_CustomerExisting_ShouldReturnNotFound()
        {
            // Arrange
            var customers = new List<Customer>
            {
                new Customer()
                {
                    Id = 1,
                    FirstName = "Fake Name"
                },
                new Customer()
                {
                    Id = 2,
                    FirstName = "Fake Name2"
                },
            };

            var mockedService = new Mock<ICustomerService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(customers.FirstOrDefault(h => h.Id == id)));
            mockedService.Setup(s => s.DeleteById(It.IsAny<int>()));

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();

            var controller = new CustomerController(mockedService.Object, mapper);

            // Action
            var result = await controller.Delete(3);


            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
