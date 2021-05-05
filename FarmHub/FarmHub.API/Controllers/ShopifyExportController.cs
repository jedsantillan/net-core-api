using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using FarmHub.API.Models;
using FarmHub.Application.Services.Infrastructure;
using FarmHub.Domain.Services.ShopifyReportGenerator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FarmHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopifyExportController : ControllerBase
    {
        private IShopifyExportService _shopifyExportService;
        private ILogger<ShopifyExportController> _logger;
        private IReportGeneratorFactory _reportGeneratorFactory;

        public ShopifyExportController(IShopifyExportService shopifyExportService,
            IReportGeneratorFactory reportGeneratorFactory,
            ILogger<ShopifyExportController> logger)
        {
            _reportGeneratorFactory = reportGeneratorFactory;
            _shopifyExportService = shopifyExportService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult> GetOrders(ShopifyFilterRequest filters)
        {
            
            var orders = await _shopifyExportService.GetOrdersWithFilter(filters.DateStart, filters.DateEnd, filters.Tags,
                filters.ProductIds, filters.PromoCodes, filters.OrderIdStart, filters.OrderIdEnd, filters.TaggedOnly);

            var reportGenerator = _reportGeneratorFactory.Create(filters.ReportType);
            var csvModels = reportGenerator.Generate(orders).Cast<dynamic>();

            try
            {
                using var stream = new MemoryStream();
                using var writer = new StreamWriter(stream);
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

                csv.WriteRecords(csvModels.ToList());
                csv.Flush();
                writer.Flush();
                
                Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
                
                return new FileContentResult(stream.ToArray(), "text/csv")
                {
                    FileDownloadName = string.IsNullOrWhiteSpace(filters.FileName)
                        ? string.Format(@"{0}_" + FileName, reportGenerator.DefaultFileNamePrefix) : filters.FileName
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return BadRequest();
            }
        }

        [HttpGet("products")]
        public async Task<List<ShopifyProductResponse>> GetProducts()
        {
            var products = await _shopifyExportService.GetProducts();
            return products.Select(p => new ShopifyProductResponse()
            {
                Id = p.Id,
                Title = p.Title
            }).ToList();
        }

        private static string FileName =>  $"{DateTime.Now:yyyyMMdd_HHmm}.csv";
    }
}