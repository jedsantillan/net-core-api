using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using ShopifySharp;

namespace FarmHub.Domain.Services.ShopifyReportGenerator
{
    public class MasterListReportGenerator : IReportGenerator<ExpandoObject>
    {
        public string DefaultFileNamePrefix => "masterlist";

        public IEnumerable<ExpandoObject> Generate(IEnumerable<Order> orders)
        {
            var orderDictionary = ReportUtils.BuildOrderDictionary(orders);
            var headers = orderDictionary.Values.SelectMany(v => v.Orders.Select(o => o.Key)).Distinct().OrderBy(h => h);
            return ConvertToExpando(orderDictionary, headers);
        }

        private IEnumerable<ExpandoObject> ConvertToExpando(Dictionary<OrderNamePair, OrderModel> dict,
            IEnumerable<string> headers)
        {
            var expandos = new List<ExpandoObject>();
            foreach (var key in dict.Keys)
            {
                var address = (key.Order.ShippingAddress ?? key.Order.BillingAddress) ??
                              key.Customer?.Addresses.FirstOrDefault();
                var email = key.Order.Email;
                var contact = key.Order.Phone;

                var x = new ExpandoObject() as IDictionary<string, Object>;
                x.Add("OrderNumber", key.OrderNumber);
                x.Add("OrderDate", key.Order.CreatedAt.ToString());
                x.Add("Name", key.Name);
                x.Add("Address",
                    address != null
                        ? $"{address.Company} {address.Address1} {address.Address2} {address.City}"
                        : "NO ADDRESS");
                x.Add("Email", email);
                x.Add("ContactNo", contact);
                x.Add("Tags", key.Tags);

                foreach (var header in headers)
                {
                    var value = "";
                    if (dict[key].Orders.TryGetValue(header, out var unitDictionary))
                    {
                        value = string.Join("\n", unitDictionary.Select(d => $"{d.Value.Amount} {d.Key}"));
                    }

                    x.Add(header, value);
                }

                x.Add("Note", dict[key].OrderData?.Note);
                x.Add("CustomerNote", dict[key].OrderData?.Customer?.Note);
                x.Add("CustomerId", dict[key].OrderData?.Customer?.Id);
                expandos.Add((ExpandoObject) x);
            }

            var totalRow = new ExpandoObject() as IDictionary<string, object>;
            totalRow.Add("OrderNumber", "END");
            totalRow.Add("Tags", "");
            totalRow.Add("Name", "TOTAL");
            totalRow.Add("Address", "");
            totalRow.Add("Email", "");
            totalRow.Add("ContactNo", "");

            var totalUnitOfMeasures = ReportUtils.ComputeTotalForOrderDictionary(dict);
            
            foreach (var header in headers)
            {
                if (!totalUnitOfMeasures.TryGetValue(header, out var unitsValue)) continue;
                var cellValue = string.Join("\n", unitsValue.Select(u => $"{u.Value} {u.Key}"));
                totalRow.Add(header, cellValue);
            }

            expandos.Add((ExpandoObject) totalRow);
            return expandos;
        }
    }
}