using BLL.Services;
using System.IdentityModel.Tokens.Jwt;

namespace DeliveryWebApi.MiddleWares
{
    public class TokenValidationMiddleWare
    {
        private readonly RequestDelegate _next;
        public TokenValidationMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request
                .Headers["Authorization"].ToString()
                .Replace("Bearer ", "");
            if (!string.IsNullOrWhiteSpace(token))
            {
                var userId = ExtractUserIdFromToken(token);
                if (userId != null)
                {
                    var tokenService = context.RequestServices.GetRequiredService<ITokenService>();
                    var isUserLoggedOut = await tokenService.IsUserLoggedOut(userId.Value);

                    if (isUserLoggedOut)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync($"User with Id = {userId} was logged out");
                        return;
                    }

                }
            }

            await _next(context);
        }

        private Guid? ExtractUserIdFromToken(string token)
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;

            var userIdClaim = jwtToken?.Claims.FirstOrDefault(x => x.Type == "Id");
            return userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId) ? userId : null;
        }
    }
}
