using AutoMapper;
using FarmHub.API.Models;
using FarmHub.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using FarmHub.Application.Services.Repositories;

namespace FarmHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingAddressController : ControllerBase
    {
        private readonly IShippingAddressService _shippingAddressService;
        private readonly IMapper _mapper;

        public ShippingAddressController(IShippingAddressService shippingAddressService, IMapper mapper)
        {
            _shippingAddressService = shippingAddressService;
            _mapper = mapper;
        }

        // GET: api/ShippingAddress
        [HttpGet]
        public async Task<List<ShippingAddressViewResponse>> Get()
        {
            var shippingAddressList = await _shippingAddressService.GetAllListAsync();
            var getShippingAddressList = _mapper.Map<List<ShippingAddress>, List<ShippingAddressViewResponse>>(shippingAddressList);

            return getShippingAddressList;
        }

        // GET: api/ShippingAddress/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ShippingAddressViewResponse>> GetById(int id)
        {
            var shippingAddress = await _shippingAddressService.GetByIdAsync(id);

            if (shippingAddress == null)
            {
                return NotFound();
            }

            var getShippingAddressRequest = _mapper.Map<ShippingAddress, ShippingAddressViewResponse>(shippingAddress);

            return Ok(getShippingAddressRequest);

        }

        [Route("customer/{customerId:int}")]
        [HttpGet]
        public async Task<List<ShippingAddressViewResponse>> GetAddressByCustomerId(int customerId)
        {
            var shippingAddressList = await _shippingAddressService.GetAllAddressByCustomerIdAsync(customerId);
            var getShippingAddressList = _mapper.Map<List<ShippingAddress>, List<ShippingAddressViewResponse>>(shippingAddressList);

            return getShippingAddressList;
        }

        // POST: api/ShippingAddress
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] ShippingAddressCreateRequest model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            var shippingAddress = _mapper.Map<ShippingAddressCreateRequest, ShippingAddress>(model);
            await _shippingAddressService.InsertAsync(shippingAddress);

            return CreatedAtAction(nameof(GetById), new { id = shippingAddress.Id }, shippingAddress);
        }

        // PUT: api/ShippingAddress/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(int id, [FromBody] ShippingAddressUpdateRequest model)
        {
            var entity = await _shippingAddressService.GetByIdAsync(id);

            if (entity == null)
            {
                return BadRequest();
            }

            try
            {
                _mapper.Map(model, entity);

                await _shippingAddressService.Update(entity);

                return Ok(model);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IsShippingAddressIdExists(id))
                {
                    return NotFound();
                }
                throw;
            }
        }


        // DELETE: api/ShippingAddress/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _shippingAddressService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            await _shippingAddressService.DeleteById(id);
            return NoContent();
        }

        private bool IsShippingAddressIdExists(int id)
        {
            return _shippingAddressService.IsIdExists(id);
        }

    }
}
