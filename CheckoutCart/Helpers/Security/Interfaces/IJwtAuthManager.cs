using CheckoutCart.Domain;
using CheckoutCart.Dtos.User;

namespace CheckoutCart.Helpers.Security.Interfaces
{
    public interface IJwtAuthManager
    {
        string GenerateToken(UserWithRoleRequest user);
    }
}
