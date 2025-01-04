using BLL.Configuration;
using DAL.Configurations;
using DAL.Data;
using DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        Task<LoginResponseDto> Refresh(string refreshToken);
        Task<UserPublicModelDto> GetProfile(string email);
        Task UpdateProfile(string email, UserEditModelDto model);
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
                AccessToken = await GenerateAccessToken(user),
                RefreshToken = await GenerateRefreshToken(user)
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

        public async Task<LoginResponseDto> Refresh(string refreshToken)
        {
            var storedToken = await _context.UserRefreshTokens
                .FirstOrDefaultAsync(x => x.Token == refreshToken && x.ExpiryDateTime > DateTime.UtcNow);

            if (storedToken == null)
                throw new SecurityTokenException("Invalid or expired refresh token");

            var user = await _userManager.FindByIdAsync(storedToken.UserId.ToString());
            if (user == null)
                throw new KeyNotFoundException($"User with id = {storedToken.UserId} does not found!");

            storedToken.ExpiryDateTime = DateTime.UtcNow.AddMinutes(-5);
            await _context.SaveChangesAsync();

            return new LoginResponseDto()
            {
                AccessToken = await GenerateAccessToken(user),
                RefreshToken = await GenerateRefreshToken(user)
            };
        }

        public async Task<UserPublicModelDto> GetProfile(string email) 
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with email = {email} does not exist!");
            }
            return new UserPublicModelDto
            {
                Name = user.Name,
                Email = user.Email,
                BirthDate = user.BirthDate,
            };
        }

        public async Task UpdateProfile(string email, UserEditModelDto model) 
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with email = {email} does not exist!");
            }

            user.Name = model.Name;
            user.BirthDate = model.BirthDate;
            user.SocialNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception("Failed to update the profile.");
            }
        }

        private async Task<string> GenerateRefreshToken(User user)
        {
            var refreshToken = Guid.NewGuid();
            var refreshTokenString = Convert.ToBase64String(refreshToken.ToByteArray());

            var userRefreshToken = new UserRefreshToken
            {
                UserId = user.Id,
                Token = refreshTokenString,
                ExpiryDateTime = DateTime.UtcNow.AddDays(_jwtTokenSettings.RefreshTokenExpiryTimeInDays)
            };

            _context.UserRefreshTokens.Add(userRefreshToken);
            await _context.SaveChangesAsync();
            return refreshTokenString;
        }



    }
}
