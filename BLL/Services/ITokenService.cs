using DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public interface ITokenService
    {
        Task Logout(Guid userId);
        Task<bool> IsUserLoggedOut(Guid userId);
    }

    public class TokenService : ITokenService
    {
        private readonly ApplicationDbContext _context;
        public TokenService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task Logout(Guid userId)
        {
            var refreshTokens = await _context.UserRefreshTokens.Where(x => x.UserId == userId).ToListAsync();
            refreshTokens.ForEach(x => x.ExpiryDateTime = DateTime.UtcNow.AddMinutes(-5));
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsUserLoggedOut(Guid userId)
        {
            return await _context.LogoutUsers.AnyAsync(x => x.Identifier == userId && !x.DeleteDateTime.HasValue);
        }
    }
}
