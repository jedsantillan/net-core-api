using System.Collections.Generic;

namespace FarmHub.API.Models
{
    public class ProductViewResponse 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SKU { get; set; }
        public string About { get; set; }

        public UnitOfMeasureViewResponse? UnitOfMeasure { get; set; }
        public int? CategoryId { get; set; }
        public string? Category { get; set; }
        public IList<ProductPortionViewResponse> Portions { get; set; }
        public ImageViewResponse? Image { get; set; }
        public DiscountViewResponse Discount { get; set; }
        public string[] Tags { get; set; }
    }
}
