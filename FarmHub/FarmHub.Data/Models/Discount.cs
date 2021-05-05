using System;


namespace FarmHub.Data.Models
{
    public class Discount : ModelBase
    {
        public double DiscountValue { get; set; }
        public DiscountTypeEnum DiscountType { get; set; }
        public Product? Product { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Discount(double discountValue, DiscountTypeEnum discountType)
        {
            DiscountValue = discountValue;
            DiscountType = discountType;
        }

        public Discount()
        {
        }
    }

    public enum DiscountTypeEnum
    {
        Percentage,
        FixedAmount
    }
}
