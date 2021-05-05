using System;
using System.Threading.Tasks;
using FarmHub.API.Models;
using FarmHub.Application.Services;
using FarmHub.Application.Services.Infrastructure;
using FarmHub.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FarmHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private ICacheService _cacheService;
        private CacheConfiguration _cacheConfig;
        private ILogger<CartController> _logger;

        public CartController(ICacheService cacheService, IOptions<CacheConfiguration> cacheConfig, ILogger<CartController> logger)
        {
            _cacheService = cacheService;
            _cacheConfig = cacheConfig.Value;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCachedCart(string cacheKey)
        {
            try
            {
                var response = await _cacheService.GetCachedResponseAsync<CartItemModel[]>(cacheKey);
                if (response == null)
                {
                    return NotFound();
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CacheCart(string cacheKey, CartItemModel[] model)
        {
            try
            {
                await _cacheService.CacheResponseAsync(cacheKey, model, _cacheConfig.CartCacheTimeSpan);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoveCachedCart(string cacheKey)
        {
            
            try
            {
                await _cacheService.DeleteCachedResponseAsync(cacheKey);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }
    }
}