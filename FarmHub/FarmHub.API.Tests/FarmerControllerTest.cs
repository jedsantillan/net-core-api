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
    public class FarmerControllerTest
    {
        [Fact]
        public void GetAll_HappyPath_ShouldReturnSuccess()
        {
            List<Farmer> farmers = new List<Farmer>
            {
                new Farmer { FirstName = "Fake Name" },
                new Farmer { FirstName = "Fake Name2" }
            };
            var mockedService = new Mock<IFarmerService>();
            mockedService.Setup(s => s.GetAll()).Returns(farmers);
            var controller = new FarmerController(mockedService.Object);
            var result = controller.Get();
            result.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetById_HappyPath_ShouldReturn200()
        {
            var farmers = new List<Farmer>
            {
                new Farmer { Id = 1, FirstName = "Fake Name"},
                new Farmer { Id = 2, FirstName = "Fake Name2"}
            };

            var mockedService = new Mock<IFarmerService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(farmers.FirstOrDefault(f => f.Id == id)));

            var controller = new FarmerController(mockedService.Object);
            var result = await controller.GetById(1);
            var actionResult = result.Result;

            actionResult.Should().BeOfType<OkObjectResult>().Subject.StatusCode.Should().Be(200);
            actionResult.Should().BeOfType<OkObjectResult>().Subject.Value.Should().BeEquivalentTo(farmers.FirstOrDefault(f => f.Id == 1));
        }

        [Fact]
        public async Task GetById_FarmerNonExistent_ShouldReturn404()
        {
            var farmers = new List<Farmer>
            {
                new Farmer{Id = 1, FirstName = "Fake Name"},
                new Farmer{Id = 2, FirstName = "Fake Name2"}
            };

            var mockedService = new Mock<IFarmerService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(farmers.FirstOrDefault(h => h.Id == id)));

            var controller = new FarmerController(mockedService.Object);
            var result = await controller.GetById(3);
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_ValidFarmer_ShouldReturnSuccess()
        {
            var mockedService = new Mock<IFarmerService>();
            mockedService.Setup(s => s.InsertAsync(It.IsAny<Farmer>()));

            var controller = new FarmerController(mockedService.Object);

            var createRequest = new FarmerCreateRequest();
            var result = await controller.Post(createRequest);
            result.Should().BeOfType<CreatedAtActionResult>().Subject.Value.Should().NotBeNull()
                .And.Subject.Should().BeOfType<Farmer>().Should().NotBeNull();
        }

        [Fact]
        public async Task Put_FarmerExisting_ShouldReturn200()
        {
            var farmers = new List<Farmer>
            {
                new Farmer{Id = 1, FirstName = "Fake Name"},
                new Farmer{Id = 2, FirstName = "Fake Name2"}
            };

            var mockedService = new Mock<IFarmerService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(farmers.FirstOrDefault(f => f.Id == id)));
            mockedService.Setup(s => s.Update(It.IsAny<Farmer>()));

            var controller = new FarmerController(mockedService.Object);
            var farmerUpdateRequest = new FarmerUpdateRequest();

            var result = await controller.Put(1, farmerUpdateRequest);
            result.Should().BeOfType<OkObjectResult>();
        }


        [Fact]
        public async Task Put_FarmerNonExistent_ShouldReturnBadRequestResult()
        {
            var farmers = new List<Farmer>
            {
                new Farmer{Id = 1, FirstName = "Fake Name"},
                new Farmer{Id = 2, FirstName = "Fake Name2"}
            };

            var mockedService = new Mock<IFarmerService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(farmers.FirstOrDefault(f => f.Id == id)));

            mockedService.Setup(s => s.Update(It.IsAny<Farmer>()));

            var controller = new FarmerController(mockedService.Object);
            var farmerUpdateRequest = new FarmerUpdateRequest();

            var result = await controller.Put(3, farmerUpdateRequest);
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task Delete_FarmerExisting_ShouldReturnNoContent()
        {
            var farmers = new List<Farmer>
            {
                new Farmer{Id = 1, FirstName = "Fake Name"},
                new Farmer{Id = 2, FirstName = "Fake Name2"}
            };

            var mockedService = new Mock<IFarmerService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(farmers.FirstOrDefault(f => f.Id == id)));
            mockedService.Setup(s => s.DeleteById(It.IsAny<int>()));

            var controller = new FarmerController(mockedService.Object);
            var result = await controller.Delete(1);
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_FarmerExisting_ShouldReturnNotFound()
        {
            var farmers = new List<Farmer>
            {
                new Farmer{Id = 1, FirstName = "Fake Name"},
                new Farmer{Id = 2, FirstName = "Fake Name2"}
            };

            var mockedService = new Mock<IFarmerService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(farmers.FirstOrDefault(h => h.Id == id)));
            mockedService.Setup(s => s.DeleteById(It.IsAny<int>()));

            var controller = new FarmerController(mockedService.Object);
            var result = await controller.Delete(3);
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}



