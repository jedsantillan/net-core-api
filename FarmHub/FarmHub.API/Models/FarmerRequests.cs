using System;

namespace FarmHub.API.Models
{
    public class FarmerCreateRequest
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        public string ContactEmail { get; set; }
        public bool EmailIsConfirmed { get; set; }
        public string? ImageUrl { get; set; }
        public int? FarmerAssociationId { get; set; }

    }
    
    public class FarmerUpdateRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? Birthday { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? ContactNumber { get; set; }
        public string? ContactEmail { get; set; }
        public bool? EmailIsConfirmed { get; set; }
        public string? ImageUrl { get; set; }
        public int? FarmerAssociationId { get; set; }

    }
}
