using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace FarmHub.API.Models
{
    public class ProductCreateRequestForm
    {
        public IEnumerable<IFormFile> Images { get; set; }
        public ProductCreateRequest Product { get; set; }
    }
    public class ProductCreateRequest
    {
        public string Name { get; set; }
        public string SKU { get; set; }
        public string? About { get; set; }
        public int? UnitOfMeasureId { get; set; }
        public float OrderPortion { get; set; }
        public int? CategoryId { get; set; }
        public int[] PortionIds { get; set; }
        public string[] Tags { get; set; }
        public ProductCreateRequest(string name, string sku)
        {
            Name = name;
            SKU = sku;
        }

        public ProductCreateRequest()
        {
        }
    }

    public class ProductUpdateRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SKU { get; set; }
        public string? About { get; set; }

        public int? ImageId { get; set; }
        public int? UnitOfMeasureId { get; set; }
        public int? CategoryId { get; set; }
        public string[] Tags { get; set; }

        public int[]? PortionIds { get; set; }
        public ProductPortionUpdateRequest[]? PortionPrice { get; set; }
        public int? DiscountId { get; set; }
    }

    public class ProductPriceUpdateRequest
    {
        public int PortionId { get; set; }
        public double Price { get; set; }
    }
}