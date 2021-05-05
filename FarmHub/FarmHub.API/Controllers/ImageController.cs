using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FarmHub.API.Models;
using FarmHub.Application.Services.Infrastructure;
using FarmHub.Application.Services.Repositories;
using FarmHub.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FarmHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;
        private IAzureStorageService _storageService;

        public ImageController(IImageService imageService, IAzureStorageService storageService,  IMapper mapper)
        {
            _imageService = imageService;
            _storageService = storageService;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<List<ImageViewResponse>> Get()
        {
            var imageList = await _imageService.GetAllListAsync();
            var imageViewList = _mapper.Map<List<Image>, List<ImageViewResponse>>(imageList);

            return imageViewList;
        }

        // GET: api/Discount/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ImageViewResponse>> GetById(int id)
        {
            var imageItem = await _imageService.GetByIdAsync(id);

            if (imageItem == null)
                return NotFound();

            var imageViewItem = _mapper.Map<Image, ImageViewResponse>(imageItem);
            return Ok(imageViewItem);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromForm]ImageRequest model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            var url = await _storageService.UploadFile(model.UploadFile, "images");

            var image = _mapper.Map<ImageRequest, Image>(model);
            image.ImageUrl = url;

            await _imageService.InsertAsync(image);

            return CreatedAtAction(nameof(GetById), new { id = image.Id }, image);
        }

    }
}