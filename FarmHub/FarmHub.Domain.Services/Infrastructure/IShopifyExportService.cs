using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShopifySharp;

namespace FarmHub.Application.Services.Infrastructure
{
    public interface IShopifyExportService
    {
        Task<IEnumerable<Order>> GetOrdersWithFilter(DateTime filtersDateStart, DateTime filtersDateEnd,
            IEnumerable<string> filtersTags, IEnumerable<long?> filtersProductIds, IEnumerable<string> filtersPromoCodes, int? orderNumberStart, int? orderNumberEnd, bool? taggedOnly);

        Task<IEnumerable<Product>> GetProducts();
    }
}