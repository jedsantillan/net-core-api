using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmHub.Data.Models
{
    public class ProductPortion
    {
        public int ProductId { get; set; }

        public Product Product { get; set; }

        public int PortionId { get; set; }
        public Portion Portion { get; set; }    
        public double Price { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}