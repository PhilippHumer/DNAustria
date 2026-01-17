using System.Linq;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Entferne die echte Npgsql-Registration
            var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null) services.Remove(descriptor);
            // Entferne IAppDbContext direkte Registrierung (falls vorhanden), wird durch Scoped neu erstellt
            var appCtx = services.FirstOrDefault(d => d.ServiceType == typeof(Application.Interfaces.IAppDbContext));
            if (appCtx != null) services.Remove(appCtx);

            services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("Tests"));
            services.AddScoped<Application.Interfaces.IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
            // Registrierung einer SystemClock für alten Handler-Konstruktor
            services.AddSingleton<ISystemClock, SystemClock>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = "Test";
                options.DefaultScheme = "Test";
            }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
            services.AddAuthorization();

            // Build ServiceProvider und seeden
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
            Infrastructure.Persistence.DataSeeder.SeedAsync(db).GetAwaiter().GetResult();
        });
    }
}

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    #pragma warning disable CS0618 // ISystemClock veraltet aber von dieser Runtime erwartet
    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock) { }
    #pragma warning restore CS0618

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "test"),
            new Claim(ClaimTypes.Name, "test"),
            new Claim(ClaimTypes.Role, "Editor"),
            new Claim(ClaimTypes.Role, "Admin"),
        };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
