using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace FarmHub.Data.Models
{
    public class OrderItem : ModelBase
    {
        public int OrderId { get; set; }

        public Order Order { get; set; }


        [JsonProperty("portionId")]
        [JsonPropertyName("portionId")]
        public int ProductPortionPortionId { get; set; }


        [JsonProperty("productId")]
        [JsonPropertyName("productId")]
        public int ProductPortionProductId { get; set; }

        public ProductPortion ProductPortion { get; set; }

        public int Quantity { get; set; }

    }
}
    