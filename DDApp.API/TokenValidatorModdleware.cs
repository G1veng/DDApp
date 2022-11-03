using DDApp.API.Services;

namespace DDApp.API
{
    public class TokenValidatorModdleware
    {
        private readonly RequestDelegate _next;

        public TokenValidatorModdleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserService userService)
        {
            var isOk = true;
            var sessionIdString = context.User.Claims.FirstOrDefault(x => x.Type == "sessionId")?.Value;
            
            if(Guid.TryParse(sessionIdString, out var sessionId))
            {
                var session = await userService.GetSessionById(sessionId);

                if (!session.IsActive)
                {
                    isOk = false;
                    context.Response.Clear();
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Session is not active");
                }
            }

            if (isOk)
            {
                await _next(context);
            }
            
        }
    }

    public static class TokenValidatorModdlewareExtensions
    {
        public static IApplicationBuilder UseTokenValidator(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenValidatorModdleware>();
        }
    }
}
