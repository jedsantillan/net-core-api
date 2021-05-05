using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FarmHub.API.Models;
using FarmHub.Application.Services.Repositories;
using FarmHub.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FarmHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService _basketService;
        private readonly IMapper _mapper;

        public BasketController(IBasketService basketService, IMapper mapper)
        {
            _basketService = basketService;
            _mapper = mapper;
        }


        // GET: api/Basket
        [HttpGet]
        public async Task<List<BasketViewResponse>> Get()
        {
            var basketList = await _basketService.GetAllListAsync();
            var getBasketList = _mapper.Map<List<Basket>, List<BasketViewResponse>>(basketList);

            return getBasketList;
        }

        // GET: api/Basket/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BasketViewResponse>> GetById(int id)
        {
            var basketItem = await _basketService.GetByIdAsync(id);

            if (basketItem == null)
                return NotFound();

            var getBasketResponsetItem = _mapper.Map<Basket, BasketViewResponse>(basketItem);

            return Ok(getBasketResponsetItem);
        }


    }
}
