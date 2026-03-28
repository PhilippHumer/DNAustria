using DNAustria.Api.BuilderExtensions;
using DNAustria.Dal.Data;
using DNAustria.Logic;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.ActivateSerilog();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.RegisterServices();

// Register LLM service for OpenAI integration
builder.Services.AddScoped<ILLMLogic, LLMLogic>();

builder.Services.ConfigureCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    Log.Information("OpenAPI-doc reachable at http://localhost:5001/scalar");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCustomCors();

// Seed database in development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbSeeder.SeedAsync(dbContext);
}

app.Run();