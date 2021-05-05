using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace FarmHub.API.Models
{
    public class OrderItemViewResponse
    {
        public int OrderId { get; set; }

        [JsonProperty("portionId")]
        [JsonPropertyName("portionId")]
        public int ProductPortionPortionId { get; set; }
        
        public string Portion { get; set; }
        public double PortionValue { get; set; }

        [JsonProperty("productId")]
        [JsonPropertyName("productId")]
        public int ProductPortionProductId { get; set; }

        public string Product { get; set; }
        public string UnitOfMeasure { get; set; }
        
        public int Quantity { get; set; }
        
        public double Total { get; set; }

    }
}
