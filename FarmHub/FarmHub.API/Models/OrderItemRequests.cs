using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace FarmHub.API.Models
{
    public class OrderItemCreateRequest
    {
        [JsonProperty("portionId")]
        [JsonPropertyName("portionId")]
        public int ProductPortionPortionId { get; set; }
        
        [JsonProperty("productId")]
        [JsonPropertyName("productId")]
        public int ProductPortionProductId { get; set; }

        public int Quantity { get; set; }

        public OrderItemCreateRequest()
        {

        }

    }
    public class OrderItemUpdateRequest
    {
        [JsonProperty("portionId")]
        [JsonPropertyName("portionId")]
        public int? ProductPortionPortionId { get; set; }

        [JsonProperty("productId")]
        [JsonPropertyName("productId")]
        public int? ProductPortionProductId { get; set; }

        public int? Quantity { get; set; }


        public OrderItemUpdateRequest()
        {

        }
    }
}
