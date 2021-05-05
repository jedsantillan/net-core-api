using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace FarmHub.Data.Models
{
    public class AuthUser : IdentityUser<int>
    {
        public virtual ICollection<Customer> Customers { get; set; }
    }
}