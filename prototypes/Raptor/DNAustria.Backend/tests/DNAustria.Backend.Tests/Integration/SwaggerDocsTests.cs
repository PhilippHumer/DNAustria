using System.Net.Http.Json;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DNAustria.Backend.Tests.Integration;

public class SwaggerDocsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public SwaggerDocsTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public async Task SwaggerDocument_Contains_ExportApprovedPath()
    {
        var factory = _factory.WithWebHostBuilder(builder => builder.UseSetting("environment", "Development"));
        var client = factory.CreateClient();
        var resp = await client.GetAsync("/swagger/v1/swagger.json");
        resp.EnsureSuccessStatusCode();

        using var doc = await JsonDocument.ParseAsync(await resp.Content.ReadAsStreamAsync());
        var root = doc.RootElement;
        Assert.True(root.TryGetProperty("paths", out var paths), "Swagger JSON missing 'paths' property");
        if (!paths.TryGetProperty("/api/events/export/approved", out _))
        {
            var available = string.Join(", ", paths.EnumerateObject().Select(p => p.Name));
            Assert.True(false, $"Swagger paths does not contain '/api/events/export/approved'. Available: {available}");
        }
    }
}