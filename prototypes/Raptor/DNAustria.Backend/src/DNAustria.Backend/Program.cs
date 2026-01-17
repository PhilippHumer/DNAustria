using System;
using System.Linq;
using DNAustria.Backend.Data;
using DNAustria.Backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using DNAustria.Backend.Mapping;

var builder = WebApplication.CreateBuilder(args);

// Configure default URLs. Prefer ASPNETCORE_URLS if provided so ports can be controlled via launchSettings.json or Docker.
// Normalize host placeholders to 0.0.0.0 so Kestrel listens on all interfaces when appropriate.
var urls = new List<string>();
var envUrls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
if (!string.IsNullOrEmpty(envUrls))
{
    urls.AddRange(envUrls.Split(';', StringSplitOptions.RemoveEmptyEntries)
        .Select(u =>
        {
            var uriStr = u.Trim();
            try
            {
                var uri = new Uri(uriStr);
                var host = uri.Host;
                if (host == "+" || host == "0.0.0.0" || host == "localhost")
                {
                    host = "0.0.0.0";
                }
                var builderUri = new UriBuilder(uri) { Host = host };
                return builderUri.Uri.ToString().TrimEnd('/');
            }
            catch
            {
                // Fallback replacements for non-parseable strings
                return uriStr.Replace("localhost", "0.0.0.0").Replace("+", "0.0.0.0");
            }
        }));
}
else
{
    // Default behavior: HTTP 5000; optionally HTTPS 5001 when enabled via env vars/cert config.
    urls.Add("http://0.0.0.0:5000");
    var enableHttps = Environment.GetEnvironmentVariable("ENABLE_HTTPS") == "true" ||
                      !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Path")) ||
                      !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_HTTPS_PORT"));
    if (enableHttps)
    {
        urls.Add("https://0.0.0.0:5001");
    }
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
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    // Only apply migrations for relational database providers (skips InMemory used in tests)
    if (db.Database.IsRelational())
    {
        db.Database.Migrate();

        // Determine whether to seed sample data. Default: true (preserves existing behavior).
        // Set SEED_SAMPLE_DATA=false (env var) or SeedSampleData=false (config) to disable.
        var seedEnv = Environment.GetEnvironmentVariable("SEED_SAMPLE_DATA");
        var seedConfig = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>()["SeedSampleData"];
        var seedValue = seedEnv ?? seedConfig;
        var doSeed = true;
        if (!string.IsNullOrEmpty(seedValue) && !bool.TryParse(seedValue, out doSeed))
        {
            doSeed = true;
        }

        if (doSeed)
        {
            try
            {
                DbInitializer.SeedAsync(db).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }
        else
        {
            logger.LogInformation("Database seeding skipped (SEED_SAMPLE_DATA or SeedSampleData set to false).");
        }
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