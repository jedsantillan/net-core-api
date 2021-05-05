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
    public class CategoryControllerTests
    {
        [Fact]
        public async Task GetAll_HappyPath_ShouldReturnSuccess()
        {
            var categories = new List<Category>
            {
                new Category(name: "Fake Name"),
                new Category(name: "Fake Name2"),
            };

            var mockedService = new Mock<ICategoryService>();
            mockedService.Setup(s => s.GetAll()).Returns(categories);

            // automapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();

            var controller = new CategoryController(mockedService.Object, mapper);
            var result = await controller.Get();
            result.Should().NotBeNull();
        }


        [Fact]
        public async Task GetById_HappyPath_ShouldReturn200()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category(name: "Fake Name")
                {
                    Id = 1
                },
                new Category(name: "Fake Name2")
                {
                    Id = 2
                },
            };

            var categoriesGetRequest = new List<CategoryResponse>
            {
                new CategoryResponse()
                {
                    Id = 1, 
                    Name = "Fake Name"
                },
                new CategoryResponse()
                {
                    Id = 2,
                    Name = "Fake Name2"
                },
            };

            var mockedService = new Mock<ICategoryService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(categories.FirstOrDefault(h => h.Id == id)));

                // automapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();

            var controller = new CategoryController(mockedService.Object, mapper);

            // Act
            var result = await controller.GetById(1);
            var actionResult = result.Result;

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>().Subject.StatusCode.Should().Be(200);
            actionResult.Should().BeOfType<OkObjectResult>().Subject.Value.Should().BeEquivalentTo(categoriesGetRequest.FirstOrDefault(h => h.Id == 1));
        }

        [Fact]
        public async Task GetById_CategoryNonExistent_ShouldReturn404()
        {
            var categories = new List<Category>
            {
                new Category(name: "Fake Name")
                {
                    Id = 1
                },
                new Category(name: "Fake Name2")
                {
                    Id = 2
                },
            };

            var mockedService = new Mock<ICategoryService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(categories.FirstOrDefault(h => h.Id == id)));

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();

            var controller = new CategoryController(mockedService.Object, mapper);
            var result = await controller.GetById(3);
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_ValidCategory_ShouldReturnSuccess()
        {
            // Arrange
            var mockedService = new Mock<ICategoryService>();
            mockedService.Setup(s => s.InsertAsync(It.IsAny<Category>()));

                // automapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });
            var mapper = mockMapper.CreateMapper();

            var controller = new CategoryController(mockedService.Object, mapper);
            var createRequest = new CategoryCreateRequest("test hp1");
            
            // Act
            var result = await controller.Post(createRequest);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>().Subject.Value.Should().NotBeNull()
                .And.Subject.Should().BeOfType<Category>().Should().NotBeNull();
        }

        [Fact]
        public async Task Put_CategoryExisting_ShouldReturn200()
        {
            // Arrange
            var category = new List<Category>
            {
                new Category(name: "Fake Name")
                {
                    Id = 1
                },
                new Category(name: "Fake Name2")
                {
                    Id = 2
                },
            };

            var categorygetrequest = new CategoryResponse
            {
                Id = 1,
                Name = "Fake Name Change"
            };

            var mockedService = new Mock<ICategoryService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(category.FirstOrDefault(h => h.Id == id)));
            mockedService.Setup(s => s.Update(It.IsAny<Category>()));

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });
            var mapper = mockMapper.CreateMapper();

            var controller = new CategoryController(mockedService.Object, mapper);

            // Act
            var result = await controller.Put(1, categorygetrequest);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Put_CategoryExistent_ShouldReturnBadRequestResult()
        {
            var categories = new List<Category>
            {
                new Category(name: "Fake Name")
                {
                    Id = 1
                },
                new Category(name: "Fake Name2")
                {
                    Id = 2
                },
            };

            var mockedService = new Mock<ICategoryService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(categories.FirstOrDefault(h => h.Id == id)));

            mockedService.Setup(s => s.Update(It.IsAny<Category>()));

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();


            var controller = new CategoryController(mockedService.Object, mapper);
            var categorygetrequest = new CategoryResponse();

            var result = await controller.Put(3, categorygetrequest);
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task Delete_CategoryExisting_ShouldReturnNoContent()
        {
            var categories = new List<Category>
            {
                new Category(name: "Fake Name")
                {
                    Id = 1
                },
                new Category(name: "Fake Name2")
                {
                    Id = 2
                },
            };

            var mockedService = new Mock<ICategoryService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(categories.FirstOrDefault(h => h.Id == id)));
            mockedService.Setup(s => s.DeleteById(It.IsAny<int>()));

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();

            var controller = new CategoryController(mockedService.Object, mapper);
            var result = await controller.Delete(1);
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_CategoryExisting_ShouldReturnNotFound()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category(name: "Fake Name")
                {
                    Id = 1
                },
                new Category(name: "Fake Name2")
                {
                    Id = 2
                },
            };

            var mockedService = new Mock<ICategoryService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(categories.FirstOrDefault(h => h.Id == id)));
            mockedService.Setup(s => s.DeleteById(It.IsAny<int>()));

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();

            var controller = new CategoryController(mockedService.Object, mapper);

            // Action
            var result = await controller.Delete(3);


            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
