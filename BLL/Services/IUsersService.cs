using DAL.Data;
using DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;


namespace BLL.Services
{
    public interface IUsersService
    {
        Task Register(UserCreateDto model);
        Task<LoginResponseDto> Login(LoginCredentialsDto model);
    }
    public class UsersService : IUsersService
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public UsersService(UserManager<User> userManager, ApplicationDbContext context )
        {
            _userManager = userManager;
            _context = context;
        }
        public async Task Register(UserCreateDto model)
        {
            var existing = await _userManager.FindByEmailAsync(model.Email);
            if (existing != null)
            {
                throw new ArgumentException("User with same email already exists");
            }

            var identityUser = new User()
            {
                Name = model.Name,
                Email = model.Email,
                BirthDate = DateOnly.FromDateTime(model.BirthDate),
                UserName = model.Email
            };

            var result = await _userManager.CreateAsync(identityUser, model.Password);
            if (!result.Succeeded)
            {
                throw new Exception($"Some errors during creating user! Data: {result.Errors}");
            }
        }

        public async Task<LoginResponseDto> Login(LoginCredentialsDto model) 
        {
            var user = await ValidateUser(model);
            var result = new LoginResponseDto
            {
                // here we should implement accesstoken and refreshtoken
            };
            return result;
        }

        private async Task<User> ValidateUser(LoginCredentialsDto credentials)
        {
            var identityUser = await _userManager.FindByEmailAsync(credentials.Email);
            if (identityUser != null)
            {
                var result = _userManager.PasswordHasher.VerifyHashedPassword(identityUser, identityUser.PasswordHash,
                    credentials.Password);
                if (result == PasswordVerificationResult.Success)
                {
                    return identityUser;
                }
            }

            throw new ArgumentException("Login data was incorrect");
        }

    }
}
