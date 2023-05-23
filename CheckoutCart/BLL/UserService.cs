using AutoMapper;
using TechShop.BBL.Interface;
using TechShop.DAL;
using TechShop.DAL.Interface;
using TechShop.Domain;
using TechShop.Dtos.User;
using TechShop.Helpers.Exceptions;
using TechShop.Helpers.Extensions;
using Microsoft.AspNetCore.Identity;
using System.Security.Authentication;

namespace TechShop.BBL
{
    public class UserService : IUserService
    {
        private readonly IUserDao _userDao;
        private readonly IMapper _mapper;
        private readonly IRoleDao _roleDao;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(IUserDao userDao, IMapper mapper, IRoleDao roleDao, IPasswordHasher<User> passwordHasher)
        {
            _userDao = userDao;
            _mapper = mapper;
            _roleDao = roleDao;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserCreateResponse> GetUserByUsernameAsync(string username)
        {
            var user = await _userDao.FindByUsernameAsync(username);
            if (user is null)
            {
                throw new UserNotFoundException($"User with username {username} not found");
            }

            var role = await _roleDao.FindByIdAsync(user.RoleId);

            var userResponse = _mapper.Map<UserCreateResponse>(user);
            userResponse.Role = role.Name; 
            return userResponse;
        }

        public async Task<UserCreateResponse> CreateUserAsync(UserCreateRequest userDto)
        {

            if (!userDto.Password.IsValidPassword())
            {
                throw new ArgumentException("Password does not meet the required rules. It must be at least 8 characters long, include at least one lowercase letter, one uppercase letter, one digit, and one special character.");
            }

            if (userDto.Password != userDto.ConfirmPassword)
            {
                throw new ArgumentException("Passwords do not match.");
            }

            var role = await _roleDao.FindByNameAsync(userDto.Role);
            if (role is null)
            {
                throw new ArgumentException("Invalid role specified.");
            }

            

            var user = _mapper.Map<User>(userDto);
            user.RoleId = role.Id;

            var userCreated = await _userDao.CreateAsync(user);

            var userResponse = _mapper.Map<UserCreateResponse>(userCreated);
            userResponse.Role = role.Name;
            return  userResponse;
        }

        public async Task<bool> UpdateUserAsync(UserUpdateRequest userDto,  string username)
        {
            var user = await _userDao.FindByUsernameAsync(username);
            if (user is null)
            {
                throw new UserNotFoundException($"User with username {username} not found");
            }


            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, userDto.Password);
            if (verificationResult != PasswordVerificationResult.Success)
            {
                throw new AuthenticationException("Current password is incorrect.");
            }

            if (!userDto.NewPassword.IsValidPassword())
            {
                throw new ArgumentException("Password does not meet the required rules.");
            }

            if (userDto.NewPassword != userDto.ConfirmNewPassword)
            {
                throw new ArgumentException("New password and confirmation do not match.");
            }

            var role = await _roleDao.FindByNameAsync(userDto.Role);
            if (role is null)
            {
                throw new ArgumentException($"Role {userDto.Role} does not exist.");
            }

            user.RoleId = role.Id;
            user.Password = userDto.NewPassword;

            var isSucess = await _userDao.UpdateAsync(user);

            return isSucess;
        }
    }
}

