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
    public class FarmerAssociationControllerTest
    {
        [Fact]
        public void GetAll_HappyPath_ShouldReturnSuccess()
        {
            List<FarmerAssociation> farmerAssociations = new List<FarmerAssociation>
            {
                new FarmerAssociation {Name = "Fake Name"},
                new FarmerAssociation {Name = "Fake Name2"}
            };

            var mockedService = new Mock<IFarmerAssociationService>();
            mockedService.Setup(s => s.GetAll()).Returns(farmerAssociations);
            var controller = new FarmerAssociationController(mockedService.Object);
            var result = controller.Get();
            result.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetById_HappyPath_ShouldReturn200()
        {
            var farmerAssociations = new List<FarmerAssociation>
            {
                new FarmerAssociation { Id = 1, Name = "Fake Name"},
                new FarmerAssociation { Id = 2, Name = "Fake Name2"}
            };

            var mockedService = new Mock<IFarmerAssociationService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(farmerAssociations.FirstOrDefault(f => f.Id == id)));

            var controller = new FarmerAssociationController(mockedService.Object);
            var result = await controller.GetById(1);
            var actionResult = result.Result;

            actionResult.Should().BeOfType<OkObjectResult>().Subject.StatusCode.Should().Be(200);
            actionResult.Should().BeOfType<OkObjectResult>().Subject.Value.Should().BeEquivalentTo(farmerAssociations.FirstOrDefault(f => f.Id == 1));
        }

        [Fact]
        public async Task GetById_FarmerAssociationNonExistent_ShouldReturn404()
        {
            var farmerAssociations = new List<FarmerAssociation>
            {
                new FarmerAssociation{Id = 1, Name = "Fake Name"},
                new FarmerAssociation{Id = 2, Name = "Fake Name2"}
            };

            var mockedService = new Mock<IFarmerAssociationService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(farmerAssociations.FirstOrDefault(f => f.Id == id)));

            var controller = new FarmerAssociationController(mockedService.Object);
            var result = await controller.GetById(3);
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_ValidFarmerAssociation_ShouldReturnSuccess()
        {
            var mockedService = new Mock<IFarmerAssociationService>();
            mockedService.Setup(s => s.InsertAsync(It.IsAny<FarmerAssociation>()));

            var controller = new FarmerAssociationController(mockedService.Object);

            var createRequest = new FarmerAssociationCreateRequest();
            var result = await controller.Post(createRequest);
            result.Should().BeOfType<CreatedAtActionResult>().Subject.Value.Should().NotBeNull()
                .And.Subject.Should().BeOfType<FarmerAssociation>().Should().NotBeNull();
        }

        [Fact]
        public async Task Put_FarmerAssociationExisting_ShouldReturn200()
        {
            var farmerAssociations = new List<FarmerAssociation>
            {
                new FarmerAssociation{Id = 1, Name = "Fake Name"},
                new FarmerAssociation{Id = 2, Name = "Fake Name2"}
            };

            var mockedService = new Mock<IFarmerAssociationService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(farmerAssociations.FirstOrDefault(f => f.Id == id)));
            mockedService.Setup(s => s.Update(It.IsAny<FarmerAssociation>()));

            var controller = new FarmerAssociationController(mockedService.Object);
            var farmerAssociationUpdateRequest = new FarmerAssociationUpdateRequest();

            var result = await controller.Put(1, farmerAssociationUpdateRequest);
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Put_FarmerAssociationNonExistent_ShouldReturnBadRequestResult()
        {
            var farmerAssociations = new List<FarmerAssociation>
            {
                new FarmerAssociation{Id = 1, Name = "Fake Name"},
                new FarmerAssociation{Id = 2, Name = "Fake Name2"}
            };

            var mockedService = new Mock<IFarmerAssociationService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(farmerAssociations.FirstOrDefault(f => f.Id == id)));

            mockedService.Setup(s => s.Update(It.IsAny<FarmerAssociation>()));

            var controller = new FarmerAssociationController(mockedService.Object);
            var farmerAssociationUpdateRequest = new FarmerAssociationUpdateRequest();

            var result = await controller.Put(3, farmerAssociationUpdateRequest);
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task Delete_FarmerAssociationExisting_ShouldReturnNoContent()
        {
            var farmerAssociations = new List<FarmerAssociation>
            {
                new FarmerAssociation{Id = 1, Name = "Fake Name"},
                new FarmerAssociation{Id = 2, Name = "Fake Name2"}
            };

            var mockedService = new Mock<IFarmerAssociationService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(farmerAssociations.FirstOrDefault(f => f.Id == id)));
            mockedService.Setup(s => s.DeleteById(It.IsAny<int>()));

            var controller = new FarmerAssociationController(mockedService.Object);
            var result = await controller.Delete(1);
            result.Should().BeOfType<NoContentResult>();
        }


        [Fact]
        public async Task Delete_FarmerAssociationExisting_ShouldReturnNotFound()
        {
            var farmerAssociations = new List<FarmerAssociation>
            {
                new FarmerAssociation{Id = 1, Name = "Fake Name"},
                new FarmerAssociation{Id = 2, Name = "Fake Name2"}
            };

            var mockedService = new Mock<IFarmerAssociationService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(farmerAssociations.FirstOrDefault(h => h.Id == id)));
            mockedService.Setup(s => s.DeleteById(It.IsAny<int>()));

            var controller = new FarmerAssociationController(mockedService.Object);
            var result = await controller.Delete(3);
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
