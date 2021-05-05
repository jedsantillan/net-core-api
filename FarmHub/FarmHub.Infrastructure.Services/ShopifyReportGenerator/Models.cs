using System.Collections.Generic;
using Newtonsoft.Json;
using ShopifySharp.Filters;

namespace FarmHub.Domain.Services.ShopifyReportGenerator
{
    public class ShopifyUnitOfMeasure
    {
        public float Amount { get; set; }
        public string Unit { get; set; }
    }

    public class OrderNamePair
    {
        public int? OrderNumber { get; set; }
        public string Name { get; set; }
        public string Tags { get; set; }
        public string Vendor { get; set; }
        public ShopifySharp.Customer Customer { get; set; }
        public ShopifySharp.Order Order { get; set; }
    }

    public class OrderModel
    {
        public Dictionary<string, Dictionary<string, ShopifyUnitOfMeasure>> Orders { get; set; }
        public ShopifySharp.Order OrderData { get; set; }
    }
}