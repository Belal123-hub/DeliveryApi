using DAL.Data;
using DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;


namespace BLL.Services
{
    public interface IUsersService
    {
        Task Register(UserCreateDto model);
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

    }
}
