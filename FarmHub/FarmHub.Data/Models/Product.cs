using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace FarmHub.Data.Models
{
    public class Product : ModelBase
    {

        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(15)]
        public string SKU { get; set; }
        [MaxLength(150)]
        public string? About { get; set; }
        
        [NotMapped]
        public Image? MainImage => Images.FirstOrDefault();
        
        public int? UnitOfMeasureId { get; set; }
        public UnitOfMeasure UnitOfMeasure { get; set; }
        public Category Category { get; set; }
        public int? CategoryId { get; set; }
        public Discount? Discount { get; set; }
        public int? DiscountId { get; set; }
        
        
        public virtual ICollection<Image> Images { get; set; } = new List<Image>();
        public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
        public virtual ICollection<ProductPortion> ProductPortions { get; set; } = new List<ProductPortion>();
        public virtual ICollection<BasketProduct> BasketProducts { get; set; } = new List<BasketProduct>();
        public virtual ICollection<Tag>? ProductTags { get; set; } = new List<Tag>();
        
        public Product(string name, string sku)
        {
            Name = name;
            SKU = sku;
        }

        public Product()
        {

        }

    }
}