using TechShop.Domain;
using TechShop.Dtos.User;

namespace TechShop.Helpers.Security.Interfaces
{
    public interface IJwtAuthManager
    {
        string GenerateToken(UserWithRoleRequest user);
    }
}
