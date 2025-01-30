using DAL.Data;
using Microsoft.EntityFrameworkCore;

public interface ITokenService
{
    Task Logout(Guid userId, string token);
    Task<bool> IsUserLoggedOut(Guid userId);
    Task <bool> IsTokenBlacklisted(string token);
}

public class TokenService : ITokenService
{
    private readonly ApplicationDbContext _context;
    public TokenService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Logout(Guid userId, string token)
    {
        // Invalidate refresh tokens
        var refreshTokens = await _context.UserRefreshTokens.Where(x => x.UserId == userId).ToListAsync();
        refreshTokens.ForEach(x => x.ExpiryDateTime = DateTime.UtcNow.AddMinutes(-5));

        // Add the token to the blacklist
        _context.BlacklistedTokens.Add(new BlacklistedToken
        {
            Token = token,
            ExpiryDateTime = DateTime.UtcNow.AddHours(1) // Set an expiry time for the blacklisted token
        });

        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsUserLoggedOut(Guid userId)
    {
        return await _context.LogoutUsers.AnyAsync(x => x.Identifier == userId && !x.DeleteDateTime.HasValue);
    }

    public async Task<bool> IsTokenBlacklisted(string token)
    {
        return await _context.BlacklistedTokens.AnyAsync(x => x.Token == token && x.ExpiryDateTime > DateTime.UtcNow);
    }
}