using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FarmHub.API.Models;
using FarmHub.Application.Services.Repositories;
using FarmHub.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FarmHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PortionController : ControllerBase
    {
        private IPortionService _portionService;
        private readonly IMapper _mapper;

        public PortionController(IPortionService unitOfMeasureService, IMapper mapper)
        {
            _portionService = unitOfMeasureService;
            _mapper = mapper;
        }


        // GET: api/Portion
        [HttpGet]
        public async Task<List<PortionViewResponse>> Get()
        {
            var portionList = await _portionService.GetAllListAsync();
            var getPortionRequestList = _mapper.Map<List<Portion>, List<PortionViewResponse>>(portionList);

            return getPortionRequestList;
        }


        // GET: api/Portion/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PortionViewResponse>> GetById(int id)
        {
            var portion = await _portionService.GetByIdAsync(id);

            if (portion == null)
            {
                return NotFound();
            }

            var getPortionRequestItem = _mapper.Map<Portion, PortionViewResponse>(portion);

            return Ok(getPortionRequestItem);
        }

        // POST: api/Portion
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] PortionCreateRequest request)
        {
            // TODO Andrei: Use automapper 
            var entity = new Portion(request.DisplayName, request.RealDecimalValue)
            {
                DisplayName = request.DisplayName,
                RealDecimalValue = request.RealDecimalValue
            };

            await _portionService.InsertAsync(entity);
            return CreatedAtAction(nameof(GetById), new {id = entity.Id}, entity);
        }

        // PUT: api/Portion/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put(int id, [FromBody] PortionUpdateRequest model)
        {
            var entity = await _portionService.GetByIdAsync(id);

            if (entity == null)
            {
                return BadRequest();
            }

            // TODO Andrei: Use automapper 

            entity.DisplayName = model.DisplayName ?? entity.DisplayName;
            entity.RealDecimalValue = model.RealDecimalValue ?? entity.RealDecimalValue;

            await _portionService.Update(entity);
            return Ok(entity);
        }

        // DELETE: api/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _portionService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            await _portionService.DeleteById(id);
            return NoContent();
        }
    }
}