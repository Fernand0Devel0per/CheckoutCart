using AutoMapper;
using CheckoutCart.Domain;
using CheckoutCart.Dtos.Category;
using CheckoutCart.Dtos.Status;
using CheckoutCart.Dtos.User;

namespace CheckoutCart.Helpers.AutoMapper
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

        }
    }
}
