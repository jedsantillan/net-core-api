using System;
using System.Net;
using System.Threading.Tasks;
using FarmHub.Application.Services.Email.Templates;
using FarmHub.Application.Services.Infrastructure;
using FarmHub.Application.Services.Services.Interface;
using FarmHub.Data.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FarmHub.Application.Services.Services
{
    public class OrderConfirmationService : IConfirmationEmailService<Order>
    {
        private readonly IEmailService _emailService;
        private readonly IEmailMessageFactory _emailMessageFactory;
        private readonly IOptions<UrlOptions> _urlOptions;
        private readonly ILogger<OrderConfirmationService> _logger;

        public OrderConfirmationService(IEmailService emailService, IEmailMessageFactory emailMessageFactory,
            IOptions<UrlOptions> urlOptions, ILogger<OrderConfirmationService> logger)
        {
            _emailService = emailService;
            _emailMessageFactory = emailMessageFactory;
            _urlOptions = urlOptions;
            _logger = logger;
        }

        public async Task<int> SendConfirmationEmail(Order order)
        {
            var orderTemplate = new OrderConfirmationTemplate(order, _urlOptions.Value.OrderUrl);
            var sgm = _emailMessageFactory.Create(orderTemplate);
            var httpStatusCode =
                await _emailService.SendAsync(order.Customer.ContactEmail, order.Customer.FirstName, sgm);
            return httpStatusCode != HttpStatusCode.Accepted ? 0 : 1;
        }
    }
}