
using Serilog;

namespace DNAustria.Api.BuilderExtensions;

public static class SerilogExtensions
{
    public static WebApplicationBuilder ActivateSerilog(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File("logs/log.txt",  rollingInterval: RollingInterval.Day)
            .CreateLogger();
        builder.Host.UseSerilog();
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();
        return builder;
    }
}