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
    public class FarmerAssociationController : ControllerBase
    {
        private readonly IFarmerAssociationService _farmerAssociationService;

        public FarmerAssociationController(IFarmerAssociationService farmerAssociationService)
        {
            _farmerAssociationService = farmerAssociationService;
        }
        // GET: api/FarmerAssociation
        [HttpGet]
        public IEnumerable<FarmerAssociation> Get()
        {
            return _farmerAssociationService.GetAll();
        }

        // GET: api/FarmerAssociation/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FarmerAssociation>> GetById(int id)
        {
            var farmerAssociation = await _farmerAssociationService.GetByIdAsync(id);

            if (farmerAssociation == null)
            {
                return NotFound();
            }

            return Ok(farmerAssociation);
        }

        // POST: api/FarmerAssociation
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] FarmerAssociationCreateRequest request)
        {
            var entity = new FarmerAssociation()
            {
                Name = request.Name,
                Address = request.Address,
                ContactNo = request.ContactNo,
                Email = request.Email,
                Latitude = request.Latitude,
                Longitude = request.Longitude
            };

            await _farmerAssociationService.InsertAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        // PUT: api/FarmerAssociation/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put(int id, [FromBody] FarmerAssociationUpdateRequest model)
        {
            var entity = await _farmerAssociationService.GetByIdAsync(id);

            if (entity == null)
            {
                return BadRequest();
            }

            entity.Name = model.Name ?? entity.Name;
            entity.Address = model.Address ?? entity.Address;
            entity.ContactNo = model.ContactNo ?? entity.ContactNo;
            entity.Email = model.Email ?? entity.Email;
            entity.Latitude = model.Latitude ?? entity.Latitude;
            entity.Longitude = model.Longitude?? entity.Longitude;

            await _farmerAssociationService.Update(entity);
            return Ok(entity);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _farmerAssociationService.GetByIdAsync(id);
            if(item == null)
            {
                return NotFound();
            }

            await _farmerAssociationService.DeleteById(id);
            return NoContent();
        }
    }
}