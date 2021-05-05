using AutoMapper;
using FarmHub.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using FarmHub.Application.Services.Repositories;
using FarmHub.Data.Models;

namespace FarmHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;

        public CustomerController(ICustomerService customerService,IMapper mapper)
        {
            _customerService = customerService;
            _mapper = mapper;
        }

        // GET: api/Customer
        [HttpGet]
        public async Task<List<CustomerViewResponse>> Get()
        {
            var customerList = await _customerService.GetAllListAsync();
            var getCustomerList = _mapper.Map<List<Customer>, List<CustomerViewResponse>>(customerList);

            return getCustomerList;
        }

        // GET: api/Customer/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerViewResponse>> GetById(int id)
        {
            var customer = await _customerService.GetByIdAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            var getCustomerRequest = _mapper.Map<Customer, CustomerViewResponse>(customer);

            return Ok(getCustomerRequest);
        }
        
        // GET: api/Customer/5
        [HttpGet("user/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerViewResponse>> GetByUserId(int id)
        {
            var customer = await _customerService.FirstOrDefaultAsync(c => c.AuthUser != null && c.AuthUser.Id == id);

            if (customer == null)
            {
                return NotFound();
            }

            var getCustomerRequest = _mapper.Map<Customer, CustomerViewResponse>(customer);

            return Ok(getCustomerRequest);
        }

        // POST: api/Customer
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] CustomerCreateRequest model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            var customer = _mapper.Map<CustomerCreateRequest, Customer>(model);
            await _customerService.InsertAsync(customer);

            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
        }

        // PUT: api/Customer/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(int id, [FromBody] CustomerUpdateRequest model)
        {
            var entity = await _customerService.GetByIdAsync(id);

            if (entity == null)
            {
                return BadRequest();
            }

            try
            {
                _mapper.Map(model, entity);

                await _customerService.Update(entity);

                return Ok(model);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IsCustomerIdExists(id))
                {
                    return NotFound();
                }
                throw;
            }
        }

        // DELETE: api/Customer/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _customerService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            await _customerService.DeleteById(id);
            return NoContent();
        }


        private bool IsCustomerIdExists(int id)
        {
            return _customerService.IsIdExists(id);
        }
    }
}
