using DNAustria.Application.Services;
using DNAustria.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
const string FrontendCorsPolicy = "FrontendCorsPolicy";

// Register Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<OrganizationService>();
builder.Services.AddScoped<ContactService>();
builder.Services.AddScoped<AddressService>();
builder.Services.AddScoped<LocationService>();
builder.Services.AddScoped<EventService>();
builder.Services.AddControllers();
// Allow only local frontend origins for development.
builder.Services.AddCors(options =>
{
	options.AddPolicy(FrontendCorsPolicy, policy =>
	{
		policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
			  .AllowAnyMethod()
			  .AllowAnyHeader()
			  .AllowCredentials();
	});
});


var app = builder.Build();

// Enable Swagger UI in Development environment
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

// Ensure routing is enabled before applying CORS.
app.UseRouting();
app.UseCors(FrontendCorsPolicy);

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapControllers();

app.Run();

public partial class Program { }
