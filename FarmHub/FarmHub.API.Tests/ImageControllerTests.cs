using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FarmHub.API.Controllers;
using FarmHub.API.Models;
using FarmHub.Application.Services.Infrastructure;
using FarmHub.Application.Services.Repositories;
using FarmHub.Data.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FarmHub.API.Tests
{
    public class ImageControllerTests
    {
        [Fact]
        public async Task GetAll_HappyPath_ShouldReturnSuccess()
        {
            var images = new List<Image>
            {
                new Image(imageUrl: "http://127.0.0.1:10000/devstoreaccount1/mayani/images/fakeimage1.jpg", imageType: (int)ImageTypeEnum.ProductImage),
                new Image(imageUrl: "http://127.0.0.1:10000/devstoreaccount1/mayani/images/fakeimage2.jpg", imageType: (int)ImageTypeEnum.ProductImage),
            };

            var mockedService = new Mock<IImageService>();
            mockedService.Setup(s => s.GetAll()).Returns(images);

            var mockedStorageService = new Mock<IAzureStorageService>();

            // automapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();
            var controller = new ImageController(mockedService.Object, mockedStorageService.Object, mapper);

            var result = await controller.Get();

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetById_HappyPath_ShouldReturn200()
        {
            // Arrange
            var images = new List<Image>
            {
                new Image(imageUrl: "http://127.0.0.1:10000/devstoreaccount1/mayani/images/fakeimage1.jpg", imageType: (int)ImageTypeEnum.ProductImage)
                {
                    Id = 1,
                    Title = "Fake Image Name 1"
                },
                new Image(imageUrl: "http://127.0.0.1:10000/devstoreaccount1/mayani/images/fakeimage2.jpg", imageType: (int)ImageTypeEnum.ProductImage)
                {
                    Id = 2,
                    Title = "Fake Image Name 2"
                },
            };

            var imageRequest = new List<ImageViewResponse>
            {
                new ImageViewResponse()
                {
                    Id = 1,
                    Url = "http://127.0.0.1:10000/devstoreaccount1/mayani/images/fakeimage1.jpg",
                    ImageType = ImageTypeEnum.ProductImage.ToString(),
                    Title = "Fake Image Name 1"
                },
                new ImageViewResponse()
                {
                    Id = 2,
                    Url = "http://127.0.0.1:10000/devstoreaccount1/mayani/images/fakeimage2.jpg",
                    ImageType = ImageTypeEnum.ProductImage.ToString(),
                    Title = "Fake Image Name 2"
                },
            };

            var mockedService = new Mock<IImageService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(images.FirstOrDefault(h => h.Id == id)));

            var mockedStorageService = new Mock<IAzureStorageService>();

            // automapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();

            var controller = new ImageController(mockedService.Object, mockedStorageService.Object, mapper);

            // Act
            var result = await controller.GetById(1);
            var actionResult = result.Result;

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>().Subject.StatusCode.Should().Be(200);
            //actionResult.Should().BeOfType<OkObjectResult>().Subject.Value.Should().BeEquivalentTo(imageRequest.FirstOrDefault(h => h.Id == 1));
        }

        [Fact]
        public async Task GetById_ImageNonExistent_ShouldReturn404()
        {
            // Arrange
            var images = new List<Image>
            {
                new Image(imageUrl: "http://127.0.0.1:10000/devstoreaccount1/mayani/images/fakeimage1.jpg", imageType: (int)ImageTypeEnum.ProductImage)
                {
                    Id = 1,
                    Title = "Fake Image Name 1"
                },
                new Image(imageUrl: "http://127.0.0.1:10000/devstoreaccount1/mayani/images/fakeimage1.jpg", imageType: (int)ImageTypeEnum.ProductImage)
                {
                    Id = 2,
                    Title = "Fake Image Name 2"
                },
            };

            var mockedService = new Mock<IImageService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(images.FirstOrDefault(h => h.Id == id)));

            var mockedStorageService = new Mock<IAzureStorageService>();

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();

            var controller = new ImageController(mockedService.Object, mockedStorageService.Object, mapper);
            var result = await controller.GetById(3);
            result.Result.Should().BeOfType<NotFoundResult>();
        }


        [Fact]
        public async Task Create_ValidImage_ShouldReturnSuccess()
        {
            // Arrange
            var mockedService = new Mock<IImageService>();
            mockedService.Setup(s => s.InsertAsync(It.IsAny<Image>()));

            var mockedStorageService = new Mock<IAzureStorageService>();

            var fileMock = new Mock<IFormFile>();

            //Setup mock file using a memory stream
            var content = "This is a test image";
            var fileName = "test.jpg";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            // automapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });
            var mapper = mockMapper.CreateMapper();

            var mockedFile = fileMock.Object; // assign the mocked file

            var controller = new ImageController(mockedService.Object, mockedStorageService.Object, mapper);
            var createRequest = new ImageRequest(mockedFile, 0);

            // Act
            var result = await controller.Post(createRequest);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>().Subject.Value.Should().NotBeNull()
                .And.Subject.Should().BeOfType<Image>().Should().NotBeNull();
        }


    }
}
