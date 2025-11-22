using Discover.DNAustria.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();                // classic controllers
builder.Services.AddEndpointsApiExplorer();      // required for Swagger metadata
builder.Services.AddSwaggerGen();                // generate Swagger/OpenAPI

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       "Host=localhost;Database=discoverdnaustria;Username=appuser;Password=password";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// Automatic DB migrate and seed on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    SeedData.EnsureSeeded(db);
}

// Enable Swagger in all environments
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DiscoverDNAustria API V1");
});

// Configure middleware
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers(); // map your MVC controllers

app.Run();