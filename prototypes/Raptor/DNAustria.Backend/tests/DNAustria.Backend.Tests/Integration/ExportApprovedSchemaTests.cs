using System.Text.Json;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DNAustria.Backend.Tests.Integration;

public class ExportApprovedSchemaTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public ExportApprovedSchemaTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public async Task ExportApproved_JsonTypes_Are_Correct()
    {
        var factory = _factory.WithWebHostBuilder(b => b.ConfigureServices(services =>
        {
            // Use InMemory DB for this integration test to avoid depending on local Postgres schema/migrations
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(Microsoft.EntityFrameworkCore.DbContextOptions<DNAustria.Backend.Data.AppDbContext>));
            if (descriptor != null) services.Remove(descriptor);
            services.AddDbContext<DNAustria.Backend.Data.AppDbContext>(opt => opt.UseInMemoryDatabase("test-export-approved"));
        }).UseSetting("environment", "Development"));
        var client = factory.CreateClient();

        // Ensure there's at least one approved event (don't rely on DB seeding in tests)
        var now = DateTime.UtcNow;
        var create = new {
            title = "IntegrationExportTest",
            description = "desc",
            datestart = now,
            dateend = now.AddHours(1),
            status = 1 // EventStatus.Approved
        };
        var json = System.Text.Json.JsonSerializer.Serialize(create);
        var createResp = await client.PostAsync("/api/events", new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json"));
        createResp.EnsureSuccessStatusCode();

        var resp = await client.GetAsync("/api/events/export/approved");
        resp.EnsureSuccessStatusCode();

        using var doc = await JsonDocument.ParseAsync(await resp.Content.ReadAsStreamAsync());
        var root = doc.RootElement;
        Assert.True(root.TryGetProperty("events", out var events));
        Assert.True(events.GetArrayLength() > 0);
        var ev = events[0];

        // event_link must be a valid HTTP(S) URI (string matching ^https?://.*$)
        Assert.True(ev.TryGetProperty("event_link", out var link) && link.ValueKind == JsonValueKind.String, "event_link must be a string");
        var linkStr = link.GetString();
        Assert.True(!string.IsNullOrWhiteSpace(linkStr), "event_link must not be empty");
        Assert.True(System.Text.RegularExpressions.Regex.IsMatch(linkStr!, "^https?://.*$"), "event_link must match ^https?://.*$");
        Assert.True(Uri.TryCreate(linkStr, UriKind.Absolute, out var uri) && (uri.Scheme == "http" || uri.Scheme == "https"), "event_link must be a valid absolute URI with http/https scheme");

        // age fields, if present, must be numbers
        if (ev.TryGetProperty("event_age_minimum", out var ageMin)) Assert.True(ageMin.ValueKind == JsonValueKind.Number);
        if (ev.TryGetProperty("event_age_maximum", out var ageMax)) Assert.True(ageMax.ValueKind == JsonValueKind.Number);

        // address state, if present, must be one of allowed
        var allowed = new[] { "Burgenland","Kärnten","Niederösterreich","Oberösterreich","Salzburg","Steiermark","Tirol","Vorarlberg","Wien" };
        if (ev.TryGetProperty("event_address_state", out var state)) Assert.True(state.ValueKind == JsonValueKind.String && allowed.Contains(state.GetString() ?? string.Empty, StringComparer.Ordinal));

        // event_mint_region, if present, must be integer within 1..28
        if (ev.TryGetProperty("event_mint_region", out var mr))
        {
            Assert.True(mr.ValueKind == JsonValueKind.Number);
            Assert.True(mr.TryGetInt32(out var mi) && mi >= 1 && mi <= 28);
        }

        // group_id, if present, must be string
        if (ev.TryGetProperty("group_id", out var gid)) Assert.True(gid.ValueKind == JsonValueKind.String);
    }
}