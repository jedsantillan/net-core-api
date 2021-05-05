using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FarmHub.API.Controllers;
using FarmHub.API.Models;
using FarmHub.Application.Services.Repositories;
using FarmHub.Data.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FarmHub.API.Tests
{
    public class DiscountControllerTests
    {

        [Fact]
        public async Task GetAll_DiscountHappyPath_ShouldReturnSuccess()
        {
            var discounts = new List<Discount>
            {
                new Discount(discountValue: 20, discountType: DiscountTypeEnum.Percentage),
                new Discount(discountValue: 50, discountType: DiscountTypeEnum.FixedAmount),
            };

            var mockedService = new Mock<IDiscountService>();
            mockedService.Setup(s => s.GetAll()).Returns(discounts);

            // automapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();

            var controller = new DiscountController(mockedService.Object, mapper);
            var result = await controller.Get();
            result.Should().NotBeNull();
        }


        [Fact]
        public async Task GetById_DiscountHappyPath_ShouldReturn200()
        {
            // Arrange
            var discounts = new List<Discount>
            {
                new Discount(discountValue: 20, discountType: DiscountTypeEnum.Percentage)
                {
                    Id = 1
                },
                new Discount(discountValue: 50, discountType: DiscountTypeEnum.Percentage)
                {
                    Id = 2
                },
            };

            var discountViewResponse = new List<DiscountViewResponse>
            {
                new DiscountViewResponse()
                {
                    Id = 1,
                    DiscountValue = 20,
                    DiscountType = "Percentage"
                },
                new DiscountViewResponse()
                {
                    Id = 2,
                    DiscountValue = 50,
                    DiscountType = "FixedAmount"
                },
            };

            var mockedService = new Mock<IDiscountService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(discounts.FirstOrDefault(h => h.Id == id)));

            // automapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();

            var controller = new DiscountController(mockedService.Object, mapper);

            // Act
            var result = await controller.GetById(1);
            var actionResult = result.Result;

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>().Subject.StatusCode.Should().Be(200);
            actionResult.Should().BeOfType<OkObjectResult>().Subject.Value.Should().BeEquivalentTo(discountViewResponse.FirstOrDefault(h => h.Id == 1));
        }

        [Fact]
        public async Task GetById_DiscountNonExistent_ShouldReturn404()
        {
            // Arrange
            var discounts = new List<Discount>
            {
                new Discount(discountValue: 20, discountType: DiscountTypeEnum.Percentage)
                {
                    Id = 1
                },
                new Discount(discountValue: 50, discountType: DiscountTypeEnum.FixedAmount)
                {
                    Id = 2
                },
            };

            var discountViewResponse = new List<DiscountViewResponse>
            {
                new DiscountViewResponse()
                {
                    Id = 1,
                    DiscountValue = 20,
                    DiscountType = "Percentage"
                },
                new DiscountViewResponse()
                {
                    Id = 2,
                    DiscountValue = 50,
                    DiscountType = "FixedAmount"
                },
            };

            var mockedService = new Mock<IDiscountService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(discounts.FirstOrDefault(h => h.Id == id)));

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();

            var controller = new DiscountController(mockedService.Object, mapper);
            var result = await controller.GetById(3);
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_ValidDiscount_ShouldReturnSuccess()
        {
            // Arrange
            var mockedService = new Mock<IDiscountService>();
            mockedService.Setup(s => s.InsertAsync(It.IsAny<Discount>()));

            // automapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });
            var mapper = mockMapper.CreateMapper();

            var controller = new DiscountController(mockedService.Object, mapper);
            var createRequest = new DiscountCreateRequest(discountValue: 20, discountType: 0);

            // Act
            var result = await controller.Post(createRequest);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>().Subject.Value.Should().NotBeNull()
                .And.Subject.Should().BeOfType<Discount>().Should().NotBeNull();
        }

        [Fact]
        public async Task Put_DiscountExisting_ShouldReturn200()
        {
            // Arrange
            var discounts = new List<Discount>
            {
                new Discount(discountValue: 20, discountType: DiscountTypeEnum.Percentage)
                {
                    Id = 1
                },
                new Discount(discountValue: 50, discountType: DiscountTypeEnum.Percentage)
                {
                    Id = 2
                },
            };

            var discountUpdateRequest = new DiscountUpdateRequest()
            {
                Id = 1,
                DiscountValue = 30,
                DiscountType = DiscountTypeEnum.Percentage
            };

            var mockedService = new Mock<IDiscountService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(discounts.FirstOrDefault(h => h.Id == id)));
            mockedService.Setup(s => s.Update(It.IsAny<Discount>()));

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });
            var mapper = mockMapper.CreateMapper();

            var controller = new DiscountController(mockedService.Object, mapper);

            // Act
            var result = await controller.Put(1, discountUpdateRequest);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Put_DiscountExistent_ShouldReturnBadRequestResult()
        {
            var discounts = new List<Discount>
            {
                new Discount(discountValue: 20, discountType: DiscountTypeEnum.Percentage)
                {
                    Id = 1
                },
                new Discount(discountValue: 50, discountType: DiscountTypeEnum.Percentage)
                {
                    Id = 2
                },
            };


            var mockedService = new Mock<IDiscountService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(discounts.FirstOrDefault(h => h.Id == id)));

            mockedService.Setup(s => s.Update(It.IsAny<Discount>()));

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();


            var controller = new DiscountController(mockedService.Object, mapper);
            var discountUpdateRequest = new DiscountUpdateRequest();

            var result = await controller.Put(3, discountUpdateRequest);
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task Delete_DiscountExisting_ShouldReturnNoContent()
        {
            var discounts = new List<Discount>
            {
                new Discount(discountValue: 20, discountType: DiscountTypeEnum.Percentage)
                {
                    Id = 1
                },
                new Discount(discountValue: 50, discountType: DiscountTypeEnum.Percentage)
                {
                    Id = 2
                },
            };

            var mockedService = new Mock<IDiscountService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(discounts.FirstOrDefault(h => h.Id == id)));
            mockedService.Setup(s => s.DeleteById(It.IsAny<int>()));

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();

            var controller = new DiscountController(mockedService.Object, mapper);
            var result = await controller.Delete(1);
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_CategoryExisting_ShouldReturnNotFound()
        {
            // Arrange
            var discounts = new List<Discount>
            {
                new Discount(discountValue: 20, discountType: DiscountTypeEnum.Percentage)
                {
                    Id = 1
                },
                new Discount(discountValue: 50, discountType: DiscountTypeEnum.Percentage)
                {
                    Id = 2
                },
            };

            var mockedService = new Mock<IDiscountService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(discounts.FirstOrDefault(h => h.Id == id)));
            mockedService.Setup(s => s.DeleteById(It.IsAny<int>()));

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();

            var controller = new DiscountController(mockedService.Object, mapper);

            // Action
            var result = await controller.Delete(3);


            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }



    }
}
