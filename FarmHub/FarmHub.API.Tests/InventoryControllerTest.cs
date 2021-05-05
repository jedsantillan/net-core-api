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
    public class InventoryControllerTest
    {
        [Fact]
        public async Task GetAll_HappyPath_ShouldReturnSuccess()
        {
            var inventories = new List<Inventory>
            {
                new Inventory { Id = 1,Amount = 10 },
                new Inventory { Id = 2,Amount = 20 }
            };

            var mockedService = new Mock<IInventoryService>();
            mockedService.Setup(s => s.GetAllListAsync()).Returns(Task.FromResult(inventories));

            var controller = new InventoryController(mockedService.Object);

            var result = await controller.Get();

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetById_HappyPath_ShouldReturn200()
        {
            var inventories = new List<Inventory>
            {
                new Inventory { Id = 1,Amount = 10 },
                new Inventory { Id = 2,Amount = 20 }
            };

            var mockedService = new Mock<IInventoryService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(inventories.FirstOrDefault(f => f.Id == id)));

            var controller = new InventoryController(mockedService.Object);
            var result = await controller.GetById(1);
            var actionResult = result.Result;

            actionResult.Should().BeOfType<OkObjectResult>().Subject.StatusCode.Should().Be(200);
            actionResult.Should().BeOfType<OkObjectResult>().Subject.Value.Should().BeEquivalentTo(inventories.FirstOrDefault(f => f.Id == 1));
        }

        [Fact]
        public async Task GetById_InventoryNonExistent_ShouldReturn404()
        {
            var inventories = new List<Inventory>
            {
                new Inventory { Id = 1,Amount = 10 },
                new Inventory { Id = 2,Amount = 20 }
            };

            var mockedService = new Mock<IInventoryService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(inventories.FirstOrDefault(h => h.Id == id)));

            var controller = new InventoryController(mockedService.Object);
            var result = await controller.GetById(3);
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_ValidInventory_ShouldReturnSuccess()
        {
            var mockedService = new Mock<IInventoryService>();
            mockedService.Setup(s => s.InsertAsync(It.IsAny<Inventory>()));

            var controller = new InventoryController(mockedService.Object);

            var createRequest = new InventoryCreateRequest();
            var result = await controller.Post(createRequest);
            result.Should().BeOfType<CreatedAtActionResult>().Subject.Value.Should().NotBeNull()
                .And.Subject.Should().BeOfType<Inventory>().Should().NotBeNull();
        }

        [Fact]
        public async Task Put_InventoryExisting_ShouldReturn200()
        {
            var inventories = new List<Inventory>
            {
                new Inventory { Id = 1,Amount = 10 },
                new Inventory { Id = 2,Amount = 20 }
            };

            var mockedService = new Mock<IInventoryService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(inventories.FirstOrDefault(f => f.Id == id)));
            mockedService.Setup(s => s.Update(It.IsAny<Inventory>()));

            var controller = new InventoryController(mockedService.Object);
            var inventoryUpdateRequest = new InventoryUpdateRequest();

            var result = await controller.Put(1, inventoryUpdateRequest);
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Put_InventoryNonExistent_ShouldReturnBadRequestResult()
        {
            var inventories = new List<Inventory>
            {
                new Inventory { Id = 1,Amount = 10 },
                new Inventory { Id = 2,Amount = 20 }
            };

            var mockedService = new Mock<IInventoryService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(inventories.FirstOrDefault(f => f.Id == id)));

            mockedService.Setup(s => s.Update(It.IsAny<Inventory>()));

            var controller = new InventoryController(mockedService.Object);
            var inventoryUpdateRequest = new InventoryUpdateRequest();

            var result = await controller.Put(3, inventoryUpdateRequest);
            result.Should().BeOfType<BadRequestResult>();
        }


        [Fact]
        public async Task Delete_InventoryExisting_ShouldReturnNoContent()
        {
            var inventories = new List<Inventory>
            {
                new Inventory { Id = 1,Amount = 10 },
                new Inventory { Id = 2,Amount = 20 }
            };

            var mockedService = new Mock<IInventoryService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(inventories.FirstOrDefault(f => f.Id == id)));
            mockedService.Setup(s => s.DeleteById(It.IsAny<int>()));

            var controller = new InventoryController(mockedService.Object);
            var result = await controller.Delete(1);
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_InventoryExisting_ShouldReturnNotFound()
        {
            var inventories = new List<Inventory>
            {
                new Inventory { Id = 1,Amount = 10 },
                new Inventory { Id = 2,Amount = 20 }
            };

            var mockedService = new Mock<IInventoryService>();
            mockedService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(inventories.FirstOrDefault(h => h.Id == id)));
            mockedService.Setup(s => s.DeleteById(It.IsAny<int>()));

            var controller = new InventoryController(mockedService.Object);
            var result = await controller.Delete(3);
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
