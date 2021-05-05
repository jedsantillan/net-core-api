using System;
using AutoMapper;
using FarmHub.API.Models;
using FarmHub.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using FarmHub.Application.Services.Repositories;
using FarmHub.Application.Services.Services.Interface;
using Microsoft.Extensions.Logging;

namespace FarmHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IConfirmationEmailService<Order> _orderConfirmationEmailService;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, 
            IConfirmationEmailService<Order> orderConfirmationEmailService,
            IMapper mapper,
            ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _orderConfirmationEmailService = orderConfirmationEmailService;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/Order
        [HttpGet]
        public async Task<List<OrderViewResponse>> Get()
        {
            var orderList = await _orderService.GetAllListAsync();
            var getOrderList = _mapper.Map<List<Order>, List<OrderViewResponse>>(orderList);

            return getOrderList;
        }

        // GET: api/Order/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderViewResponse>> GetById(int id)
        {
            var order = await _orderService.GetByIdAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            var getOrderRequest = _mapper.Map<Order, OrderViewResponse>(order);

            return Ok(getOrderRequest);
        }

        // POST: api/Order
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] OrderCreateRequest model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            var order = _mapper.Map<OrderCreateRequest, Order>(model);

            await _orderService.InsertAsync(order);

            var orderResponse = _mapper.Map<Order, OrderViewResponse>(order);

            return CreatedAtAction(nameof(GetById), new { id = order.Id }, orderResponse);
        }

        // PUT: api/Order/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put(int id, [FromBody] OrderUpdateRequest model)
        {
            var entity = await _orderService.GetByIdAsync(id);
            if (entity == null)
            {
                return BadRequest();
            }

            try
            {
                // if update changes the status
                var sendConfirmationEmail = model.Status == OrderStatus.Acknowledged &&
                                            entity.Status != OrderStatus.Acknowledged;
                _mapper.Map(model, entity);

                await _orderService.Update(entity);

                if (sendConfirmationEmail)
                {
                    try
                    {
                        var result = await _orderConfirmationEmailService.SendConfirmationEmail(entity);

                        if (result == 0)
                        {
                            _logger.LogError($"OrderConfirmation Email for Order Id {entity.Id} not sent.");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }
                }

                return Ok(entity);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IsOrderIdExists(id))
                {
                    return NotFound();
                }
                throw;
            }
        }

        // DELETE: api/Order/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _orderService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            await _orderService.DeleteById(id);
            return NoContent();
        }

        private bool IsOrderIdExists(int id)
        {
            return _orderService.IsIdExists(id);
        }

        // DELETE: api/Order/5
        [HttpDelete("confirm/{orderId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ConfirmById(int orderId)
        {
            var order = await _orderService.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null) return NotFound();

            var result = await _orderService.ConfirmOrderById(orderId);
            return Ok(result);
        }
    }
}
