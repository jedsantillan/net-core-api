using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FarmHub.Application.Services.Infrastructure;
using FarmHub.Data.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace FarmHub.Application.Services.Email.Templates
{
    public class OrderConfirmationTemplate : IEmailTemplate<OrderConfirmationEmail>
    {
        private readonly string _dateFormat = "MMMM:DD:YYYY";
        private readonly string _timezoneOffset = "+0800";
        
        //TODO Andrei: Test correctness of templates
        public OrderConfirmationTemplate(Order order, string valueOrderUrl)
        {
            TemplateData = new OrderConfirmationEmail()
            {
                CustomerName = order.Customer.FirstName,
                OrderNo = order.Id,
                OrderUrl = string.Format(valueOrderUrl, order.Id),
                Total = order.TotalPrice,
                OrderDate = order.CreatedDate,
                Orders = order.OrderItems.Select(i => new OrderModel()
                {
                    Product = i.ProductPortion.Product.Name,
                    Quantity = i.Quantity,
                    Portion = i.ProductPortion.Portion.DisplayName,
                    Unit = i.ProductPortion.Product.UnitOfMeasure.ShortName,
                    Price = i.ProductPortion.Price * i.Quantity
                }).ToArray(),
            };
        }

        public string TemplateName => "OrderConfirmation";
        public OrderConfirmationEmail TemplateData { get; set; }
    }

    public class OrderConfirmationEmail
    {
        [JsonProperty("customerName")]
        public string CustomerName { get; set; }

        [JsonProperty("orderUrl")]
        public string OrderUrl { get; set; }
        
        [JsonProperty("orderNo")]
        public int OrderNo { get; set; }
        
        [JsonProperty("total")]
        public double Total { get; set; }
        
        [JsonProperty("orderDate")]
        public DateTime OrderDate { get; set; }
        
        [JsonProperty("orders")]
        public OrderModel[] Orders { get; set; }
    }

    public class OrderModel
    {
        [JsonProperty("product")]
        public string Product { get; set; }
        [JsonProperty("unit")]
        public string Unit { get; set; }
        [JsonProperty("quantity")]
        public int Quantity { get; set; }
        [JsonProperty("portion")]
        public string Portion { get; set; }
        [JsonProperty("price")]
        public double Price { get; set; }
    }
}