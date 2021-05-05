using System.Collections.Generic;

namespace FarmHub.Data.Models
{
    public class FarmerAssociation : ModelBase
    {
        public string Name { get; set; }
        public string Address { get; set; }
        
        public string ContactNo { get; set; }
        public string? Email { get; set; }
        
        public double? Latitude { get; set; }
        public  double? Longitude { get; set; }
        
        public virtual ICollection<Farmer> Farmers { get; set; }
        public virtual ICollection<Inventory> Inventories { get; set; }


    }
}