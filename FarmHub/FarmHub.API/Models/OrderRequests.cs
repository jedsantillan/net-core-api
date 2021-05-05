using FarmHub.Data.Models;

namespace FarmHub.API.Models
{
    public class OrderCreateRequest
    {
        public double TotalPrice { get; set; }
        public double? DiscountedPrice { get; set; }
        public int CustomerId { get; set; }
        public int? DiscountId { get; set; }
        public int HarvestPeriodId { get; set; }
        public int ShippingAddressId { get; set; }
        
        public OrderStatus Status { get; set; }

        public OrderItemCreateRequest[] OrderItems { get; set; }

        public OrderCreateRequest()
        {

        }
    }

    public class OrderUpdateRequest
    {
        public double? TotalPrice { get; set; }
        public double? DiscountedPrice { get; set; }
        public int CustomerId { get; set; }
        public int? DiscountId { get; set; }
        public int? HarvestPeriodId { get; set; }
        public int? ShippingAddressId { get; set; }

        public OrderStatus Status { get; set; }
        public OrderItemCreateRequest[] OrderItems { get; set; }



        public OrderUpdateRequest()
        {

        }
    }
}
