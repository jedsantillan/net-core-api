using System.Linq;
using Adyen.Model.Checkout;
using AutoMapper;
using FarmHub.Application.Services.Infrastructure;
using FarmHub.Data.Models;

namespace FarmHub.API.Models
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CategoryCreateRequest, Category>();
            CreateMap<Category, CategoryResponse>().ReverseMap();

            CreateMap<ProductCreateRequest, Product>()
                .ForMember(p => p.Images, opt => opt.Ignore());
            CreateMap<ProductUpdateRequest, Product>().ReverseMap();

            CreateMap<UnitOfMeasure, UnitOfMeasureViewResponse>().ReverseMap();
            CreateMap<PortionViewResponse, Portion>().ReverseMap();

            CreateMap<DiscountCreateRequest, Discount>();
            CreateMap<DiscountUpdateRequest, Discount>();
            CreateMap<Discount, DiscountViewResponse>()
                .ForMember(dest => dest.DiscountType, src => src.MapFrom(src => src.DiscountType.ToString()));

            CreateMap<Image, ImageRequest>().ReverseMap();
            CreateMap<Image, ImageViewResponse>()
                .ForMember(dest => dest.ImageType, src => src.MapFrom(src => src.ImageType.ToString()))
                .ForMember(dest => dest.Url, src => src.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.ProductId, src => src.MapFrom(src => src.ProductId));

            CreateMap<ProductPortion, ProductPortionViewResponse>()
                .ForMember(dest => dest.Id, src => src.MapFrom(src => src.Portion.Id))
                .ForMember(dest => dest.DisplayName, src => src.MapFrom(src => src.Portion.DisplayName))
                .ForMember(dest => dest.RealDecimalValue, src => src.MapFrom(src => src.Portion.RealDecimalValue))
                .ForMember(dest => dest.Price, src => src.MapFrom(src => src.Price));

            CreateMap<Product, ProductViewResponse>()
                .ForMember(dest => dest.Discount, src => src.MapFrom(src => src.Discount))
                .ForMember(dest => dest.Category, src => src.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.CategoryId, src => src.MapFrom(src => src.Category.Id))
                .ForPath(dest => dest.Image, src => src.MapFrom(src => src.MainImage))
                .ForMember(dest => dest.Portions, src => src.MapFrom(src => src.ProductPortions
                    .Select(x => x).ToList()))
                .ForMember(dest => dest.Tags, src => src.MapFrom(src => src.ProductTags.Select(t => t.Name).ToArray()));

            CreateMap<MayaniPaymentMethod, DefaultPaymentMethodDetails>();

            CreateMap<CustomerCreateRequest, Customer>();
            CreateMap<CustomerUpdateRequest, Customer>().ReverseMap();

            CreateMap<Customer, CustomerViewResponse>();

            CreateMap<ShippingAddressCreateRequest, ShippingAddress>();
            CreateMap<ShippingAddressUpdateRequest, ShippingAddress>().ReverseMap();

            CreateMap<ShippingAddress, ShippingAddressViewResponse>()
                .ForMember(dest => dest.CustomerId, src => src.MapFrom(src => src.CustomerId));

            CreateMap<OrderItemCreateRequest, OrderItem>();
            CreateMap<OrderItemUpdateRequest, OrderItem>().ReverseMap();

            CreateMap<OrderItem, OrderItemViewResponse>()
                .ForMember(dest => dest.Product, src => src.MapFrom(s => s.ProductPortion.Product.Name))
                .ForMember(dest => dest.Portion, src => src.MapFrom(s => s.ProductPortion.Portion.DisplayName))
                .ForMember(dest => dest.PortionValue, src => src.MapFrom(s => s.ProductPortion.Portion.RealDecimalValue))
                .ForMember(dest => dest.UnitOfMeasure, src => src.MapFrom(s => s.ProductPortion.Product.UnitOfMeasure.ShortName))
                .ForMember(dest => dest.Total, src => src.MapFrom(s => s.ProductPortion.Portion.RealDecimalValue * s.Quantity));

            CreateMap<OrderCreateRequest, Order>();
            CreateMap<OrderUpdateRequest, Order>().ReverseMap();

            CreateMap<Order, OrderViewResponse>();

            CreateMap<Basket, BasketViewResponse>()
                .ForMember(dest => dest.Products, src => src.MapFrom(src => src.BasketProducts
                    .Select(x => x).ToList()));
            
            CreateMap<BasketProduct, BasketProductViewResponse>()
                .ForMember(dest => dest.ProductName, src => src.MapFrom(src => src.Product.Name));

            CreateMap<UserRegistrationRequestCreateModel, Customer>();
            CreateMap<AuthUser, UserRegistrationResponse>().ReverseMap();

            CreateMap<CarouselRequest, Carousel>();
            CreateMap<CarouselItemRequest, CarouselItem>();
        }
    }
}
