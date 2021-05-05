using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FarmHub.Data.Models
{
    public class Category : ModelBase
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();

        public Category(string name)
        {
            Name = name;
        }

        public Category()
        {
        }
    }
}