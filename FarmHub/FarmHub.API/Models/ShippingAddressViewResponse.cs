namespace FarmHub.API.Models
{
    public class ShippingAddressViewResponse
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Landmark { get; set; }
        public bool IsDefault { get; set; }
        public int CustomerId { get; set; }
    }

}
