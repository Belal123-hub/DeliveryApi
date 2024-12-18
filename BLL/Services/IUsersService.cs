using BLL.Configuration;
using DAL.Configurations;
using DAL.Data;
using DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


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
        private readonly JwtBearerTokenSettings _jwtTokenSettings;
        private readonly ApplicationDbContext _context;

        public UsersService(UserManager<User> userManager,IOptions<JwtBearerTokenSettings> options, ApplicationDbContext context )
        {
            _userManager = userManager;
            _context = context;
            _jwtTokenSettings = options.Value;
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
                AcessToken = await GenerateAccessToken(user)
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

        private async Task<string> GenerateAccessToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtTokenSettings.secretKey);

            var isAdmin = await _userManager.IsInRoleAsync(user, ApplicationRoleNames.Administrator);

            var descriptior = new SecurityTokenDescriptor
            {
                Audience = _jwtTokenSettings.Audience,
                Issuer = _jwtTokenSettings.Issuer,
                Expires = DateTime.UtcNow.AddSeconds(_jwtTokenSettings.AccessTokenExpiryTimeInSeconds),
                Subject = new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim("Id", user.Id.ToString()),
                    new Claim(ClaimTypes.Role, isAdmin ? ApplicationRoleNames.Administrator : ApplicationRoleNames.User),
                }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(descriptior);
            var userToken = tokenHandler.WriteToken(token);
            return userToken;
        }

    }
}
