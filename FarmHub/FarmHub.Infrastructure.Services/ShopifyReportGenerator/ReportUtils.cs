using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FarmHub.Domain.Services.ShopifyReportGenerator
{
    public static class ReportUtils
    {
        internal static Dictionary<OrderNamePair, OrderModel> BuildOrderDictionary(
            IEnumerable<ShopifySharp.Order> orders)
        {
            return orders.ToDictionary(o => new OrderNamePair
                {
                    OrderNumber = o.OrderNumber,
                    Name = o.Customer == null ||
                           string.IsNullOrWhiteSpace($"{o.Customer.FirstName} {o.Customer.LastName}")
                        ? o.ShippingAddress?.Name
                        : $"{o.Customer.FirstName} {o.Customer.LastName}",
                    Tags = o.Tags,
                    Vendor = new string[] {"Mayani Farm", "Gulay Ng Bayan"}.All(s =>
                        o.LineItems.Select(l => l.Vendor).Contains(s))
                        ? "Mixed"
                        : o.LineItems.FirstOrDefault()?.Vendor,
                    Order = o,
                },
                o => new OrderModel
                {
                    OrderData = o,
                    Orders = o.LineItems.GroupBy(l => l.Vendor + " - " + l.Title)
                        .ToDictionary(g => g.Key, g =>
                            g.GroupBy(l => ConvertToUom(l).Unit).ToDictionary(
                                g => g.Key, g => ComputeTotal(g.Select(ConvertToUom))))
                });
        }


        internal static ShopifyUnitOfMeasure ComputeTotal(IEnumerable<ShopifyUnitOfMeasure> UoMs)
        {
            var uomTotal = new ShopifyUnitOfMeasure();
            foreach (var unitOfMeasure in UoMs)
            {
                uomTotal.Amount += unitOfMeasure.Amount;
                uomTotal.Unit = unitOfMeasure.Unit;
            }

            return uomTotal;
        }

        internal static ShopifyUnitOfMeasure ConvertToUom(ShopifySharp.LineItem lineItem)
        {
            var amount = lineItem.Quantity;
            var denomination = lineItem.VariantTitle;

            //// if-else for bypass
            //if (lineItem.Name == "Barako Blend coffee (grounded)")
            //{ denomination = "1 pack"; }
            //else
            //{
            //    if (amount == null || denomination == null)
            //    {
            //        throw new Exception("Amount or denomination should not be null");
            //    }
            //}

            if (amount == null || denomination == null)
            {
                // throw new Exception("Amount or denomination should not be null");
                return new ShopifyUnitOfMeasure
                {
                    Amount = (float) amount,
                    Unit = "pc"
                };
            }

            const string separateRegex = @"^(?<amount>\d[\/\d]*)\.?\s+(?<unit>.+$)";
            var match = Regex.Match(denomination, separateRegex);

            if (!match.Success)
            {
                return new ShopifyUnitOfMeasure
                {
                    Amount = (float) amount,
                    Unit = denomination
                };
            }

            var capturedAmount = match.Groups["amount"]?.Captures.FirstOrDefault()?.Value;
            var unit = match.Groups["unit"]?.Captures.FirstOrDefault()?.Value;
            return new ShopifyUnitOfMeasure
            {
                Amount = ConvertFractionToNumeric(capturedAmount) * (amount ?? 1),
                Unit = unit
            };
        }

        internal static float ConvertFractionToNumeric(string amount)
        {
            var split = amount.Split(@"/");

            if (split.Length > 1)
            {
                return float.Parse(split[0]) / float.Parse(split[1]);
            }

            return float.Parse(split[0]);
        }

        public static Dictionary<string, Dictionary<string, float>> ComputeTotalForOrderDictionary(Dictionary<OrderNamePair, OrderModel> dict)
        {
            return dict.Values.SelectMany(v => v.Orders)
                         .GroupBy(o => o.Key)
                         .ToDictionary(g => g.Key, g =>
                             g.SelectMany(m => m.Value.Values)
                                 .GroupBy(c => c.Unit)
                                 .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount)));
        }
    }
}