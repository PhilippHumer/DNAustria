using DNAustria.Dal.Data;
using Microsoft.EntityFrameworkCore;

namespace DNAustria.Api.BuilderExtensions;

public static class ServiceExtensions
{
    public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddScoped<Logic.IContactsLogic, Logic.ContactsLogic>();
        return builder;
    }
}