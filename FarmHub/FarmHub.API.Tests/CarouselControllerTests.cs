using System.Collections.Generic;
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
    public class CarouselControllerTests
    {
        private Mock<ICarouselService> _mockedCarouselService;
        private IMapper _mapper;

        public CarouselControllerTests()
        {
            _mockedCarouselService = new Mock<ICarouselService>();
            
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });
            _mapper = mockMapper.CreateMapper();
        }

        [Fact]
        public async Task GetById_HappyPath_ShouldReturnSuccess()
        {
            var carousel = new Carousel(name: "carousell name")
            {
                Id = 1,
                Items = new List<CarouselItem>()
                {
                    new CarouselItem(imageUrl: "test.jpg")
                    {
                        Id = 1,
                        CarouselId = 1,
                        ImageRedirectUrl = "www.nba.com",
                        Title = "This is the title",
                        Content = "This is the content",
                        ButtonCaption = "This is button",
                        ButtonLinkUrl = "This is button link",
                    }
                },
                Type = CarouselType.HomePage
            };

            _mockedCarouselService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(carousel);
            
            var controller = new CarouselController(_mockedCarouselService.Object, _mapper);

            var result = await controller.GetById(1);

            result.Should().NotBeNull();
        }
        
        [Fact]
        public async Task GetAll_HappyPath_ShouldReturnSuccess()
        {
            var carousel = new Carousel(name: "carousell name")
            {
                Id = 1,
                Items = new List<CarouselItem>()
                {
                    new CarouselItem(imageUrl: "test.jpg")
                    {
                        Id = 1,
                        CarouselId = 1,
                        ImageRedirectUrl = "www.nba.com",
                        Title = "This is the title",
                        Content = "This is the content",
                        ButtonCaption = "This is button",
                        ButtonLinkUrl = "This is button link",
                    }
                },
                Type = CarouselType.HomePage
            };

            _mockedCarouselService.Setup(s => s.GetCarouselByType(CarouselType.HomePage))
                .ReturnsAsync(new []{carousel});
            
            var controller = new CarouselController(_mockedCarouselService.Object, _mapper);

            var result = await controller.GetByType(CarouselType.HomePage);

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateCarousel_HappyPath_ShouldReturnSuccess()
        {
            var carouselRequest = new CarouselRequest
            {
                Name = "",
                Type = CarouselType.HomePage,
                Items = new[]
                {
                    new CarouselItemRequest()
                    {
                        ImageUrl = "",
                        ImageRedirectUrl = "www.nba.com",
                        Title = "This is the title",
                        Content = "This is the content",
                        ButtonCaption = "This is button",
                        ButtonLinkUrl = "This is button link",
                    }
                }
            };

            _mockedCarouselService.Setup(s => s.InsertAsync(It.IsAny<Carousel>()));
            
            var controller = new CarouselController(_mockedCarouselService.Object, _mapper);


            var result = await controller.Post(carouselRequest);
            
            result.Should().BeOfType<CreatedAtActionResult>().Subject.Value.Should().NotBeNull()
                .And.Subject.Should().BeOfType<Carousel>().Should().NotBeNull();
        }
    }
}