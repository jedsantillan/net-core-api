using System;
using System.ComponentModel.DataAnnotations;

namespace FarmHub.Data.Models
{
    public class Farmer : ModelBase
    {

        [MaxLength(100)]
        public string FirstName { get; set; }

        [MaxLength(100)]
        public string LastName { get; set; }

        public DateTime Birthday { get; set; }

        [MaxLength(1)]
        public string Gender { get; set; }

        [MaxLength(255)]
        public string Address { get; set; }
        
        [MaxLength(20)]
        public string ContactNumber { get; set; }

        [MaxLength(254)]
        public string ContactEmail { get; set; }

        public bool EmailIsConfirmed { get; set; }

        public string? ImageUrl { get; set; }

        public int? FarmerAssociationId { get; set; }
        public FarmerAssociation? FarmerAssociation { get; set; }
    }
}