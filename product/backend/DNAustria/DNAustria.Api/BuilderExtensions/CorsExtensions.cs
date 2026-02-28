namespace DNAustria.Api.BuilderExtensions;

public static class CorsExtensions
{
    private const string PolicyName = "cors_policy";

    public static IServiceCollection ConfigureCors(this IServiceCollection collection)
    {
        collection.AddCors(options => options.AddPolicy(PolicyName, builder => builder
            .AllowAnyHeader()
            .AllowAnyOrigin() //TODO: set frontend url here...
            .AllowAnyMethod()));
        return collection;
    }
    
    public static IApplicationBuilder UseCustomCors(this IApplicationBuilder app)
    {
        app.UseCors(PolicyName);
        return app;
    }
}