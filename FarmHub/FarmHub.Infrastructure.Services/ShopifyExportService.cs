using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FarmHub.Application.Services.Infrastructure;
using Newtonsoft.Json;
using ShopifySharp;
using ShopifySharp.Filters;

namespace FarmHub.Domain.Services
{
    public class MayaniOrderFilter : OrderListFilter
    {
        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("page_info")] public string PageInfo { get; set; }
    }

    public class ShopifyExportService : IShopifyExportService
    {
        private ShopifySharp.OrderService _orderService;
        private ShopifySharp.ProductService _productService;

        public ShopifyExportService(ShopifySharp.OrderService orderService, ShopifySharp.ProductService productService)
        {
            _orderService = orderService;
            _productService = productService;
        }

        public async Task<IEnumerable<Order>> GetOrdersWithFilter(DateTime filtersDateStart,
            DateTime filtersDateEnd, IEnumerable<string> filtersTags,
            IEnumerable<long?> filtersProductIds, IEnumerable<string> filtersPromoCodes, int? orderNumberStart,
            int? orderNumberEnd, bool? returnTaggedOnly = true)
        {
            const int limit = 250;
            const string fulfillmentStatus = "unfulfilled";
            const string status = "open";
            var orders = new List<Order>();

            var pageOfOrders = await _orderService.ListAsync(new MayaniOrderFilter()
            {
                Limit = limit,
                FulfillmentStatus = fulfillmentStatus,
                Status = status,
                CreatedAtMin = new DateTimeOffset(filtersDateStart),
                CreatedAtMax = new DateTimeOffset(filtersDateEnd),
            }, CancellationToken.None);

            orders.AddRange(pageOfOrders.Items);

            while (pageOfOrders.HasNextPage)
            {
                var filter = pageOfOrders.GetNextPageFilter();
                pageOfOrders = await _orderService.ListAsync(filter);

                orders.AddRange(pageOfOrders.Items);
            }

            orders = orders.OrderByDescending(o => o.Tags?.Length).ThenBy(o => o.OrderNumber).ToList();

            orders.ForEach(o => { o.LineItems = o.LineItems.Where(l => l.FulfillableQuantity > 0); });

            // filter those which has pineapples
            //orders = orders.Where(o => o.LineItems.Any(l => l.Name.ToUpper().Contains("PINEAPPLE"))).ToList();

            if (filtersProductIds != null && filtersProductIds.Any())
                orders = orders.Where(o => o.LineItems.Any(l => filtersProductIds.Contains(l.ProductId))).ToList();


            Func<Order, bool> customFilter;
            if (returnTaggedOnly != null && returnTaggedOnly.Value && filtersTags != null && filtersTags.Any())
            {
                customFilter = o => (orderNumberEnd == null || o.OrderNumber >= orderNumberStart) &&
                                    (orderNumberEnd == null || o.OrderNumber <= orderNumberEnd) &&
                                    o.Tags != null && filtersTags.Any() &&
                                    filtersTags.Intersect(o.Tags.Split(",")).Any();
            }
            else
            {
                customFilter = o =>
                    (orderNumberEnd == null || o.OrderNumber >= orderNumberStart) &&
                    (orderNumberEnd == null || o.OrderNumber <= orderNumberEnd) || filtersTags != null &&
                    o.Tags != null && filtersTags.Any() && filtersTags.Intersect(o.Tags.Split(",")).Any();
            }

            // define what to reject. Can we remove this? TODO: Come up with a better way of implementing this
            // var rejectList =
            //     orders.Where(o => o.LineItems != null && o.LineItems.Any(l =>l.Name?.ToUpper().Contains("PINEAPPLE")) && 
            //                       !(o.OrderNumber == 5598 || o.OrderNumber == 5599 || o.OrderNumber == 5604 || o.OrderNumber == 5605)).ToList();
            // replace 'filteredOrders' to 'orders' in BuildOrderDictionary method

            return orders.Where(customFilter).ToList();
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            const int limit = 50;
            var products = new List<Product>();

            var pageOfProducts = await _productService.ListAsync(new ProductListFilter()
            {
                Limit = limit
            }, CancellationToken.None);

            products.AddRange(pageOfProducts.Items);

            while (pageOfProducts.HasNextPage)
            {
                var filter = pageOfProducts.GetNextPageFilter();
                pageOfProducts = await _productService.ListAsync(filter);

                products.AddRange(pageOfProducts.Items);
            }

            return products;
        }
    }
}