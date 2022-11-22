using DDApp.Common.Exceptions;
using DDApp.Common.Exceptions.Authorization;
using DDApp.Common.Exceptions.Forbidden;
using DDApp.Common.Exceptions.UnsopportedMediaType;
using Microsoft.IdentityModel.Tokens;
using DDApp.Common.Exceptions.UnprocessableEntity;

namespace DDApp.API.Middlewares
{
    public class ErrorModdleware
    {
        private readonly RequestDelegate _next;

        public ErrorModdleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(NotFoundException ex)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsJsonAsync(ex.Message);
            }
            catch(ForbiddenException ex)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsJsonAsync(ex.Message);
            }
            catch(SecurityTokenException ex)
            {
                context.Response.StatusCode = 498;
                await context.Response.WriteAsJsonAsync(ex.Message);
            }
            catch (AuthorizationException ex)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(ex.Message);
            }
            catch(UnsopportedMediaTypeException ex)
            {
                context.Response.StatusCode = 415;
                await context.Response.WriteAsJsonAsync(ex.Message);
            }
            catch(UnprocessableEntityException ex)
            {
                context.Response.StatusCode = 422;
                await context.Response.WriteAsJsonAsync(ex.Message);
            }
        }
    }

    public static class ErrorModdlewareExtensions
    {
        public static IApplicationBuilder UserGlobalErrorWrapper(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorModdleware>();
        }
    }
}
