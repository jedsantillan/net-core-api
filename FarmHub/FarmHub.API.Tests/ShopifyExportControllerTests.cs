using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using FarmHub.API.Controllers;
using FarmHub.API.Models;
using FarmHub.Application.Services.Infrastructure;
using FarmHub.Domain.Services.ShopifyReportGenerator;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FarmHub.API.Tests
{
    public class ShopifyExportControllerTests
    {
        [Fact]
        public async Task
            GetShopifyOrdersWithFilterSuccessful_ShouldReturn200AndCSVAndCallExportServiceWithFilterParams()
        {
            var mockShopifyService = new Mock<IShopifyExportService>();
            var mockReportGeneratorFactory = new Mock<IReportGeneratorFactory>();

            var httpContext = new DefaultHttpContext();
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };


            var controller =
                new ShopifyExportController(mockShopifyService.Object, mockReportGeneratorFactory.Object,
                    Mock.Of<ILogger<ShopifyExportController>>())
                {
                    ControllerContext = controllerContext
                };

            var filters = new ShopifyFilterRequest
            {
                DateStart = DateTime.Now.Subtract(new TimeSpan(5, 0, 0, 0)),
                DateEnd = DateTime.Now,
                Tags = new string[] {"test", "tag2"},
                ProductIds = new long?[] {1, 2, 3},
                PromoCodes = new string[] {"PC1", "PC2"},
                OrderIdStart = 1234,
                OrderIdEnd = 1234,
                TaggedOnly = true
            };

            dynamic eo1 = new ExpandoObject();
            eo1.Name = "Andrei Pogi";
            eo1.Address = "Pogi Land";
            eo1.Kangkong = 1;
            eo1.Tomato = 1;

            var expandoResult = new List<ExpandoObject>()
            {
                eo1
            };

            var shopifyResult = new List<ShopifySharp.Order>
            {
                new ShopifySharp.Order()
                {
                    Id = 1234
                }
            };

            var mockMasterListRg = new Mock<IReportGenerator<ExpandoObject>>();
            mockMasterListRg.Setup(s => s.Generate(It.IsAny<IEnumerable<ShopifySharp.Order>>())).Returns(expandoResult);
            mockReportGeneratorFactory.Setup(f => f.Create(It.IsAny<ShopifyReportType>()))
                .Returns(mockMasterListRg.Object);

            mockShopifyService.Setup(s => s.GetOrdersWithFilter(filters.DateStart, filters.DateEnd, filters.Tags,
                    filters.ProductIds, filters.PromoCodes, filters.OrderIdStart, filters.OrderIdEnd, filters.TaggedOnly))
                .ReturnsAsync(shopifyResult);

            var fileResult = await controller.GetOrders(filters);

            fileResult.As<FileContentResult>().Should().NotBeNull();
        }
    }
}