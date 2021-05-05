using System.Collections.Generic;
using System.Threading.Tasks;
using Adyen;
using Adyen.Model.Checkout;
using Adyen.Service;
using FarmHub.Application.Services.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace FarmHub.Domain.Services
{
    public class AdyenPaymentService : IAdyenPaymentService
    {
        private readonly Checkout _checkout;
        private readonly string _merchantAccount;
        private readonly ILogger<AdyenPaymentService> _logger;

        public AdyenPaymentService(Client adyenClient,
            IOptions<AdyenConfig> adyenConfig,
            ILogger<AdyenPaymentService> logger)
        {
            _checkout = new Checkout(adyenClient);
            _merchantAccount =  adyenConfig.Value.MerchantAccount;
            _logger = logger;
            _logger.LogInformation($"Using Adyen Config: {JsonConvert.SerializeObject( adyenConfig.Value)}");
            
        }
        public async Task<PaymentMethodsResponse> PaymentMethodsAsync(string countryCode, string locale, string currency, 
            long amountValue, PaymentMethodsRequest.ChannelEnum? channel)
        {
            var amount = new Amount(currency, amountValue);

            var paymentMethodsRequest = new PaymentMethodsRequest(merchantAccount: _merchantAccount)
            {
                CountryCode = countryCode,
                ShopperLocale = locale,
                Amount = amount,
                Channel = channel
            };
            return await _checkout.PaymentMethodsAsync(paymentMethodsRequest);
        }

        public async Task<PaymentResponse> PaymentsAsync(string currency, long amountValue, int orderId, string returnUrl,
            MayaniPaymentMethod paymentMethod, DefaultPaymentMethodDetails details, BrowserInfo browserInfo, bool useGetForRedirectFromIssuerMethod)
        {
            var amount = new Amount(currency, amountValue);
            var paymentsRequest = new PaymentRequest
            {
                Reference = orderId.ToString(),
                Amount = amount,
                ReturnUrl = returnUrl,
                MerchantAccount = _merchantAccount,
                PaymentMethod = details,
                BrowserInfo = browserInfo,
                RedirectFromIssuerMethod = useGetForRedirectFromIssuerMethod ? "GET" : "POST"
            };
            
            return await _checkout.PaymentsAsync(paymentsRequest);
        }

        public async Task<PaymentResponse> PaymentDetailsAsync(string requestMD, string requestPaRes, string paymentData)
        {
            var pcl = new PaymentCompletionDetails
            {
                MD = requestMD,
                PaRes = requestPaRes
            };
            var paymentsDetailsRequest =
                new PaymentsDetailsRequest(pcl, paymentData);

            _logger.LogInformation("Endpoint {0}. Calling {1} with parameters: `{2}`", "SubmitAdditionalDetails",
                "PaymentDetailsAsync", JsonConvert.SerializeObject(paymentsDetailsRequest));
            
            return await _checkout.PaymentDetailsAsync(paymentsDetailsRequest);
        }
    }
}