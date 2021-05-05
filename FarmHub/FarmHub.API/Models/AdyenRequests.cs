using Adyen.Model.Checkout;
using FarmHub.Application.Services.Infrastructure;

namespace FarmHub.API.Models
{
    public class AdyenGetPaymentMethodsRequest
    {
        public string Currency { get; set; }
        public long Amount { get; set; }
        public string CountryCode { get; set; }
        public string Locale { get; set; }
        public PaymentMethodsRequest.ChannelEnum? Channel { get; set; } = PaymentMethodsRequest.ChannelEnum.Web;
    }

    public class AdyenSubmitPaymentOneTimeRequest
    {
        public string Currency { get; set; }
        public long Amount { get; set; }
        public string ReturnUrl { get; set; }
        public int OrderId { get; set; }
        public bool UseGetForRedirectFromIssuerMethod { get; set; } = false;
        public BrowserInfo BrowserInfo { get; set; }
        public MayaniPaymentMethod PaymentMethod { get; set; }
    }

    public class AdyenSubmitAdditionalDetailsRequest
    {
        public int OrderId { get; set; }
        public string MD { get; set; }
        public string PaRes { get; set; }
    }
}