using System;
using System.Linq;
using System.Threading.Tasks;
using FarmHub.Application.Services.Repositories;
using FarmHub.Application.Services.Services;
using FarmHub.Application.Services.Services.Interface;
using FarmHub.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FarmHub.API.Controllers
{
    [Route("api/[controller]")]
    public class ConfirmationController : ControllerBase
    {
        private readonly IConfirmationService _confirmationService;
        private readonly ICustomerService _customerService;
        private readonly IConfirmationEmailService<Customer> _customerConfirmationEmailService;

        public ConfirmationController(IConfirmationService confirmationService, ICustomerService customerService,
            IConfirmationEmailService<Customer> customerConfirmationEmailService)
        {
            _confirmationService = confirmationService;
            _customerService = customerService;
            _customerConfirmationEmailService = customerConfirmationEmailService;
        }

        [HttpGet("confirm/{guid}", Name = "ConfirmAccount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmAccountAsync(string guid)
        {
            if (!Guid.TryParse(guid, out var resultGuid)) return BadRequest();

            var result = await _confirmationService.ValidateGuid(resultGuid, ConfirmationEmailType.AccountCreation);
            return Ok(result);
        }

        [HttpGet("forget/{guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmForgetPasswordAsync(string guid)
        {
            if (!Guid.TryParse(guid, out var resultGuid)) return BadRequest();

            var result = await _confirmationService.ValidateGuid(resultGuid, ConfirmationEmailType.ForgotPassword);
            return Ok(result);
        }

        
        [HttpGet("resend/{previousGuid}", Name = "ResendConfirmation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResendConfirmationEmail(string previousGuid)
        {
            var existing = await _confirmationService.FirstOrDefaultAsync(c => c.Guid == Guid.Parse(previousGuid));

            if (existing == null)
            {
                return BadRequest();
            }

            var customer = await _customerService.FirstOrDefaultAsync(c => c.ContactEmail == existing.Email);
            var result = await _customerConfirmationEmailService.SendConfirmationEmail(customer);
            return Ok(result);
        }
    }
}