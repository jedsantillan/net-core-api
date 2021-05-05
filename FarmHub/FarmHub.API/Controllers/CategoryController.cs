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
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }


        // GET: api/Category
        [HttpGet]
        public async Task<List<CategoryResponse>> Get()
        {
            var categoryList = await _categoryService.GetAllListAsync();
            var getCategoryRequestList = _mapper.Map<List<Category>, List<CategoryResponse>>(categoryList);

            return getCategoryRequestList;
        }

        // GET: api/Category/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryResponse>> GetById(int id)
        {
            Category categoryItem = await _categoryService.GetByIdAsync(id);

            if (categoryItem == null)
            {
                return NotFound();
            }

            var getCategoryRequestItem = _mapper.Map<Category, CategoryResponse>(categoryItem);

            return Ok(getCategoryRequestItem);
        }

        // POST: api/Category
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] CategoryCreateRequest model)
        {
            if (model == null)
            {
                return BadRequest();
            }
          
            Category category = _mapper.Map<CategoryCreateRequest, Category>(model);
            await _categoryService.InsertAsync(category);

            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        // PUT: api/Category/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put(int id, [FromBody] CategoryResponse value)
        {

            if (id != value.Id)
            {
                return BadRequest();
            }
            try
            {
                var category = _mapper.Map<CategoryResponse, Category>(value);
                await _categoryService.Update(category); // id passed from URL must match Category.Id

                return Ok(category);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IsCategoryIdExists(id))
                {
                    return NotFound();
                }
                else
                    throw;
            }

            
        }

        // DELETE: api/Category/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _categoryService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            await _categoryService.DeleteById(id);
            return NoContent();


        }


        private bool IsCategoryIdExists(int id)
        {
            return _categoryService.IsIdExists(id);
        }

    }
}