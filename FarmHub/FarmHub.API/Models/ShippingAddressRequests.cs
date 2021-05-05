namespace FarmHub.API.Models
{
    public class ShippingAddressCreateRequest
    {
        public string Address { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Landmark { get; set; }
        public bool IsDefault { get; set; }
        public int CustomerId { get; set; }


        public ShippingAddressCreateRequest()
        {

        }
    }

    public class ShippingAddressUpdateRequest
    {
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }
        public string? Landmark { get; set; }
        public bool? IsDefault { get; set; }
        public int? CustomerId { get; set; }


        public ShippingAddressUpdateRequest()
        {

        }
    }
}
