namespace DNAustria.Api.BuilderExtensions;

public static class CorsExtensions
{
    private const string PolicyName = "cors_policy";

    public static IServiceCollection ConfigureCors(this IServiceCollection collection, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("Frontend:AllowedOrigins").Get<string[]>() ?? ["http://localhost:4200"];

        collection.AddCors(options => options.AddPolicy(PolicyName, builder => builder
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins(allowedOrigins)
            .AllowCredentials()));
        return collection;
    }
    
    public static IApplicationBuilder UseCustomCors(this IApplicationBuilder app)
    {
        app.UseCors(PolicyName);
        return app;
    }
}
