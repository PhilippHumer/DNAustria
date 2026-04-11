using DNAustria.Api.Authentication;
using DNAustria.Dal.Data;
using DNAustria.Logic;
using DNAustria.Logic.Events;
using DNAustria.Logic.LocationsService;
using DNAustria.Logic.Organizations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DNAustria.Api.BuilderExtensions;

public static class ServiceExtensions
{
    public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.Configure<AuthenticationOptions>(builder.Configuration.GetSection(AuthenticationOptions.SectionName));
        builder.Services.Configure<LdapOptions>(builder.Configuration.GetSection(LdapOptions.SectionName));
        builder.Services.Configure<MockAuthenticationOptions>(builder.Configuration.GetSection(MockAuthenticationOptions.SectionName));
        builder.Services.AddScoped<ILocationsService, LocationsService>();
        builder.Services.AddScoped<IOrganizationsLogic, OrganizationsLogic>();
        //TODO: register your services here...
        builder.Services.AddScoped<IEventLogic, EventLogic>();
        builder.Services.AddScoped<Logic.IContactsLogic, Logic.ContactsLogic>();
        builder.Services.AddScoped<IEventTracker, EventTracker>();
        builder.Services.AddScoped<IEventExtractionService, EventExtractionService>();
        builder.Services.AddScoped<LdapAuthenticationService>();
        builder.Services.AddScoped<MockAuthenticationService>();
        builder.Services.AddScoped<IAuthenticationService>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<AuthenticationOptions>>().Value;
            return string.Equals(options.Mode, AuthenticationMode.Mock, StringComparison.OrdinalIgnoreCase)
                ? serviceProvider.GetRequiredService<MockAuthenticationService>()
                : serviceProvider.GetRequiredService<LdapAuthenticationService>();
        });
        return builder;
    }
}
