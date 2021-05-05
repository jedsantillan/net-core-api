using FarmHub.Data.Models;
using System.Collections.Generic;

namespace FarmHub.API.Models
{
    public class OrderViewResponse
    {
        public int Id { get; set; }
        public double TotalPrice { get; set; }
        public double? DiscountedPrice { get; set; }
        
        public OrderStatus Status { get; set; }
        public PaymentType PaymentType { get; set; }
        public PaymentStatus PaymentStatus { get; set; }

        public int? DiscountId { get; set; }
        public DiscountViewResponse? Discount { get; set; }
        public int HarvestPeriodId { get; set; }
        public HarvestPeriod? HarvestPeriod { get; set; }
        public int ShippingAddressId { get; set; }
        public ShippingAddressViewResponse ShippingAddress { get; set; }
        public int CustomerId { get; set; }
        public CustomerViewResponse Customer { get; set; }

        public List<OrderItemViewResponse> OrderItems { get; set; }
    }
}
