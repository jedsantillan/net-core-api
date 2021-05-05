namespace FarmHub.API.Models
{
    public class PortionCreateRequest
    {
        public string DisplayName { get; set; }
        public double RealDecimalValue { get; set; }

        public PortionCreateRequest(string displayName, double realDecimalValue)
        {
            DisplayName = displayName;
            RealDecimalValue = realDecimalValue;
        }

        public PortionCreateRequest()
        {
            
        }
    }
    
    public class PortionUpdateRequest
    {
        public string? DisplayName { get; set; }
        public double? RealDecimalValue { get; set; }
    }


    public class ProductPortionUpdateRequest
    {
        public int PortionId { get; set; }
        public double? Price { get; set; }
    }
}