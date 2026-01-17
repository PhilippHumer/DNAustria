using System;
using System.Net;
using System.Text.Json;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class EventsCrudTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly WebApplicationFactory<Program> _factory;
    public EventsCrudTests(CustomWebApplicationFactory factory){ _factory = factory; }
    private bool Skip => (Environment.GetEnvironmentVariable("INTEGRATION_TESTS") ?? "false").ToLower() != "true";

    [Fact]
    public async Task Create_List_Delete_Event_Works()
    {
        if (Skip) return;
        var client = _factory.CreateClient();
        // Create
        var dto = new EventDto{ Title="Test Event", Description="Desc" };
        var json = JsonSerializer.Serialize(dto);
        var respCreate = await client.PostAsync("/server/api/events", new StringContent(json, Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.Created, respCreate.StatusCode);
        var createdJson = await respCreate.Content.ReadAsStringAsync();
        Assert.Contains("Test Event", createdJson);
        // Extract Id
        var doc = JsonDocument.Parse(createdJson);
        var id = doc.RootElement.GetProperty("id").GetGuid();
        // List
        var respList = await client.GetAsync("/server/api/events?page=1&pageSize=10");
        Assert.Equal(HttpStatusCode.OK, respList.StatusCode);
        var listJson = await respList.Content.ReadAsStringAsync();
        Assert.Contains("Test Event", listJson);
        // Delete
        var respDelete = await client.DeleteAsync($"/server/api/events/{id}");
        Assert.Equal(HttpStatusCode.NoContent, respDelete.StatusCode);
    }
}
