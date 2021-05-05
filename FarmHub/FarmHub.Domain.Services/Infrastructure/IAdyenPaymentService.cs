using System.Threading.Tasks;
using Adyen.Model.Checkout;

namespace FarmHub.Application.Services.Infrastructure
{
    public interface IAdyenPaymentService
    {
        Task<PaymentMethodsResponse> PaymentMethodsAsync(string paymentMethodsRequest, string locale, string currency, long amount, PaymentMethodsRequest.ChannelEnum? channel);
        Task<PaymentResponse> PaymentsAsync(string currency, long amount, int orderId,
            string returnUrl, MayaniPaymentMethod paymentMethod, DefaultPaymentMethodDetails paymentMethodDetails,
            BrowserInfo browserInfo, bool useGetForRedirectFromIssuerMethod);
        Task<PaymentResponse> PaymentDetailsAsync(string requestMd, string requestPaRes, string paymentData);
    }
    

    public class MayaniPaymentMethod
    {
        public string Type { get; set; }
        public string EncryptedCardNumber { get; set; }
        public string EncryptedExpiryMonth { get; set; }
        public string EncryptedExpiryYear { get; set; }
        public string EncryptedSecurityCode { get; set; }
        public string HolderName { get; set; }
    }
}