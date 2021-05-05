using System.ComponentModel.DataAnnotations;

namespace FarmHub.API.Models
{
    public class CustomerCreateRequest
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string Gender { get; set; }

        [MaxLength(20)]
        public string ContactNumber { get; set; }

        [MaxLength(254)]
        public string ContactEmail { get; set; }

        public bool EmailIsConfirmed { get; set; }
    }

    public class CustomerUpdateRequest
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? UserName { get; set; }

        public string? Gender { get; set; }

        [MaxLength(20)]
        public string? ContactNumber { get; set; }

        [MaxLength(254)]
        public string? ContactEmail { get; set; }

        public bool? EmailIsConfirmed { get; set; }
    }
}
