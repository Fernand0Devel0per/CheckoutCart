using AutoMapper;
using TechShop.Domain;
using TechShop.Dtos.Category;
using TechShop.Dtos.Order;
using TechShop.Dtos.Product;
using TechShop.Dtos.Status;
using TechShop.Dtos.User;

namespace TechShop.Helpers.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserCreateRequest, User>()
              .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
              .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
              .ForMember(dest => dest.RoleId, opt => opt.Ignore());

            CreateMap<User, UserCreateResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Role, opt => opt.Ignore());

            CreateMap<Status, StatusResponse>();

            CreateMap<Category, CategoryResponse>();

            CreateMap<ProductCreateRequest, Product>()
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<Product, ProductCreateResponse>()
                .ForMember(dest => dest.Category, opt => opt.Ignore());

            CreateMap<Product, ProductResponse>()
                .ForMember(dest => dest.Category, opt => opt.Ignore());

            CreateMap<ProductOrder, ProductOrderItemResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.PriceAtOrder));
            
            CreateMap<Order, OrderCreateResponse>()
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate.ToString("dd/MM/yyyy")))
                .ForMember(dest => dest.Status, opt => opt.Ignore());

            CreateMap<Order, OrderSearchResponse>()
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate.ToString("dd/MM/yyyy")))
                .ForMember(dest => dest.Status, opt => opt.Ignore());

            CreateMap<Order, OrderWithProductResponse>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id));

        }
    }
}
