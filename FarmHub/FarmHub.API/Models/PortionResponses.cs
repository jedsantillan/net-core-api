namespace FarmHub.API.Models
{
    public class PortionViewResponse
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public double RealDecimalValue { get; set; }

    }

    public class ProductPortionViewResponse
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public double RealDecimalValue { get; set; }
        public double Price { get; set; }
        public double? DiscountedPrice { get; set; }
    }
}
