using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Adyen.Service;

namespace FarmHub.Data.Models
{
    public class Order : ModelBase
    {
        public Order()
        {
            CardPayments = new List<CardPayment>();
        }
        public double TotalPrice { get; set; }

        public double? DiscountedPrice { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.New;

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        public PaymentType PaymentType { get; set; }


        public int? DiscountId { get; set; }

        public int HarvestPeriodId { get; set; }

        public int ShippingAddressId { get; set; }

        public int CustomerId { get; set; }

        public Discount? Discount { get; set; }

        public HarvestPeriod HarvestPeriod { get; set; }

        public ShippingAddress ShippingAddress { get; set; }

        public Customer Customer { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }

        public virtual ICollection<CardPayment> CardPayments { get; set; }

        [NotMapped]
        public CardPayment LastCardPaymentRecord => CardPayments.OrderByDescending(p => p.CreatedDate).FirstOrDefault();
    }

    public enum OrderStatus
    {
        New,
        Acknowledged,
        Harvested,
        ScheduledForDelivery,
        DeliveryInProgress,
        Fulfilled
    }

    public enum PaymentType
    {
        Cash,
        Card
    }
}
