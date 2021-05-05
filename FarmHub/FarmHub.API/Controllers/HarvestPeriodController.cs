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
    public class HarvestPeriodController : ControllerBase
    {
        private IHarvestPeriodService _harvestPeriodService;

        public HarvestPeriodController(IHarvestPeriodService harvestPeriodService)
        {
            _harvestPeriodService = harvestPeriodService;
        }

        // GET: api/HarvestPeriod
        [HttpGet]
        public async Task<List<HarvestPeriod>> Get()
        {
            return await _harvestPeriodService.GetAllListAsync();
        }

        // GET: api/HarvestPeriod/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<HarvestPeriod>> GetById(int id)
        {
            var harvestPeriod = await _harvestPeriodService.GetByIdAsync(id);

            if (harvestPeriod == null)
            {
                return NotFound();
            }

            return Ok(harvestPeriod);
        }

        // POST: api/HarvestPeriod
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] HarvestPeriodCreateRequest request)
        {
            // TODO Andrei: Use automapper 
            var entity = new HarvestPeriod(request.Name)
            {
                Description = request.Description,
                StartOrderDate = request.StartOrderDate,
                LastOrderDate = request.LastOrderDate,
                StartCommitmentDate = request.StartCommitmentDate,
                LastCommitmentDate = request.LastCommitmentDate,
                DispatchDate = request.DispatchDate
            };

            await _harvestPeriodService.InsertAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        // PUT: api/HarvestPeriod/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put(int id, [FromBody] HarvestPeriodUpdateRequest model)
        {
            var entity = await _harvestPeriodService.GetByIdAsync(id);

            if (entity == null)
            {
                return BadRequest();
            }

            // TODO Andrei: Use automapper 

            entity.Name = model.Name ?? entity.Name;
            entity.Description = model.Description ?? entity.Description;
            entity.IsActive = model.IsActive ?? entity.IsActive;
            entity.StartOrderDate = model.StartOrderDate ?? entity.StartOrderDate;
            entity.LastOrderDate = model.LastOrderDate ?? entity.LastOrderDate;
            entity.StartCommitmentDate = model.StartCommitmentDate ?? entity.StartCommitmentDate;
            entity.LastCommitmentDate = model.LastCommitmentDate ?? entity.LastCommitmentDate;
            entity.DispatchDate = model.DispatchDate ?? entity.DispatchDate;

            await _harvestPeriodService.Update(entity);
            return Ok(entity);
        }

        // DELETE: api/HarvestPeriod/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _harvestPeriodService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            await _harvestPeriodService.DeleteById(id);
            return NoContent();
        }
    }
}