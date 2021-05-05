using System;

namespace FarmHub.API.Models
{
    public class DiscountViewResponse
    {
        public int Id { get; set; }
        public double DiscountValue { get; set; }
        public string DiscountType { get; set; }
        public string? ProductName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
