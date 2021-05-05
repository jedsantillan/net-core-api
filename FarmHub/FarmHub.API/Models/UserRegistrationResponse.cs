namespace FarmHub.API.Models
{
    public class UserRegistrationResponse
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public int Id { get; set; }
        public bool EmailConfirmed { get; set; }
    }
}