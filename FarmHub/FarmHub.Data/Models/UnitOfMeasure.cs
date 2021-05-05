using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FarmHub.Data.Models
{
    public class UnitOfMeasure : ModelBase
    {
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(5)]
        public string? ShortName { get; set; }
        public bool IsDecimal { get; set; } = true;

        public ICollection<Product> Products { get; set; } = new List<Product>();

        public UnitOfMeasure(string name)
        {
            Name = name;
        }

        public UnitOfMeasure()
        {

        }
    }
}