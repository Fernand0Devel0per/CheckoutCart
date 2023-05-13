using CheckoutCart.Domain;
using CheckoutCart.Dtos.User;

namespace CheckoutCart.BBL.Interface
{
    public interface IUserService
    {
        Task<UserCreateResponse> GetUserByUsernameAsync(string username);
        Task<UserCreateResponse> CreateUserAsync(UserCreateRequest userDto);
        Task<bool> UpdateUserAsync(UserUpdateRequest userDto, string username);
    }
}
