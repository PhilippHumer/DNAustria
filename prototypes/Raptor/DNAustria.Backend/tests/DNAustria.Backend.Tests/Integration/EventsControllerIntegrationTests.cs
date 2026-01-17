using System;
using System.Net.Http.Json;
using System.Linq;
using System.Threading.Tasks;
using DNAustria.Backend.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DNAustria.Backend.Tests.Integration;

public class EventsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public EventsControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove existing AppDbContext or DbContextOptions registrations (to avoid using PostgreSQL in tests)
                var descriptors = services.Where(d => d.ServiceType == typeof(DNAustria.Backend.Data.AppDbContext) || d.ServiceType == typeof(DbContextOptions<DNAustria.Backend.Data.AppDbContext>)).ToList();
                foreach (var d in descriptors) services.Remove(d);

                // Add in-memory db
                services.AddDbContext<DNAustria.Backend.Data.AppDbContext>(options => options.UseInMemoryDatabase("test_integration_db"));
            });
        });
    }

    [Fact]
    public async Task PostImport_ReturnsParsedEvent()
    {
        var client = _factory.CreateClient();
        var req = new { content = "<h1>Imported Event</h1>\n12.01.2026", isHtml = true };
        var resp = await client.PostAsJsonAsync("/api/events/import", req);
        resp.EnsureSuccessStatusCode();
        var dto = await resp.Content.ReadFromJsonAsync<EventCreateDto>();
        Assert.NotNull(dto);
        Assert.Contains("Imported Event", dto.Title);
    }

    [Fact]
    public async Task CreateEvent_ThenGetById_ReturnsEvent()
    {
        var client = _factory.CreateClient();

        var dto = new EventCreateDto { Title = "ITest", Description = "desc", DateStart = DateTime.UtcNow, DateEnd = DateTime.UtcNow.AddHours(1) };
        var createResp = await client.PostAsJsonAsync("/api/events", dto);
        createResp.EnsureSuccessStatusCode();
        var created = await createResp.Content.ReadFromJsonAsync<DNAustria.Backend.Dtos.EventDetailDto>();
        Assert.NotNull(created);

        var get = await client.GetAsync($"/api/events/{created.Id}");
        get.EnsureSuccessStatusCode();
        var got = await get.Content.ReadFromJsonAsync<DNAustria.Backend.Dtos.EventDetailDto>();
        Assert.Equal("ITest", got.Title);
    }
}