using AutoMapper;
using FarmHub.API.Models;
using FarmHub.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using FarmHub.Application.Services.Repositories;

namespace FarmHub.API.Controllers
{
    [Route("api/controller")]
    [ApiController]
    public class OrderItemController : ControllerBase
    {
        private IOrderItemService _orderItemService;
        private readonly IMapper _mapper;

        public OrderItemController(IOrderItemService orderItemService, IMapper mapper)
        {
            _orderItemService = orderItemService;
            _mapper = mapper;
        }

        // GET: api/OrderItem
        [HttpGet]
        public async Task<List<OrderItemViewResponse>> Get()
        {
            var orderItemList = await _orderItemService.GetAllListAsync();
            var getOrderItemList = _mapper.Map<List<OrderItem>, List<OrderItemViewResponse>>(orderItemList);

            return getOrderItemList;
        }

        // GET: api/OrderItem/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderItem>> GetById(int id)
        {
            var orderItem = await _orderItemService.GetByIdAsync(id);

            if (orderItem == null)
            {
                return NotFound();
            }

            var getOrderItemRequest = _mapper.Map<OrderItem, OrderItemViewResponse>(orderItem);

            return Ok(getOrderItemRequest);
        }

        // DELETE: api/OrderItem/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _orderItemService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            await _orderItemService.DeleteById(id);
            return NoContent();
        }


    }
}
