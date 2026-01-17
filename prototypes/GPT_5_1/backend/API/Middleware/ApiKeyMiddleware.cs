using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection; // hinzugefügt für GetService
using System.Linq;

namespace API.Middleware;

public class ApiKeyMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Public Export ohne Key
        if (context.Request.Path.StartsWithSegments("/server/api/public"))
        {
            await next(context); return;
        }
        // FIX: statt context.Request.Services -> context.RequestServices
        var config = context.RequestServices.GetService<IConfiguration>();
        var requiredKey = config?["INTERNAL_API_KEY"];
        if (string.IsNullOrWhiteSpace(requiredKey))
        {
            await next(context); return; // kein Key konfiguriert -> offen
        }
        var provided = context.Request.Headers["X-API-Key"].FirstOrDefault();
        if (provided == requiredKey)
        {
            await next(context); return;
        }
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("API key missing or invalid");
    }
}
