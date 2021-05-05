using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using ShopifySharp;

namespace FarmHub.Domain.Services.ShopifyReportGenerator
{
    public class RidersCommReportGenerator : IReportGenerator<ExpandoObject>
    {
        public string DefaultFileNamePrefix => "riders";

        public IEnumerable<ExpandoObject> Generate(IEnumerable<Order> orders)
        {
            return ConvertToExpando(orders);
        }

        private IEnumerable<ExpandoObject> ConvertToExpando(IEnumerable<Order> orders)
        {
            var expandos = new List<ExpandoObject>();
            foreach (var order in orders)
            {
                var eo = new ExpandoObject() as IDictionary<string, object>;
                eo.Add("OrderNumber", order.OrderNumber);
                
                var name = order.Customer == null ||
                           string.IsNullOrWhiteSpace($"{order.Customer.FirstName} {order.Customer.LastName}")
                    ? order.ShippingAddress?.Name ?? "No Name"
                    : $"{order.Customer.FirstName} {order.Customer.LastName}";
                
                eo.Add("Customer", name);
                
                eo.Add("PaymentStatus", order.FinancialStatus);
                eo.Add("Total", order.TotalPrice);
                eo.Add("DiscountCodes", order.DiscountCodes.FirstOrDefault()?.Code);
                eo.Add("Tags", order.Tags);
                
                eo.Add("ContactNo", order.Phone);
                eo.Add("Email", order.Email);
                
                var address = (order.ShippingAddress ?? order.BillingAddress) ??
                             order.Customer?.Addresses.FirstOrDefault();
                
                eo.Add("Address", address != null ? $"{address.Company} {address.Address1} {address.Address2} " : "NO ADDRESS");
                eo.Add("City", address?.City);
                expandos.Add((ExpandoObject) eo);
            }
            return expandos;
        }
    }
}