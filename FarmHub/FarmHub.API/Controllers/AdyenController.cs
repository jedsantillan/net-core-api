using System;
using System.Threading.Tasks;
using Adyen.Model.Checkout;
using AutoMapper;
using FarmHub.API.Models;
using FarmHub.Application.Services.Infrastructure;
using FarmHub.Application.Services.Repositories;
using FarmHub.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FarmHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdyenController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<AdyenController> _logger;
        private readonly IOrderService _orderService;
        private readonly IAdyenPaymentService _adyenPaymentService;

        public AdyenController(IAdyenPaymentService adyenPaymentService,
            IOrderService orderService,
            IMapper mapper,
            ILogger<AdyenController> logger)
        {
            _adyenPaymentService = adyenPaymentService;
            _orderService = orderService;
            _mapper = mapper;
            _logger = logger;
        }

        // POST: api/adyen/paymentMethods
        [HttpPost("paymentMethods")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPaymentMethods(AdyenGetPaymentMethodsRequest request)
        {
            try
            {
                var paymentMethodsResponse = await _adyenPaymentService.PaymentMethodsAsync(request.CountryCode,
                    request.Locale, request.Currency, request.Amount, request.Channel);
                return Ok(paymentMethodsResponse);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        [HttpPost("payments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SubmitOneTimePayment(AdyenSubmitPaymentOneTimeRequest request)
        {
            var details = _mapper.Map<MayaniPaymentMethod, DefaultPaymentMethodDetails>(request.PaymentMethod);

            try
            {
                var paymentStatus = PaymentStatus.Pending;
                var paymentsResponse = await _adyenPaymentService.PaymentsAsync(request.Currency,
                    request.Amount,
                    request.OrderId,
                    request.ReturnUrl,
                    request.PaymentMethod,
                    details,
                    request.BrowserInfo,
                    request.UseGetForRedirectFromIssuerMethod);

                paymentStatus = AdyenResultCodeToMayaniResultCode(paymentsResponse.ResultCode);

                await _orderService.UpdatePaymentStatusAsync(request.OrderId,
                    paymentStatus,
                    paymentsResponse.PaymentData,
                    paymentsResponse.PspReference,
                    paymentsResponse.RefusalReason);

                return Ok(paymentsResponse);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        [HttpPost("details")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SubmitAdditionalDetails(AdyenSubmitAdditionalDetailsRequest request)
        {
            var order = await _orderService.GetByIdAsync(request.OrderId);
            var paymentData = order?.LastCardPaymentRecord?.PaymentData;

            if (string.IsNullOrEmpty(paymentData))
            {
                return BadRequest();
            }

            try
            {
                var paymentsResponse =
                    await _adyenPaymentService.PaymentDetailsAsync(request.MD, request.PaRes, paymentData);
                var paymentStatus = AdyenResultCodeToMayaniResultCode(paymentsResponse.ResultCode);

                await _orderService.UpdatePaymentStatusAsync(order.Id, paymentStatus,
                    paymentsResponse.PaymentData,
                    paymentsResponse.PspReference,
                    paymentsResponse.RefusalReason);

                return Ok(paymentsResponse);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        private static PaymentStatus AdyenResultCodeToMayaniResultCode(PaymentResponse.ResultCodeEnum? resultCodeEnum)
        {
            var paymentStatus = resultCodeEnum switch
            {
                PaymentResponse.ResultCodeEnum.Authorised => PaymentStatus.Success,
                PaymentResponse.ResultCodeEnum.Refused => PaymentStatus.Failed,
                _ => PaymentStatus.Pending
            };

            return paymentStatus;
        }
    }
}