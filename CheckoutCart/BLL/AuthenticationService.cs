using TechShop.BBL.Interface;
using TechShop.DAL.Interface;
using TechShop.Domain;
using TechShop.Dtos.User;
using TechShop.Helpers.Security.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Security.Authentication;

namespace TechShop.BBL
{
    public class AuthenticationService : IAuthenticationService
    {

        private readonly IUserDao _userDao;
        private readonly IRoleDao _roleDao;
        private readonly IJwtAuthManager _jwtAuthManager;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthenticationService(IUserDao userDao, IJwtAuthManager jwtAuthManager, IPasswordHasher<User> passwordHasher, IRoleDao roleDao)
        {
            _userDao = userDao;
            _jwtAuthManager = jwtAuthManager;
            _passwordHasher = passwordHasher;
            _roleDao = roleDao;
        }

        public async Task<string> AuthenticateAsync(LoginRequest loginRequest)
        {
            var user = await _userDao.FindByUsernameAsync(loginRequest.Username);

            if (user is null || !VerifyPassword(loginRequest.Password, user.Password, user))
            {
                throw new AuthenticationException("Username or password is incorrect.");
            }
            var role = await _roleDao.FindByIdAsync(user.RoleId);
            return _jwtAuthManager.GenerateToken(new UserWithRoleRequest {Role= role.Name, Username = loginRequest.Username});
        }

        private bool VerifyPassword(string inputPassword, string storedHashedPassword, User user)
        {
            var verificationResult = _passwordHasher.VerifyHashedPassword(user, storedHashedPassword, inputPassword);
            return verificationResult == PasswordVerificationResult.Success;
        }
    }
}
