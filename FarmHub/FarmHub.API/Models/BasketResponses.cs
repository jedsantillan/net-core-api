using System.Collections.Generic;

namespace FarmHub.API.Models
{
    public class BasketViewResponse
    {
        public int Id { get; set; }
        public string BasketName { get; set; }
        public string Description { get; set; }
        public List<BasketProductViewResponse> Products { get; set; }
    }

    public class BasketProductViewResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
    }
}
