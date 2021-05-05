using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FarmHub.Data.Models
{
    public class ShippingAddress : ModelBase
    {
        [MaxLength(300)]
        public string Address { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        [MaxLength(100)]
        public string Province { get; set; }

        [MaxLength(250)]
        public string Landmark { get; set; }

        public bool IsDefault { get; set; }

        public int CustomerId { get; set; }

        public Customer Customer { get; set; }

        public ICollection<Order> Orders { get; set; }

    }



}
