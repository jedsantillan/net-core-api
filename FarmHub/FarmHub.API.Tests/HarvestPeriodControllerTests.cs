using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class HarvestPeriodControllerTests
    {
        [Fact]
        public async Task GetAll_HappyPath_ShouldReturnSuccess()
        {
            var harvests = new List<HarvestPeriod>
            {
                new HarvestPeriod(name: "Fake Name"),
                new HarvestPeriod(name: "Fake Name2"),
            };

            var mockedService = new Mock<IHarvestPeriodService>();
            mockedService.Setup(s => s.GetAllListAsync()).Returns(Task.FromResult(harvests));

            var controller = new HarvestPeriodController(mockedService.Object);

            var result = await controller.Get();

            result.Should().NotBeNull();
        }


        [Fact]
        public async Task GetById_HappyPath_ShouldReturn200()
        {
            var harvests = new List<HarvestPeriod>
            {
                new HarvestPeriod(name: "Fake Name")
                {
                    Id = 1
                },
                new HarvestPeriod(name: "Fake Name2")
                {
                    Id = 2
                },
            };

            var mockedService = new Mock<IHarvestPeriodService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(harvests.FirstOrDefault(h => h.Id == id)));

            var controller = new HarvestPeriodController(mockedService.Object);
            var result = await controller.GetById(1);
            var actionResult = result.Result;

            actionResult.Should().BeOfType<OkObjectResult>().Subject.StatusCode.Should().Be(200);
            actionResult.Should().BeOfType<OkObjectResult>().Subject.Value.Should().BeEquivalentTo(harvests.FirstOrDefault(h => h.Id == 1));
        }

        [Fact]
        public async Task GetById_HarvestPeriodNonExistent_ShouldReturn404()
        {
            var harvests = new List<HarvestPeriod>
            {
                new HarvestPeriod(name: "Fake Name")
                {
                    Id = 1
                },
                new HarvestPeriod(name: "Fake Name2")
                {
                    Id = 2
                },
            };

            var mockedService = new Mock<IHarvestPeriodService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(harvests.FirstOrDefault(h => h.Id == id)));

            var controller = new HarvestPeriodController(mockedService.Object);
            var result = await controller.GetById(3);
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_ValidHarvestPeriod_ShouldReturnSuccess()
        {
            var mockedService = new Mock<IHarvestPeriodService>();
            mockedService.Setup(s => s.InsertAsync(It.IsAny<HarvestPeriod>()));

            var controller = new HarvestPeriodController(mockedService.Object);

            var createRequest = new HarvestPeriodCreateRequest("test hp1");
            var result = await controller.Post(createRequest);
            result.Should().BeOfType<CreatedAtActionResult>().Subject.Value.Should().NotBeNull()
                .And.Subject.Should().BeOfType<HarvestPeriod>().Should().NotBeNull();
        }

        [Fact]
        public async Task Put_HarvestPeriodExisting_ShouldReturn200()
        {
            var harvests = new List<HarvestPeriod>
            {
                new HarvestPeriod(name: "Fake Name")
                {
                    Id = 1
                },
                new HarvestPeriod(name: "Fake Name2")
                {
                    Id = 2
                },
            };

            var mockedService = new Mock<IHarvestPeriodService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(harvests.FirstOrDefault(h => h.Id == id)));
            mockedService.Setup(s => s.Update(It.IsAny<HarvestPeriod>()));

            var controller = new HarvestPeriodController(mockedService.Object);
            var harvestPeriodUpdateRequest = new HarvestPeriodUpdateRequest();

            var result = await controller.Put(1, harvestPeriodUpdateRequest);
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Put_HarvestPeriodNonExistent_ShouldReturnBadRequestResult()
        {
            var harvests = new List<HarvestPeriod>
            {
                new HarvestPeriod(name: "Fake Name")
                {
                    Id = 1
                },
                new HarvestPeriod(name: "Fake Name2")
                {
                    Id = 2
                },
            };

            var mockedService = new Mock<IHarvestPeriodService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(harvests.FirstOrDefault(h => h.Id == id)));

            mockedService.Setup(s => s.Update(It.IsAny<HarvestPeriod>()));

            var controller = new HarvestPeriodController(mockedService.Object);
            var harvestPeriodUpdateRequest = new HarvestPeriodUpdateRequest();

            var result = await controller.Put(3, harvestPeriodUpdateRequest);
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task Delete_HarvestPeriodExisting_ShouldReturnNoContent()
        {
            var harvests = new List<HarvestPeriod>
            {
                new HarvestPeriod(name: "Fake Name")
                {
                    Id = 1
                },
                new HarvestPeriod(name: "Fake Name2")
                {
                    Id = 2
                },
            };

            var mockedService = new Mock<IHarvestPeriodService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(harvests.FirstOrDefault(h => h.Id == id)));
            mockedService.Setup(s => s.DeleteById(It.IsAny<int>()));

            var controller = new HarvestPeriodController(mockedService.Object);
            var result = await controller.Delete(1);
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_HarvestPeriodExisting_ShouldReturnNotFound()
        {
            var harvests = new List<HarvestPeriod>
            {
                new HarvestPeriod(name: "Fake Name")
                {
                    Id = 1
                },
                new HarvestPeriod(name: "Fake Name2")
                {
                    Id = 2
                },
            };

            var mockedService = new Mock<IHarvestPeriodService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(harvests.FirstOrDefault(h => h.Id == id)));
            mockedService.Setup(s => s.DeleteById(It.IsAny<int>()));

            var controller = new HarvestPeriodController(mockedService.Object);
            var result = await controller.Delete(3);
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
