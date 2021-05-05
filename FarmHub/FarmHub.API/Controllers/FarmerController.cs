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
    public class FarmerController : ControllerBase
    {
        private IFarmerService _farmerService;

        public FarmerController(IFarmerService farmerService)
        {
            _farmerService = farmerService;
        }

        // GET: api/Farmer
        [HttpGet]
        public IEnumerable<Farmer> Get()
        {
            return _farmerService.GetAll();
        }

        // GET: api/Farmer/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Farmer>> GetById(int id)
        {
            var farmer = await _farmerService.GetByIdAsync(id);

            if (farmer == null)
            {
                return NotFound();
            }

            return Ok(farmer);
        }

        // POST: api/Farmer
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] FarmerCreateRequest request)
        {
            var entity = new Farmer()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Birthday = request.Birthday,
                Gender = request.Gender,
                Address = request.Address,
                ContactNumber = request.ContactNumber,
                ContactEmail = request.ContactEmail,
                EmailIsConfirmed = request.EmailIsConfirmed,
                ImageUrl = request.ImageUrl,
                FarmerAssociationId = request.FarmerAssociationId

            };

            await _farmerService.InsertAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        // PUT: api/Farmer/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put(int id, [FromBody] FarmerUpdateRequest model)
        {
            var entity = await _farmerService.GetByIdAsync(id);

            if (entity == null)
            {
                return BadRequest();
            }

            entity.FirstName = model.FirstName ?? entity.FirstName;
            entity.LastName = model.LastName ?? entity.LastName;
            entity.Birthday = model.Birthday ?? entity.Birthday;
            entity.Gender = model.Gender ?? entity.Gender;
            entity.Address = model.Address ?? entity.Address;
            entity.ContactNumber = model.ContactNumber ?? entity.ContactNumber;
            entity.ContactEmail = model.ContactEmail ?? entity.ContactEmail;
            entity.EmailIsConfirmed = model.EmailIsConfirmed ?? entity.EmailIsConfirmed;
            entity.ImageUrl = model.ImageUrl ?? entity.ImageUrl;
            entity.FarmerAssociationId = model.FarmerAssociationId ?? entity.FarmerAssociationId;

            await _farmerService.Update(entity);
            return Ok(entity);


        }

        // DELETE: api/Farmer/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _farmerService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            await _farmerService.DeleteById(id);
            return NoContent();
        }
    }
}
