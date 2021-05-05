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
    public class UnitOfMeasureController : ControllerBase
    {
        private IUnitOfMeasureService _unitOfMeasureService;

        public UnitOfMeasureController(IUnitOfMeasureService unitOfMeasureService)
        {
            _unitOfMeasureService = unitOfMeasureService;
        }

        // GET: api/UnitOfMeasure
        [HttpGet]
        public IEnumerable<UnitOfMeasure> Get()
        {
            return _unitOfMeasureService.GetAll();
        }

        // GET: api/UnitOfMeasure/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UnitOfMeasure>> GetById(int id)
        {
            var unitOfMeasure = await _unitOfMeasureService.GetByIdAsync(id);

            if (unitOfMeasure == null)
            {
                return NotFound();
            }

            return Ok(unitOfMeasure);
        }

        // POST: api/UnitOfMeasure
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] UnitOfMeasureCreateRequest request)
        {
            // TODO Andrei: Use automapper 
            var entity = new UnitOfMeasure(request.Name)
            {
                IsDecimal = request.IsDecimal,
                ShortName = request.ShortName
            };

            await _unitOfMeasureService.InsertAsync(entity);
            return CreatedAtAction(nameof(GetById), new {id = entity.Id}, entity);
        }

        // PUT: api/UnitOfMeasure/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put(int id, [FromBody] UnitOfMeasureUpdateRequest model)
        {
            var entity = await _unitOfMeasureService.GetByIdAsync(id);

            if (entity == null)
            {
                return BadRequest();
            }

            // TODO Andrei: Use automapper 

            entity.Name = model.Name ?? entity.Name;
            entity.ShortName = model.ShortName ?? entity.ShortName;
            entity.IsDecimal = model.IsDecimal ?? entity.IsDecimal;

            await _unitOfMeasureService.Update(entity);
            return Ok(entity);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _unitOfMeasureService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            await _unitOfMeasureService.DeleteById(id);
            return NoContent();
        }
    }
}