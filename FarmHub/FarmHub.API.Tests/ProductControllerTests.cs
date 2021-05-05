using System.Collections.Generic;
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
    public class ProductControllerTests
    {
        private readonly Mock<IProductService> _mockedProductService;
        private readonly Mock<ITagService> _mockedTagService;
        private readonly Mock<IAzureStorageService> _azureStorageServiceMock;
        private MapperConfiguration _mapper;

        public ProductControllerTests()
        {
            _mockedProductService = new Mock<IProductService>();
            _mockedTagService = new Mock<ITagService>();
            _azureStorageServiceMock = new Mock<IAzureStorageService>();

            _mapper = new MapperConfiguration(cfg => { cfg.AddProfile(new AutoMapperProfile()); });
        }

        [Fact]
        public async Task GetAll_HappyPath_ShouldReturnSuccess()
        {
            var products = new List<Product>
            {
                new Product(name: "Fake Name", sku: "testsku"),
                new Product(name: "Fake Name2", sku: "testsku2"),
            };

            _mockedProductService.Setup(s => s.GetAll()).Returns(products);

            var controller = new ProductController(_mockedProductService.Object, _mockedTagService.Object, _azureStorageServiceMock.Object,
                _mapper.CreateMapper());

            var result = await controller.Get();

            result.Should().NotBeNull();
        }


        [Fact]
        public async Task GetById_HappyPath_ShouldReturn200()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product(name: "Fake Name", sku: "testsku")
                {
                    Id = 1
                },
                new Product(name: "Fake Name2", sku: "testsku2")
                {
                    Id = 2
                },
            };

            var productsGetRequest = new List<ProductViewResponse>
            {
                new ProductViewResponse()
                {
                    Id = 1,
                    SKU = "testsku",
                    Name = "Fake Name"
                },
                new ProductViewResponse()
                {
                    Id = 2,
                    SKU = "testsku2",
                    Name = "Fake Name2"
                },
            };

            _mockedProductService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(products.FirstOrDefault(h => h.Id == id)));

            var controller = new ProductController(_mockedProductService.Object, _mockedTagService.Object, _azureStorageServiceMock.Object,
                _mapper.CreateMapper());

            // Act
            var result = await controller.GetById(1);
            var actionResult = result.Result;

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>().Subject.StatusCode.Should().Be(200);
            //actionResult.Should().BeOfType<OkObjectResult>().Subject.Value.Should().BeEquivalentTo(productsGetRequest.FirstOrDefault(h => h.Id == 1));
        }

        [Fact]
        public async Task GetById_ProductNonExistent_ShouldReturn404()
        {
            var products = new List<Product>
            {
                new Product(name: "Fake Name", sku: "testsku")
                {
                    Id = 1
                },
                new Product(name: "Fake Name2", sku: "testsku2")
                {
                    Id = 2
                },
            };

            _mockedProductService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(products.FirstOrDefault(h => h.Id == id)));

            var controller = new ProductController(_mockedProductService.Object, _mockedTagService.Object, _azureStorageServiceMock.Object,
                _mapper.CreateMapper());
            var result = await controller.GetById(3);
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetAllProductImagesById_HappyPath_ShouldReturnSuccess()
        {
            var images = new List<Image>
            {
                new Image
                {
                    Id = 1,
                    ImageUrl = "http://127.0.0.1:10000/devstoreaccount1/mayani/images/fakeimage1.jpg",
                    ImageType = (int) ImageTypeEnum.ProductImage,
                    ProductId = 1
                },
                new Image
                {
                    Id = 2,
                    ImageUrl = "http://127.0.0.1:10000/devstoreaccount1/mayani/images/fakeimage2.jpg",
                    ImageType = (int) ImageTypeEnum.ProductImage,
                    ProductId = 1
                },
                new Image
                {
                    Id = 3,
                    ImageUrl = "http://127.0.0.1:10000/devstoreaccount1/mayani/images/fakeimage3.jpg",
                    ImageType = (int) ImageTypeEnum.ProductImage,
                    ProductId = null
                },
            };

            var imageRequest = new List<ImageViewResponse>
            {
                new ImageViewResponse()
                {
                    Id = 1,
                    Url = "http://127.0.0.1:10000/devstoreaccount1/mayani/images/fakeimage1.jpg",
                    ImageType = ImageTypeEnum.ProductImage.ToString(),
                    ProductId = 1
                },
                new ImageViewResponse()
                {
                    Id = 2,
                    Url = "http://127.0.0.1:10000/devstoreaccount1/mayani/images/fakeimage2.jpg",
                    ImageType = ImageTypeEnum.ProductImage.ToString(),
                    ProductId = 1
                },
            };


            _mockedProductService.Setup(s => s.GetAllImagesByProductIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(images.Where(h => h.ProductId == id).ToList()));

            var controller = new ProductController(_mockedProductService.Object, _mockedTagService.Object, _azureStorageServiceMock.Object,
                _mapper.CreateMapper());

            var result = await controller.GetAllProductImagesById(1);

            result.Should().NotBeNull();
            //result.Should().BeOfType<OkObjectResult>().Subject.StatusCode.Should().Be(200);
            //result.Should().BeOfType<OkObjectResult>().Subject.Value.Should().BeEquivalentTo(imageRequest.Where(x => x.ProductId == 1).Count() == 2);
        }

        [Fact]
        public async Task Create_ValidProduct_ShouldReturnSuccess()
        {
            // Arrange
            _mockedProductService.Setup(s => s.InsertAsync(It.IsAny<Product>()));

            var controller = new ProductController(_mockedProductService.Object, _mockedTagService.Object, _azureStorageServiceMock.Object,
                _mapper.CreateMapper());
            var createRequest = new ProductCreateRequest("Fake Prod", "test sku");

            createRequest.PortionIds = new int[1] {0};

            var formModel = new ProductCreateRequestForm()
            {
                Images = new List<IFormFile>(),
                Product = createRequest,
            };

            // Act
            var result = await controller.Post(formModel);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>().Subject.Value.Should().NotBeNull()
                .And.Subject.Should().BeOfType<Product>().Should().NotBeNull();
        }
        
        
        [Fact]
        public async Task Create_ValidProductWithTag_ShouldReturnSuccessAndCallTagService()
        {
            // Arrange
            _mockedProductService.Setup(s => s.InsertAsync(It.IsAny<Product>()));

            var createRequest = new ProductCreateRequest("Fake Prod", "test sku");

            createRequest.PortionIds = new int[1] {0};
            createRequest.Tags = new[] {"fruits", "veggies"};

            var formModel = new ProductCreateRequestForm()
            {
                Images = new List<IFormFile>(),
                Product = createRequest,
            };

            _mockedTagService.Setup(s => s.GetEntitiesForTagsAsync(It.IsAny<string[]>()))
                .ReturnsAsync(new[]
                {
                    new Tag(createRequest.Tags[0]),
                    new Tag(createRequest.Tags[1]),
                });

            var controller = new ProductController(_mockedProductService.Object, _mockedTagService.Object, _azureStorageServiceMock.Object,
                _mapper.CreateMapper());
            
            // Act
            var result = await controller.Post(formModel);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>().Subject.Value.Should().NotBeNull()
                .And.Subject.Should().BeOfType<Product>().Should().NotBeNull();
            
            _mockedTagService.Verify(s => s.GetEntitiesForTagsAsync(createRequest.Tags), Times.Once);
        }

        [Fact]
        public async Task Put_ProductExisting_ShouldReturn200()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product(name: "Fake Name", sku: "testsku")
                {
                    Id = 1
                },
                new Product(name: "Fake Name2", sku: "testsku2")
                {
                    Id = 2
                },
            };

            var putRequest = new ProductUpdateRequest
            {
                Id = 1,
                SKU = "testsku",
                Name = "Fake Name"
            };

            
            putRequest.Tags = new[] {"fruits", "veggies"};
            
            _mockedTagService.Setup(s => s.GetEntitiesForTagsAsync(It.IsAny<string[]>()))
                .ReturnsAsync(new[]
                {
                    new Tag(putRequest.Tags[0]),
                    new Tag(putRequest.Tags[1]),
                });
            _mockedProductService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(products.FirstOrDefault(h => h.Id == id)));
            _mockedProductService.Setup(s => s.Update(It.IsAny<Product>()));


            var controller = new ProductController(_mockedProductService.Object, _mockedTagService.Object, _azureStorageServiceMock.Object,
                _mapper.CreateMapper());

            // Act
            var result = await controller.Put(1, putRequest);

            // Assert
            result.Should().BeOfType<AcceptedAtActionResult>();
            _mockedTagService.Verify(s => s.GetEntitiesForTagsAsync(putRequest.Tags), Times.Once);

        }

        [Fact]
        public async Task Put_ProductExistent_ShouldReturnBadRequestResult()
        {
            var products = new List<Product>
            {
                new Product(name: "Fake Name", sku: "testsku")
                {
                    Id = 1
                },
                new Product(name: "Fake Name2", sku: "testsku2")
                {
                    Id = 2
                },
            };

            _mockedProductService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(products.FirstOrDefault(h => h.Id == id)));

            _mockedProductService.Setup(s => s.Update(It.IsAny<Product>()));

            var controller = new ProductController(_mockedProductService.Object, _mockedTagService.Object, _azureStorageServiceMock.Object,
                _mapper.CreateMapper());
            var productUpdateRequest = new ProductUpdateRequest();

            var result = await controller.Put(3, productUpdateRequest);
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task Delete_ProductExisting_ShouldReturnNoContent()
        {
            var products = new List<Product>
            {
                new Product(name: "Fake Name", sku: "testsku")
                {
                    Id = 1
                },
                new Product(name: "Fake Name2", sku: "testsku2")
                {
                    Id = 2
                },
            };

            _mockedProductService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(products.FirstOrDefault(h => h.Id == id)));
            _mockedProductService.Setup(s => s.DeleteById(It.IsAny<int>()));

            var controller = new ProductController(_mockedProductService.Object, _mockedTagService.Object, _azureStorageServiceMock.Object,
                _mapper.CreateMapper());
            var result = await controller.Delete(1);
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ProductExisting_ShouldReturnNotFound()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product(name: "Fake Name", sku: "testsku")
                {
                    Id = 1
                },
                new Product(name: "Fake Name2", sku: "testsku2")
                {
                    Id = 2
                },
            };

            _mockedProductService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(products.FirstOrDefault(h => h.Id == id)));
            _mockedProductService.Setup(s => s.DeleteById(It.IsAny<int>()));


            var controller = new ProductController(_mockedProductService.Object, _mockedTagService.Object, _azureStorageServiceMock.Object,
                _mapper.CreateMapper());

            // Action
            var result = await controller.Delete(3);


            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetProductsByTag_WhenValueIsPassed_ThenItShouldCallProductServiceWithParameter()
        {
            _mockedProductService.Setup(s => s.GetAllByTagAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(new List<Product>());
            
            var controller = new ProductController(_mockedProductService.Object, _mockedTagService.Object, _azureStorageServiceMock.Object,
                _mapper.CreateMapper());

            const string tag = "tag";
            const int limit = 10;
            await controller.GetProductsByTag(tag, limit);
            
            _mockedProductService.Verify(s => s.GetAllByTagAsync(tag, limit), Times.Once);
        } 
        
        [Fact]
        public async Task GetProductsByTag_WhenNoLimitIsPassed_ThenItShouldCallProductServiceWithDefaultLimitParameterOfOne()
        {
            _mockedProductService.Setup(s => s.GetAllByTagAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(new List<Product>());
            
            var controller = new ProductController(_mockedProductService.Object, _mockedTagService.Object, _azureStorageServiceMock.Object,
                _mapper.CreateMapper());

            const string tag = "tag";
            var result = await controller.GetProductsByTag(tag);
            
            _mockedProductService.Verify(s => s.GetAllByTagAsync(tag, 1), Times.Once);
        }

        [Fact]
        public async Task GetProductsBySales_WhenLimitIsPassed_ThenItShouldCallAndPassLimitToProductService()
        {
            _mockedProductService.Setup(s => s.GetAllBySales(It.IsAny<int>()))
                .ReturnsAsync(new List<Product>());
            
            var controller = new ProductController(_mockedProductService.Object, _mockedTagService.Object, _azureStorageServiceMock.Object,
                _mapper.CreateMapper());

            const int limit = 20;
            var result = await controller.GetProductsBySales(limit);

            result.Result.Should().BeOfType<OkObjectResult>().Subject.StatusCode.Should().Be(200);
            result.Result.As<OkObjectResult>().Value.Should().NotBeNull();
            _mockedProductService.Verify(s => s.GetAllBySales(limit), Times.Once);
        }
        
        [Fact]
        public async Task GetProductsBySales_WhenNoLimitIsPassed_ThenItShouldCallAndPassDefaultLimitToProductService()
        {
            _mockedProductService.Setup(s => s.GetAllBySales(It.IsAny<int>()))
                .ReturnsAsync(new List<Product>());
            
            var controller = new ProductController(_mockedProductService.Object, _mockedTagService.Object, _azureStorageServiceMock.Object,
                _mapper.CreateMapper());

            var result = await controller.GetProductsBySales();

            result.Result.Should().BeOfType<OkObjectResult>().Subject.StatusCode.Should().Be(200);
            result.Result.As<OkObjectResult>().Value.Should().NotBeNull();
            
            _mockedProductService.Verify(s => s.GetAllBySales(10), Times.Once);
        }
    }
}