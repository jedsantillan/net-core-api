using System;

namespace FarmHub.Data.Models
{
    public class ConfirmationEmail : ModelBase
    {
        public string Email { get; set; }
        public Guid Guid { get; set; }
        public DateTime Expiration { get; set; }
        public ConfirmationEmailType Type { get; set; }
        public bool IsActive { get; set; }
    }

    public enum ConfirmationEmailType
    {
        AccountCreation,
        ForgotPassword
    }
}