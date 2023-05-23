using TechShop.Domain;
using TechShop.Dtos.User;

namespace TechShop.BBL.Interface
{
    public interface IUserService
    {
        Task<UserCreateResponse> GetUserByUsernameAsync(string username);
        Task<UserCreateResponse> CreateUserAsync(UserCreateRequest userDto);
        Task<bool> UpdateUserAsync(UserUpdateRequest userDto, string username);
    }
}
