using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FarmHub.Data.Models
{
    public class Customer : ModelBase
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }
        
        public DateTime Birthday { get; set; }

        public Gender Gender { get; set; }
        
        [MaxLength(20)]
        public string? ContactNumber { get; set; }

        [MaxLength(254)]
        public string ContactEmail { get; set; }

        public bool EmailIsConfirmed { get; set; }
        
        public AuthUser? AuthUser { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        
        public virtual ICollection<ShippingAddress> ShippingAddresses{ get; set; }
    }

    public enum Gender
    {
        Male = 1,
        Female,
        Other
    }
}
