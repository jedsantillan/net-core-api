using FarmHub.Application.Services.Infrastructure;
using Newtonsoft.Json;

namespace FarmHub.Application.Services.Email.Templates
{
    public class AccountConfirmationTemplate : IEmailTemplate<AccountConfirmationEmail>
    {

        public AccountConfirmationTemplate()
        {
        }

        public string TemplateName => "AccountConfirmation";
        public AccountConfirmationEmail TemplateData { get; set; }
    }

    public class AccountConfirmationEmail
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}