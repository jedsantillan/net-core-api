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
    public class PortionControllerTests
    {
        [Fact]
        public void GetAll_HappyPath_ShouldReturnSuccess()
        {
            var portions = new List<Portion>
            {
                new Portion(displayName: "1/2", realDecimalValue: 0.5f),
                new Portion(displayName: "1/4", realDecimalValue: 0.25f),
            };
            
            var mockedService = new Mock<IPortionService>();
            mockedService.Setup(s => s.GetAll()).Returns(portions);

            // automapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();
            var controller = new PortionController(mockedService.Object, mapper);
            var result = controller.Get();
            result.Should().NotBeNull();
        }
        
        
        [Fact]
        public async Task GetById_HappyPath_ShouldReturn200()
        {
            var portions = new List<Portion>
            {
                new Portion(displayName: "1/2", realDecimalValue: 0.5f)
                {
                    Id = 1
                },
                new Portion(displayName: "1/4", realDecimalValue: 0.25f)
                {
                    Id = 2
                },
            };

            var portionsGetRequest = new List<PortionViewResponse>
            {
                new PortionViewResponse()
                {
                    Id = 1,
                    DisplayName = "1/2",
                    RealDecimalValue = 0.5f
                },
                new PortionViewResponse()
                {
                    Id = 2,
                    DisplayName = "1/4",
                    RealDecimalValue = 0.25f
                },
            };

            var mockedService = new Mock<IPortionService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult( portions.FirstOrDefault(h => h.Id == id)));

            // automapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();

            var controller = new PortionController(mockedService.Object, mapper);
            var result = await controller.GetById(1);
            var actionResult = result.Result;

            actionResult.Should().BeOfType<OkObjectResult>().Subject.StatusCode.Should().Be(200);
            actionResult.Should().BeOfType<OkObjectResult>().Subject.Value.Should().BeEquivalentTo(portionsGetRequest.FirstOrDefault(h => h.Id == 1));
        }
        
        [Fact]
        public async Task GetById_PortionNonExistent_ShouldReturn404()
        {
            var portions = new List<Portion>
            {
                new Portion("1/2", realDecimalValue: 0.5f)
                {
                    Id = 1
                },
                new Portion("1/4", realDecimalValue: 0.25f)
                {
                    Id = 2
                },
            };
            
            var mockedService = new Mock<IPortionService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(portions.FirstOrDefault(h => h.Id == id)));

            // automapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();

            var controller = new PortionController(mockedService.Object, mapper);
            var result = await controller.GetById(3);
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_ValidPortion_ShouldReturnSuccess()
        {
            var mockedService = new Mock<IPortionService>();
            mockedService.Setup(s => s.InsertAsync(It.IsAny<Portion>()));

            // automapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();


            var controller = new PortionController(mockedService.Object, mapper);
            
            var createRequest = new PortionCreateRequest("1/2", realDecimalValue: 0.5f);
            var result = await controller.Post(createRequest);
            result.Should().BeOfType<CreatedAtActionResult>().Subject.Value.Should().NotBeNull()
                .And.Subject.Should().BeOfType<Portion>().Should().NotBeNull();
        }
        
        [Fact]
        public async Task Put_PortionExisting_ShouldReturn200()
        {
            var portions = new List<Portion>
            {
                new Portion(displayName: "1/2", realDecimalValue: 0.5f)
                {
                    Id = 1
                },
                new Portion(displayName: "1/4", realDecimalValue: 0.25f)
                {
                    Id = 2
                },
            };
            
            var mockedService = new Mock<IPortionService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(portions.FirstOrDefault(h => h.Id == id)));
            mockedService.Setup(s => s.Update(It.IsAny<Portion>()));

            // automapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();

            var controller = new PortionController(mockedService.Object, mapper);
            var harvestPeriodUpdateRequest = new PortionUpdateRequest();
            
            var result = await controller.Put(1, harvestPeriodUpdateRequest);
            result.Should().BeOfType<OkObjectResult>();
        }
        
        [Fact]
        public async Task Put_PortionNonExistent_ShouldReturnBadRequestResult()
        {
            var portions = new List<Portion>
            {
                new Portion(displayName: "1/2", realDecimalValue: 0.5f)
                {
                    Id = 1
                },
                new Portion(displayName: "1/4", realDecimalValue: 0.25f)
                {
                    Id = 2
                },
            };
            
            var mockedService = new Mock<IPortionService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(portions.FirstOrDefault(h => h.Id == id)));
            
            mockedService.Setup(s => s.Update(It.IsAny<Portion>()));

            // automapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();

            var controller = new PortionController(mockedService.Object, mapper);
            var harvestPeriodUpdateRequest = new PortionUpdateRequest();
            
            var result = await controller.Put(3, harvestPeriodUpdateRequest);
            result.Should().BeOfType<BadRequestResult>();
        }
        
        [Fact]
        public async Task Delete_PortionExisting_ShouldReturnNoContent()
        {
            var portions = new List<Portion>
            {
                new Portion(displayName: "1/2", realDecimalValue: 0.5f)
                {
                    Id = 1
                },
                new Portion(displayName: "1/4", realDecimalValue: 0.25f)
                {
                    Id = 2
                },
            };
            
            var mockedService = new Mock<IPortionService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(portions.FirstOrDefault(h => h.Id == id)));
            mockedService.Setup(s => s.DeleteById(It.IsAny<int>()));

            // automapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();

            var controller = new PortionController(mockedService.Object, mapper);
            var result = await controller.Delete(1);
            result.Should().BeOfType<NoContentResult>();
        }
        
        [Fact]
        public async Task Delete_PortionExisting_ShouldReturnNotFound()
        {
            var portions = new List<Portion>
            {
                new Portion(displayName: "1/2", realDecimalValue: 0.5f)
                {
                    Id = 1
                },
                new Portion(displayName: "1/4", realDecimalValue: 0.25f)
                {
                    Id = 2
                },
            };
            
            var mockedService = new Mock<IPortionService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(portions.FirstOrDefault(h => h.Id == id)));
            mockedService.Setup(s => s.DeleteById(It.IsAny<int>()));

            // automapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();

            var controller = new PortionController(mockedService.Object, mapper);
            var result = await controller.Delete(3);
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
