using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FarmHub.API.Models;
using FarmHub.Application.Services;
using FarmHub.Application.Services.Repositories;
using FarmHub.Application.Services.Services;
using FarmHub.Application.Services.Services.Interface;
using FarmHub.Data.Models;
using FarmHub.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;

namespace FarmHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly IMapper _mapper;
        private readonly ILogger<RegistrationController> _logger;
        private readonly IGoogleRecaptchaService _recaptchaService;
        private readonly IConfirmationEmailService<Customer> _confirmationEmailService;
        private readonly LinkGenerator _linkGenerator;

        public RegistrationController(IUserRegistrationService userRegistrationService, 
            IGoogleRecaptchaService recaptchaService,
            IConfirmationEmailService<Customer> confirmationEmailService,
            IMapper mapper, ILogger<RegistrationController> logger)
        {
            _userRegistrationService = userRegistrationService;
            _mapper = mapper;
            _logger = logger;
            _recaptchaService = recaptchaService;
            _confirmationEmailService = confirmationEmailService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post(UserRegistrationRequestCreateModel model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            var recaptchaResponse = await _recaptchaService.ValidateRecaptchaToken(model.RecaptchaToken);

            if (!recaptchaResponse.Success) return Unauthorized();
            
            var customer = new Customer()
            {
                Gender = (Gender) model.Gender,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Birthday = model.Birthday,
                ContactEmail = model.Email,
                UserName = model.Email
            };
            
            var result = await _userRegistrationService.CreateCustomerAndUserAsync(model.Email, model.Email, model.Password, customer);

            if (result.Errors != null && result.Errors.Any())
            {
                return BadRequest(result.Errors);
            }
            
            await _confirmationEmailService.SendConfirmationEmail(customer);
            return CreatedAtAction(nameof(GetUserById), new { id = result.AuthUser.Id }, result.AuthUser);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById(int id)
        {
            var authUser = await _userRegistrationService.GetUserByIdAsync(id);
            var response = _mapper.Map<AuthUser, UserRegistrationResponse>(authUser);
            return Ok(response);
        }
    }
}