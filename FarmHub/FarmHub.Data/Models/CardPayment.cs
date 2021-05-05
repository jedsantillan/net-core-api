using System.ComponentModel.DataAnnotations;

namespace FarmHub.Data.Models
{
    public class CardPayment : ModelBase
    {
        [MaxLength(3000)]
        public string PaymentData { get; set; }
        [MaxLength(100)]
        public string? PaymentReference { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        [MaxLength(1000)]
        public string? RefusalReason { get; set; }
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
    }

    public enum PaymentStatus
    {
        Pending,
        Success,
        Failed
    }
}