using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FarmHub.Application.Services.Repositories;
using FarmHub.Data;
using FarmHub.Data.Models;
using FarmHub.Domain.Services.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FarmHub.Infrastructure.Services
{
    public class ProductServiceTests_PortionOperations : DbContextTestBase
    {
        [Fact]
        public async Task UpdatePortions_AllPortionIdsNotAssignedToProduct_AllShouldBeAddedToTheDatabase()
        {
            var dbOptions = TestUtils.GetInMemoryDbOptions<CatalogDbContext>(Guid.NewGuid().ToString());
            var arrangeDbContext = new CatalogDbContext(dbOptions);
            var product = new Product("Talong", "talong123");
            arrangeDbContext.Products.Add(product);
            await arrangeDbContext.Portions.AddRangeAsync(new Portion("1/2", 0.5), new Portion("1/4", 0.25));
            arrangeDbContext.SaveChanges();

            var dbContext = new CatalogDbContext(dbOptions);
            var existingProduct = dbContext.Products.FirstOrDefault(p => p.Name == "Talong");

            var productService = new ProductService(It.IsAny<ILogger<ProductService>>(), dbContext);
            productService.UpdatePortions(existingProduct, dbContext.Set<Portion>().Select(p => p.Id));
            dbContext.SaveChanges();
            var productPortions = dbContext.ProductPortions.Where(pp => pp.ProductId == existingProduct.Id).ToList();

            productPortions.Should().HaveCount(2);
        }
        
        [Fact]
        public async Task UpdatePortions_PortionIdCollectionIsNull_ShouldNotThrowAnErrorAndShouldHaveZeroPortions()
        {
            var dbOptions = TestUtils.GetInMemoryDbOptions<CatalogDbContext>(Guid.NewGuid().ToString());
            var arrangeDbContext = new CatalogDbContext(dbOptions);
            var product = new Product("Talong", "talong123");
            arrangeDbContext.Products.Add(product);
            await arrangeDbContext.Portions.AddRangeAsync(new Portion("1/2", 0.5), new Portion("1/4", 0.25));
            arrangeDbContext.SaveChanges();

            var dbContext = new CatalogDbContext(dbOptions);
            var existingProduct = dbContext.Products.FirstOrDefault(p => p.Name == "Talong");

            var productService = new ProductService(It.IsAny<ILogger<ProductService>>(), dbContext);
            productService.UpdatePortions(existingProduct, null);
            
            dbContext.SaveChanges();
            var productPortions = dbContext.ProductPortions.Where(pp => pp.ProductId == existingProduct.Id).ToList();

            productPortions.Should().HaveCount(0);
        }

        [Fact]
        public async Task UpdatePortions_OnlyOnePortionIdIsNotAssignedToProduct_OnlyOneShouldBeAddedToTheDatabase()
        {
            var dbOptions = TestUtils.GetInMemoryDbOptions<CatalogDbContext>(Guid.NewGuid().ToString());
            var arrangeDbContext = new CatalogDbContext(dbOptions);

            var product = new Product("Talong", "talong123");
            arrangeDbContext.Products.Add(product);
            await arrangeDbContext.Portions.AddRangeAsync(new Portion("1/2", 0.5), new Portion("1/4", 0.25),
                new Portion("1", 1.0));
            arrangeDbContext.SaveChanges();
            product.ProductPortions.Add(new ProductPortion
            {
                PortionId = 1,
                ProductId = 1
            });
            product.ProductPortions.Add(new ProductPortion
            {
                PortionId = 2,
                ProductId = 1
            });
            arrangeDbContext.Products.Update(product);
            arrangeDbContext.SaveChanges();

            var dbContext = new CatalogDbContext(dbOptions);
            var existingProduct = dbContext.Products
                .Include(p => p.ProductPortions)
                .FirstOrDefault(p => p.Name == "Talong");

            var productService = new ProductService(It.IsAny<ILogger<ProductService>>(), dbContext);
            productService.UpdatePortions(existingProduct, dbContext.Set<Portion>().Select(p => p.Id));
            dbContext.SaveChanges();
            var productPortions = dbContext.ProductPortions.Where(pp => pp.ProductId == existingProduct.Id).ToList();

            productPortions.Should().HaveCount(3);
        }

        [Fact]
        public async Task UpdatePortions_AllPortionIdsIsAssignedToProduct_NoneShouldBeAddedToTheDatabase()
        {
            var dbOptions = TestUtils.GetInMemoryDbOptions<CatalogDbContext>(Guid.NewGuid().ToString());
            var arrangeDbContext = new CatalogDbContext(dbOptions);

            var product = new Product("Talong", "talong123");
            arrangeDbContext.Products.Add(product);
            await arrangeDbContext.Portions.AddRangeAsync(new Portion("1/2", 0.5), new Portion("1/4", 0.25),
                new Portion("1", 1.0));
            arrangeDbContext.SaveChanges();
            product.ProductPortions.Add(new ProductPortion
            {
                PortionId = 1,
                ProductId = 1
            });
            product.ProductPortions.Add(new ProductPortion
            {
                PortionId = 2,
                ProductId = 1
            });
            product.ProductPortions.Add(new ProductPortion
            {
                PortionId = 3,
                ProductId = 1
            });
            arrangeDbContext.Products.Update(product);
            arrangeDbContext.SaveChanges();

            var dbContext = new CatalogDbContext(dbOptions);
            var existingProduct = dbContext.Products
                .Include(p => p.ProductPortions)
                .FirstOrDefault(p => p.Name == "Talong");

            var productService = new ProductService(It.IsAny<ILogger<ProductService>>(), dbContext);
            productService.UpdatePortions(existingProduct, dbContext.Set<Portion>().Select(p => p.Id));
            dbContext.SaveChanges();
            var productPortions = dbContext.ProductPortions.Where(pp => pp.ProductId == existingProduct.Id).ToList();

            productPortions.Should().HaveCount(3);
        }
    }

    public class GetProductsByTag : DbContextTestBase
    {
        private IProductService _sut;
        private List<Product> _products;

        public GetProductsByTag()
        {
            var tag = new Tag("Featured");
            var tag2 = new Tag("Christmas");

            _products = new List<Product>
            {
                new Product("Apple", "APL")
                {
                    ProductTags = new List<Tag>
                    {
                        tag
                    }
                },
                new Product("Atis", "ATS")
                {
                    ProductTags = new List<Tag>
                    {
                        tag2
                    }
                },
                new Product("Mango", "MNG")
                {
                    ProductTags = new List<Tag>
                    {
                        tag,
                        tag2
                    }
                },
            };

            _context.Tags.AddRangeAsync(new List<Tag> {tag, tag2});
            _context.Products.AddRangeAsync(_products);
            _context.SaveChanges();

            _sut = new ProductService(It.IsAny<ILogger<ProductService>>(), _context);
        }

        [Theory]
        [InlineData("featured", 10)]
        [InlineData("Featured", 10)]
        [InlineData("Featured", 1)]
        [InlineData("Featured", null)]
        [InlineData("Featured", 0)]
        public async Task WhenATagIsPassedWithMatch_ThenShouldReturnAllProductsThatHasThatTag(string tag, int? limit)
        {
            var result = await _sut.GetAllByTagAsync(tag, limit);
            var expected = _products.Where(p =>
                p.ProductTags.Any(t => string.Equals(t.Name, tag, StringComparison.CurrentCultureIgnoreCase)));

            if (limit.HasValue) expected = expected.Take(limit.Value);
            result.Should().BeEquivalentTo(expected);
        }
    }
    
    public class GetProductsBySales : DbContextTestBase
    {
        private IProductService _sut;
        private List<Product> _products;

        public GetProductsBySales()
        {

            var portions = new List<Portion>()
            {
                new Portion("1/4", 0.25),
                new Portion("1/2", 0.50),
                new Portion("1", 1),
            };
            _products = new List<Product>
            {
                new Product("Apple", "APL"),
                new Product("Atis", "ATS"),
                new Product("Mango", "MNG"),
            };

            _context.Portions.AddRangeAsync(portions);
            
            _products[0].ProductPortions = new List<ProductPortion>()
            {
                new ProductPortion()
                {
                    Product = _products[0],
                    Portion = portions[0],
                    OrderItems = new List<OrderItem>()
                    {
                        new OrderItem()
                        {
                            Quantity = 10,
                            Order = Mock.Of<Order>()
                        }
                    }
                }
            };
            _products[1].ProductPortions = new List<ProductPortion>()
            {
                new ProductPortion()
                {
                    Product = _products[1],
                    Portion = portions[0],
                    OrderItems = new List<OrderItem>()
                    {
                        new OrderItem()
                        {
                            Quantity = 100,
                            Order = Mock.Of<Order>()
                        }
                    }
                }
            };
            
            _products[2].ProductPortions = new List<ProductPortion>()
            {
                new ProductPortion()
                {
                    Product = _products[2],
                    Portion = portions[0],
                    OrderItems = new List<OrderItem>()
                    {
                        new OrderItem()
                        {
                            Quantity = 1,
                            Order = Mock.Of<Order>()
                        }
                    }
                }
            };

            _context.Products.AddRangeAsync(_products);
            _context.SaveChanges();

            _sut = new ProductService(It.IsAny<ILogger<ProductService>>(), _context);
        }

        [Fact]
        public async Task WhenProductsHaveOrdersAndNumberOfItemsLessthanLimit_ThenShouldReturnDescendingOrderOfOrderCountSum()
        {
            var result = await _sut.GetAllBySales(10);
            var expected = _products.OrderByDescending(p => p.ProductPortions.SelectMany(pp => pp.OrderItems).Count());
            result.Should().BeEquivalentTo(expected);
        }
        
        
        [Fact]
        public async Task WhenProductsHaveOrdersAndNumberOfItemsGreaterthanLimit_ThenShouldReturnItemCountTheSameAsTheLimit()
        {
            for (var i = 0; i < 10; i++)
            {
                _context.Products.Add(new Product($"P{i}", $"P{i}"));
            }

            await _context.SaveChangesAsync();
            const int limit = 10;
            var result = await _sut.GetAllBySales(limit);
            var expected = _products.OrderByDescending(p => p.ProductPortions.SelectMany(pp => pp.OrderItems).Count());
            result.Count.Should().Be(limit);
        }
    }
}