using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using ShopifySharp;

namespace FarmHub.Domain.Services.ShopifyReportGenerator
{
    public class TotalsReportGenerator : IReportGenerator<ExpandoObject>
    {
        public string DefaultFileNamePrefix => "totals";
        public IEnumerable<ExpandoObject> Generate(IEnumerable<Order> orders)
        {
            var orderDictionary = ReportUtils.BuildOrderDictionary(orders);
            var headers = orderDictionary.Values.SelectMany(v => v.Orders.Select(o => o.Key)).Distinct()
                .OrderBy(h => h);
            return ConvertToExpando(orderDictionary, headers);
        }

        private IEnumerable<ExpandoObject> ConvertToExpando(Dictionary<OrderNamePair, OrderModel> orderDictionary,
            IOrderedEnumerable<string> headers)
        {
            var totalUnitOfMeasures = ReportUtils.ComputeTotalForOrderDictionary(orderDictionary);

            var expandos = new List<ExpandoObject>();
            foreach (var key in totalUnitOfMeasures.Keys.OrderBy(k => k))
            {
                var eo = new ExpandoObject() as IDictionary<string, object>;

                eo.Add("Product", key);
                eo.Add("Totals", string.Join("\n", totalUnitOfMeasures[key].Select(u => $"{u.Value} {u.Key}")));
                expandos.Add((ExpandoObject) eo);
            }

            return expandos;
        }
    }
}