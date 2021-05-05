using System.Collections.Generic;

namespace FarmHub.Application.Services.Email
{
    public class SendGridOptions
    {
        public Dictionary<string, string> Templates { get; set; }
    }
}