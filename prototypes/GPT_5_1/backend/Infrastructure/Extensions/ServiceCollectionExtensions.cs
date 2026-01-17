using Application.Interfaces;
using AutoMapper;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var cs = configuration.GetConnectionString("Default") ?? configuration["DATABASE_CONNECTION"];
        services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(cs));
        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddAutoMapper(typeof(Application.Mapping.AppProfile));
        services.AddScoped<Application.Interfaces.IEventService, EventService>();
        services.AddScoped<Application.Interfaces.IContactService, ContactService>();
        services.AddScoped<Application.Interfaces.IOrganizationService, OrganizationService>();
        services.AddScoped<Application.Interfaces.IEventImportService, EventImportService>();
        return services;
    }
}
