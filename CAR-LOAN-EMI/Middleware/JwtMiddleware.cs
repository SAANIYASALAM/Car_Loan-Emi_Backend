using CAR_LOAN_EMI.Helpers;
using CAR_LOAN_EMI.Repositories.Interfaces;
using System.Security.Claims;

namespace CAR_LOAN_EMI.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, JwtHelper jwtHelper, IUserRepository userRepository)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                var claimsPrincipal = jwtHelper.ValidateToken(token);
                if (claimsPrincipal != null)
                {
                    var userIdClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);
                    if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                    {
                        var user = await userRepository.GetByIdAsync(userId);
                        if (user != null)
                        {
                            context.Items["User"] = user;
                        }
                    }
                }
            }

            await _next(context);
        }
    }
}
