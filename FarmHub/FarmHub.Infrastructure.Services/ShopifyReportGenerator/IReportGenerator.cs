using System.Collections.Generic;

namespace FarmHub.Domain.Services.ShopifyReportGenerator
{
    public interface IReportGenerator<out T>
    {
        string DefaultFileNamePrefix { get; }
        IEnumerable<T> Generate(IEnumerable<ShopifySharp.Order> orders);
    }
}