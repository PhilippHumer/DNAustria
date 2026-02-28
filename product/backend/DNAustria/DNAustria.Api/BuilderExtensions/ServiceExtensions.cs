using DNAustria.Dal.Data;
using DNAustria.Logic.LocationsService;
using Microsoft.EntityFrameworkCore;

namespace DNAustria.Api.BuilderExtensions;

public static class ServiceExtensions
{
    public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddScoped<ILocationsService, LocationsService>();
        //TODO: register your services here...
        return builder;
    }
}