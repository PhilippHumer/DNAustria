using DNAustria.Api.BuilderExtensions;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.ActivateSerilog();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.RegisterServices();

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

app.Run();