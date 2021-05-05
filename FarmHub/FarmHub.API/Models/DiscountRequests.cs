using FarmHub.Data.Models;
using System;

namespace FarmHub.API.Models
{
    public class DiscountCreateRequest
    {
        public double DiscountValue { get; set; }
        public DiscountTypeEnum DiscountType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public DiscountCreateRequest(double discountValue, DiscountTypeEnum discountType)
        {
            DiscountValue = discountValue;
            DiscountType = discountType;
        }

        public DiscountCreateRequest()
        {

        }
    }

    public class DiscountUpdateRequest 
    {
        public int Id { get; set; }
        public double DiscountValue { get; set; }
        public DiscountTypeEnum DiscountType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

    }


}
