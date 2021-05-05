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
    public class CarouselController : ControllerBase
    {
        private ICarouselService _carouselService;
        private readonly IMapper _mapper;

        public CarouselController(ICarouselService carouselService, IMapper mapper)
        {
            _carouselService = carouselService;
            _mapper = mapper;
        }

        [HttpGet("type/{type}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Carousel>>> GetByType(CarouselType type)
        {
            var car = await _carouselService.GetCarouselByType(type);
            if (car == null)
            {
                return NotFound();
            }

            return Ok(car);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] CarouselRequest model)
        {
            var carousel = _mapper.Map<CarouselRequest, Carousel>(model);
            await _carouselService.InsertAsync(carousel);
            return CreatedAtAction(nameof(GetById), new {id = carousel.Id}, carousel);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Carousel>> GetById(int id)
        {
            var car = await _carouselService.GetByIdAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            return Ok(car);
        }
    }
}