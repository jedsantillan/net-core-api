using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FarmHub.API.Models;
using FarmHub.Application.Services.Repositories;
using FarmHub.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FarmHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountService _discountService;
        private readonly IMapper _mapper;

        public DiscountController(IDiscountService discountService, IMapper mapper)
        {
            _discountService = discountService;
            _mapper = mapper;
        }

        // GET: api/Discount
        [HttpGet]
        public async Task<List<DiscountViewResponse>> Get()
        {
            var discountList = await _discountService.GetAllListAsync();
            var discountViewList = _mapper.Map<List<Discount>, List<DiscountViewResponse>>(discountList);

            return discountViewList;
        }

        // GET: api/Discount/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DiscountViewResponse>> GetById(int id)
        {
            var discountItem = await _discountService.GetByIdAsync(id);

            if (discountItem == null)
                return NotFound();

            var discountViewItem = _mapper.Map<Discount, DiscountViewResponse>(discountItem);

            return Ok(discountViewItem);
        }

        // POST: api/Discount
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] DiscountCreateRequest model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            var discount = _mapper.Map<DiscountCreateRequest, Discount>(model);

            await _discountService.InsertAsync(discount);

            return CreatedAtAction(nameof(GetById), new { id = discount.Id }, discount);
        }


        // PUT: api/Discount/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(int id, [FromBody] DiscountUpdateRequest model)
        {
            var entity = await _discountService.GetByIdAsync(id);
            if (entity == null)
            {
                return BadRequest();
            }

            try
            {
                _mapper.Map(model, entity);

                await _discountService.Update(entity);

                return Ok(entity);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IsDiscountIdExists(id))
                {
                    return NotFound();
                }
                throw;
            }

        }

        // DELETE: api/Discount/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _discountService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            await _discountService.DeleteById(id);
            return NoContent();
        }

        private bool IsDiscountIdExists(int id)
        {
            return _discountService.IsIdExists(id);
        }



    }
}
