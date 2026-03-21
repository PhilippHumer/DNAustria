using DNAustria.Application.Interfaces;
using DNAustria.Infrastructure.Persistence;
using DNAustria.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DNAustria.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));

        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IContactRepository, ContactRepository>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();

        return services;
    }
}
