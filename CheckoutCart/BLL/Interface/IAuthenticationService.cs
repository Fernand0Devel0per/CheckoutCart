using CheckoutCart.Dtos.User;

namespace CheckoutCart.BBL.Interface
{
    public interface IAuthenticationService
    {
        Task<string> AuthenticateAsync(LoginRequest loginRequest);
    }
}
