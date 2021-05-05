using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FarmHub.Data.Models
{
    public class Basket : ModelBase
    {
        [MaxLength(100)]
        public string BasketName { get; set; }
        [MaxLength(300)]
        public string? Description { get; set; }
        public virtual ICollection<BasketProduct> BasketProducts { get; set; } = new List<BasketProduct>();
    }
}
