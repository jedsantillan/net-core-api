using System;

namespace FarmHub.API.Models
{
    public class UserRegistrationRequestCreateModel
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public short Gender { get; set; }
        public DateTime Birthday { get; set; }
        public string RecaptchaToken { get; set; }
    }
}