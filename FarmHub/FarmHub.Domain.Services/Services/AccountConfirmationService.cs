using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FarmHub.Application.Services.Email.Templates;
using FarmHub.Application.Services.Infrastructure;
using FarmHub.Application.Services.Repositories;
using FarmHub.Application.Services.Services.Interface;
using FarmHub.Data.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FarmHub.Application.Services.Services
{

    public class AccountConfirmationService : IConfirmationEmailService<Customer>
    {
        private readonly IEmailService _emailService;
        private readonly IEmailMessageFactory _emailMessageFactory;
        private readonly IConfirmationService _confirmationService;
        private readonly IOptions<UrlOptions> _urlOptions;
        private readonly ILogger<AccountConfirmationService> _logger;
        private readonly double _daysValid = 3;

        public AccountConfirmationService(IEmailService emailService, IEmailMessageFactory emailMessageFactory, IConfirmationService confirmationService, IOptions<UrlOptions> urlOptions, ILogger<AccountConfirmationService> logger) 
        {
            _emailService = emailService;
            _emailMessageFactory = emailMessageFactory;
            _logger = logger;
            _confirmationService = confirmationService;
            _urlOptions = urlOptions;
        }

        public async Task<int> SendConfirmationEmail(Customer customer)
        {
            var guid = Guid.NewGuid();
            var confirmationEndpointLink = $"{_urlOptions.Value.ConfirmationUrl}/{guid}";
            
            var template = new AccountConfirmationTemplate
            {
                TemplateData = new AccountConfirmationEmail()
                {
                    Name = customer.FirstName,
                    Url = confirmationEndpointLink
                }
            };

            try
            {
                var existingConfirmationEmails = await _confirmationService.FilterAsync(c => c.Email == customer.ContactEmail);
                var existing = existingConfirmationEmails.FirstOrDefault();

                if (existing != null)
                {
                    existing.IsActive = false;
                    await _confirmationService.Update(existing);
                }
                
                var confirmation = new ConfirmationEmail()
                {
                    Email = customer.ContactEmail,
                    Expiration = DateTime.Now.AddDays(_daysValid),
                    Guid = guid,
                    Type = ConfirmationEmailType.AccountCreation,
                    IsActive = true
                };

                await _confirmationService.InsertAsync(confirmation);
                var message = _emailMessageFactory.Create(template);
                var httpStatusCode = await _emailService.SendAsync(customer.ContactEmail, customer.FirstName, message);
                return httpStatusCode != HttpStatusCode.Accepted ? 0 : confirmation.Id;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }

    public class UrlOptions
    {
        public string ConfirmationUrl { get; set; }
        public string OrderUrl { get; set; }
    }
}