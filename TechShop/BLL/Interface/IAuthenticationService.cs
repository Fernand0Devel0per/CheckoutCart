using TechShop.Dtos.User;

namespace TechShop.BBL.Interface
{
    public interface IAuthenticationService
    {
        Task<string> AuthenticateAsync(LoginRequest loginRequest);
    }
}
