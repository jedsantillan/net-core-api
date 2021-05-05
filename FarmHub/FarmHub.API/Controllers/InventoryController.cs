using System.Collections.Generic;
using System.Threading.Tasks;
using FarmHub.API.Models;
using FarmHub.Application.Services.Repositories;
using FarmHub.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FarmHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }
        
        // GET: api/Inventory
        [HttpGet]
        public async Task<List<Inventory>> Get()
        {
            return await _inventoryService.GetAllListAsync();
        }

        // GET: api/Inventory/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Inventory>> GetById(int id)
        {
            var inventory = await _inventoryService.GetByIdAsync(id);

            if(inventory == null)
            {
                return NotFound();
            }

            return Ok(inventory);
        }

        // POST: api/Inventory
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] InventoryCreateRequest request)
        {
            var entity = new Inventory()
            {
                ProductId = request.ProductId,
                HarvestPeriodId = request.HarvestPeriodId,
                FarmerAssociationId = request.FarmerAssociationId,
                Amount = request.Amount
            };

            await _inventoryService.InsertAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        // PUT: api/Inventory/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put(int id, [FromBody] InventoryUpdateRequest model)
        {
            var entity = await _inventoryService.GetByIdAsync(id);

            if (entity == null)
            {
                return BadRequest();
            }

            entity.ProductId = model.ProductId ?? entity.ProductId;
            entity.HarvestPeriodId = model.HarvestPeriodId ?? entity.HarvestPeriodId;
            entity.FarmerAssociationId = model.FarmerAssociationId ?? entity.FarmerAssociationId;
            entity.Amount = model.Amount ?? entity.Amount;

            await _inventoryService.Update(entity);
            return Ok(entity);
        }

        // DELETE: api/Inventory/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _inventoryService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            await _inventoryService.DeleteById(id);
            return NoContent();
        }
    }
}