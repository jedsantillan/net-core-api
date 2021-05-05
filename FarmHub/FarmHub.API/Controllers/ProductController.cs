using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FarmHub.API.Models;
using FarmHub.Application.Services.Infrastructure;
using FarmHub.Application.Services.Repositories;
using FarmHub.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FarmHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IAzureStorageService _storageService;
        private readonly IMapper _mapper;
        private ITagService _tagService;

        public ProductController(IProductService productService, ITagService tagService,
            IAzureStorageService storageService, IMapper mapper)
        {
            _productService = productService;
            _storageService = storageService;
            _tagService = tagService;
            _mapper = mapper;
        }

        // GET: api/Product
        [HttpGet]
        public async Task<List<ProductViewResponse>> Get()
        {
            var productList = await _productService.GetAllListAsync();
            var getProductRequestList = _mapper.Map<List<Product>, List<ProductViewResponse>>(productList);

            foreach (var item in getProductRequestList)
            {
                foreach (var portion in item.Portions)
                {
                    if (item.Discount != null)
                    {
                        DiscountTypeEnum discountType = (DiscountTypeEnum) Enum.Parse(typeof(DiscountTypeEnum),
                            item?.Discount.DiscountType, true);
                        portion.DiscountedPrice =
                            CalculateDiscount(portion.Price, item.Discount.DiscountValue, discountType);
                    }
                }
            }

            return getProductRequestList;
        }

        // GET: api/Product
        [HttpGet("new")]
        public async Task<List<ProductViewResponse>> GetNew(int count)
        {
            var productList = await _productService.GetAllListAsync();
            var getProductRequestList = _mapper.Map<List<Product>, List<ProductViewResponse>>(productList);

            return getProductRequestList.OrderByDescending(p => p.Id).Take(count).ToList();
        }

        // GET: api/Product/promotion
        [HttpGet("promotion")]
        public async Task<List<ProductViewResponse>> GetProductPromotion()
        {
            var productPromotionList = await _productService.GetAllPromotionListAsync();

            var getProductRequestList = _mapper.Map<List<Product>, List<ProductViewResponse>>(productPromotionList);

            return getProductRequestList;
        }


        // GET: api/Product/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductViewResponse>> GetById(int id)
        {
            var productItem = await _productService.GetByIdAsync(id);

            if (productItem == null)
                return NotFound();

            var getProductRequestItem = _mapper.Map<Product, ProductViewResponse>(productItem);

            foreach (var portion in getProductRequestItem.Portions)
            {
                if (getProductRequestItem.Discount != null)
                {
                    var discountType = (DiscountTypeEnum) Enum.Parse(typeof(DiscountTypeEnum),
                        getProductRequestItem?.Discount.DiscountType, true);
                    portion.DiscountedPrice = CalculateDiscount(portion.Price,
                        getProductRequestItem.Discount.DiscountValue, discountType);
                }
            }

            return Ok(getProductRequestItem);
        }

        [Route("{productId:int}/Portion/{portionId:int}")]
        [HttpGet]
        public async Task<ActionResult<ProductViewResponse>> GetProductPortionById(int productId, int portionId)
        {
            var productItem = await _productService.GetByIdProductPortionAsync(productId, portionId);

            if (productItem == null)
                return NotFound();

            var getProductRequestItem = _mapper.Map<Product, ProductViewResponse>(productItem);

            foreach (var portion in getProductRequestItem.Portions)
            {
                if (getProductRequestItem.Discount != null)
                {
                    DiscountTypeEnum discountType = (DiscountTypeEnum) Enum.Parse(typeof(DiscountTypeEnum),
                        getProductRequestItem?.Discount.DiscountType, true);
                    portion.DiscountedPrice = CalculateDiscount(portion.Price,
                        getProductRequestItem.Discount.DiscountValue, discountType);
                }
            }

            return Ok(getProductRequestItem);
        }

        [Route("{productId:int}/Images")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ImageViewResponse>> GetAllProductImagesById(int productId)
        {
            var productImageList = await _productService.GetAllImagesByProductIdAsync(productId);

            if (productImageList.Count == 0)
                return NotFound();

            var productImageViewList = _mapper.Map<List<Image>, List<ImageViewResponse>>(productImageList);

            return Ok(productImageViewList);
        }


        // POST: api/Product
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromForm] ProductCreateRequestForm formModel)
        {
            if (formModel == null)
            {
                return BadRequest();
            }

            var model = formModel.Product;

            var product = _mapper.Map<ProductCreateRequest, Product>(model);

            if (model.PortionIds != null)
            {
                foreach (var portionId in model.PortionIds)
                {
                    var productPortion = new ProductPortion
                    {
                        Product = product,
                        PortionId = portionId
                    };
                    product.ProductPortions.Add(productPortion);
                }
            }

            if (model.Tags != null && model.Tags.Any())
            {
                var tags = await _tagService.GetEntitiesForTagsAsync(model.Tags);
                product.ProductTags = tags;
            }

            if (formModel.Images != null)
            {
                var imageUploadTasks = formModel.Images.Select(async i => new Image()
                {
                    ImageType = ImageTypeEnum.ProductImage,
                    ImageUrl = await _storageService.UploadFile(i, "images"),
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow
                });

                var imageEntities = await Task.WhenAll(imageUploadTasks);
                product.Images = imageEntities;
            }

            await _productService.InsertAsync(product);

            return CreatedAtAction(nameof(GetById), new {id = product.Id}, product);
        }

        // PUT: api/Product/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put(int id, [FromBody] ProductUpdateRequest model)
        {
            var entity = await _productService.GetByIdAsync(id);
            if (entity == null)
            {
                return BadRequest();
            }

            try
            {
                _mapper.Map(model, entity);
                entity.Id = id;

                if (model.Tags != null && model.Tags.Any())
                {
                    var tags = await _tagService.GetEntitiesForTagsAsync(model.Tags);

                    foreach (var tag in entity.ProductTags.Except(tags))
                    {
                        entity.ProductTags?.Add(tag);
                    }
                }
                else
                {
                    entity.ProductTags = new List<Tag>();
                }

                _productService.UpdatePortions(entity, model.PortionIds);

                await _productService.Update(entity);

                return AcceptedAtAction(nameof(GetById), new {id = entity.Id}, entity);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IsProductIdExists(id))
                {
                    return NotFound();
                }

                throw;
            }
        }


        [HttpPut("Price/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PutPortionPrice(int id, [FromBody] ProductPriceUpdateRequest model)
        {
            var entity = await _productService.GetByIdAsync(id);

            if (entity == null)
            {
                return BadRequest();
            }

            try
            {
                _productService.UpdatePricePortion(entity, model.PortionId, model.Price);

                return Ok(entity);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IsProductIdExists(id))
                {
                    return NotFound();
                }

                throw;
            }
        }


        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _productService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            await _productService.DeleteById(id);
            return NoContent();
        }


        private bool IsProductIdExists(int id)
        {
            return _productService.IsIdExists(id);
        }

        private static double CalculateDiscount(double price, double discountValue, Enum discountType)
        {
            return discountType switch
            {
                DiscountTypeEnum.Percentage => price - (price * (discountValue * 0.01)),
                DiscountTypeEnum.FixedAmount => price - discountValue,
                _ => price
            };
        }

        [HttpGet("tag/{tag}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<ProductViewResponse>>> GetProductsByTag(string tag, [FromQuery] int limit = 1)
        {
            if (string.IsNullOrEmpty(tag))
            {
                return BadRequest();
            }

            var products = await _productService.GetAllByTagAsync(tag, limit);
            var productResponses = _mapper.Map<List<Product>, List<ProductViewResponse>>(products);
            return Ok(productResponses);
        }

        [HttpGet("best-sellers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<Product>>> GetProductsBySales([FromQuery] int limit = 10)
        {
            var products = await _productService.GetAllBySales(limit);
            return Ok(_mapper.Map<List<Product>, List<ProductViewResponse>>(products));
        }
    }
}