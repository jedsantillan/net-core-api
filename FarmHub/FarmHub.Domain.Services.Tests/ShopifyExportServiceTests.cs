using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FarmHub.Domain.Services;
using FarmHub.Domain.Services.ShopifyReportGenerator;
using FluentAssertions;
using Moq;
using ShopifySharp;
using ShopifySharp.Lists;
using Xunit;

namespace FarmHub.Application.Services.Tests
{
    public class ShopifyExportServiceTests
    {
        [Fact]
        public async Task WhenOrderIdAndTagsFiltersArePassed_ItShouldIncludeTheOrdersInTheIDRangeAsWellAsWithTheTag()
        {
            var orders = new List<Order>
            {
                new Order()
                {
                    OrderNumber = 1,
                    LineItems = new List<LineItem>()
                },
                new Order()
                {
                    OrderNumber = 2,
                    LineItems = new List<LineItem>()
                },
                new Order()
                {
                    OrderNumber = 3,
                    LineItems = new List<LineItem>()
                },
                new Order()
                {
                    OrderNumber = 4,
                    LineItems = new List<LineItem>()
                },
                new Order()
                {
                    OrderNumber = 5,
                    LineItems = new List<LineItem>(),
                    Tags = "Exempt"
                },
            };

            var orderService = new Mock<ShopifySharp.OrderService>("myShopifyUrl", "accessToken");
            orderService.Setup(o => o.ListAsync(It.IsAny<MayaniOrderFilter>(), CancellationToken.None))
                .ReturnsAsync(new ListResult<Order>(orders, null));

            var productService = new Mock<ShopifySharp.ProductService>("myShopifyUrl", "accessToken");
            var exportService = new ShopifyExportService(orderService.Object,  productService.Object);
            var result = await exportService.GetOrdersWithFilter(DateTime.Now, DateTime.Now, new[] {"Exempt"},
                new long?[] { },
                new string[] { }, 1, 3, false);

            result.Should().NotBeNull().And.Contain(o => o.OrderNumber.Value == 1)
                .And.Contain(o => o.OrderNumber.Value == 2)
                .And.Contain(o => o.OrderNumber.Value == 3)
                .And.Contain(o => o.OrderNumber.Value == 5);
        }
        
        [Fact]
        public async Task WhenNoOrderIdAndNoTagIsGiven_ItShouldReturnAll()
        {
            var orders = new List<Order>
            {
                new Order()
                {
                    OrderNumber = 1,
                    LineItems = new List<LineItem>()
                },
                new Order()
                {
                    OrderNumber = 2,
                    LineItems = new List<LineItem>()
                },
                new Order()
                {
                    OrderNumber = 3,
                    LineItems = new List<LineItem>()
                },
                new Order()
                {
                    OrderNumber = 4,
                    LineItems = new List<LineItem>()
                },
                new Order()
                {
                    OrderNumber = 5,
                    LineItems = new List<LineItem>(),
                    Tags = "Exempt"
                },
            };

            var orderService = new Mock<ShopifySharp.OrderService>("myShopifyUrl", "accessToken");
            orderService.Setup(o => o.ListAsync(It.IsAny<MayaniOrderFilter>(), CancellationToken.None))
                .ReturnsAsync(new ListResult<Order>(orders, null));

            var productService = new Mock<ShopifySharp.ProductService>("myShopifyUrl", "accessToken");
            var exportService = new ShopifyExportService(orderService.Object,  productService.Object);
            var result = await exportService.GetOrdersWithFilter(DateTime.Now, DateTime.Now, null,
                null,
                null, null, null);

            result.Should().NotBeNull().And.HaveCount(5);
        }


        [Fact]
        public async Task WhenTheProductIdIsUsedAsFilter_ItShouldReturnOrdersThatIncludesOneOfTheProductsSpecified()
        {
            var orders = new List<Order>
            {
                new Order()
                {
                    OrderNumber = 1,
                    LineItems = new List<LineItem>()
                },
                new Order()
                {
                    OrderNumber = 2,
                    LineItems = new List<LineItem>()
                    {
                        new LineItem()
                        {
                            ProductId = 3,
                            FulfillableQuantity = 20
                        }
                    }
                },
                new Order()
                {
                    OrderNumber = 3,
                    LineItems = new List<LineItem>()
                    {
                        new LineItem()
                        {
                            ProductId = 2,
                            FulfillableQuantity = 0
                        }
                    }
                },
                new Order()
                {
                    OrderNumber = 4,
                    LineItems = new List<LineItem>()
                    {
                        new LineItem()
                        {
                            ProductId = 1,
                            FulfillableQuantity = 20
                        }
                    }
                },
                new Order()
                {
                    OrderNumber = 5,
                    LineItems = new List<LineItem>(),
                    Tags = "Exempt"
                },
            };


            var orderService = new Mock<ShopifySharp.OrderService>("myShopifyUrl", "accessToken");
            orderService.Setup(o => o.ListAsync(It.IsAny<MayaniOrderFilter>(), CancellationToken.None))
                .ReturnsAsync(new ListResult<Order>(orders, null));

            var productService = new Mock<ShopifySharp.ProductService>("myShopifyUrl", "accessToken");
            var exportService = new ShopifyExportService(orderService.Object,  productService.Object);
            var result = await exportService.GetOrdersWithFilter(DateTime.Now, DateTime.Now, new[] {"Exempt"},
                new long?[] {1, 2, 3},
                new string[] { }, 1, 4, false);

            result.Should().NotBeNull()
                .And.Contain(o => o.OrderNumber == 4)
                .And.Contain(o => o.OrderNumber == 2);
        }
    }
}