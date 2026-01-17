using DNAustria.Backend.Data;
using DNAustria.Backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using DNAustria.Backend.Mapping;

var builder = WebApplication.CreateBuilder(args);

// Configure default URLs. Always listen on HTTP port 5000. Only enable HTTPS port 5001
// when a certificate is configured or HTTPS is explicitly enabled via env var.
var urls = new List<string> { "http://0.0.0.0:5000" };
var enableHttps = Environment.GetEnvironmentVariable("ENABLE_HTTPS") == "true" ||
                  !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Path")) ||
                  !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_HTTPS_PORT"));
if (enableHttps)
{
    urls.Add("https://0.0.0.0:5001");
}
builder.WebHost.UseUrls(urls.ToArray());

// Configuration
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        // Omit null values from JSON output so optional fields are not serialized as null, which
        // can cause strict JSON schema validators to fail when a field is present but null.
        opts.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DNAustria Backend API", Version = "v1" });
});

// DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Host=localhost;Database=dnaustria;Username=postgres;Password=postgres";
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(connectionString));

// Services
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<ILLMService, StubLLMService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();

// Apply pending migrations at startup (optional)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // Only apply migrations for relational database providers (skips InMemory used in tests)
    if (db.Database.IsRelational())
    {
        db.Database.Migrate();
    }

    // Seed sample data (idempotent)
    try
    {
        DbInitializer.SeedAsync(db).GetAwaiter().GetResult();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();