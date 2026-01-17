using System;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using System.Threading.Tasks;

public class PublicExportTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly WebApplicationFactory<Program> _factory;
    public PublicExportTests(CustomWebApplicationFactory factory){ _factory = factory; }

    private bool Skip => (Environment.GetEnvironmentVariable("INTEGRATION_TESTS") ?? "false").ToLower() != "true";

    [Fact]
    public async Task PublicExport_ReturnsApprovedEvents()
    {
        if (Skip) return; // Skip wenn nicht aktiviert
        var client = _factory.CreateClient();
        var resp = await client.GetAsync("/server/api/public/events");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        var json = await resp.Content.ReadAsStringAsync();
        Assert.Contains("event_title", json); // Minimaler Check
    }

    [Fact]
    public async Task Health_ReturnsOk()
    {
        if (Skip) return;
        var client = _factory.CreateClient();
        var resp = await client.GetAsync("/server/api/public/health");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        var json = await resp.Content.ReadAsStringAsync();
        Assert.Contains("status", json);
    }
}
