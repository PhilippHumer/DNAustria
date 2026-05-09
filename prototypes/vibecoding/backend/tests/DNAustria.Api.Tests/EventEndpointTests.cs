using System.Net;
using System.Net.Http.Json;
using DNAustria.Application.DTOs;
using DNAustria.Api.Tests.Infrastructure;

namespace DNAustria.Api.Tests;

public class EventEndpointTests : IClassFixture<EventApiFactory>
{
    private readonly HttpClient _client;

    public EventEndpointTests(EventApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    private static object NewEvent(string? title = null) => new
    {
        title = title ?? $"Event-{Guid.NewGuid()}",
        description = "Test description",
        targetAudience = new[] { 20 },
        topics = new[] { 100 },
        dateStart = "2026-06-01T10:00:00Z",
        dateEnd = "2026-06-01T12:00:00Z",
        classification = "Scheduled",
        fees = false,
        isOnline = true,
        organizationId = Guid.NewGuid()
    };

    [Fact]
    public async Task Create_ReturnsCreated()
    {
        var response = await _client.PostAsJsonAsync("/api/events", NewEvent());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<EventDto>();
        Assert.NotNull(dto);
        Assert.NotEqual(Guid.Empty, dto!.Id);
        Assert.Equal("Draft", dto.Status);
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/events");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsEvent()
    {
        var created = await _client.PostAsJsonAsync("/api/events", NewEvent());
        var dto = await created.Content.ReadFromJsonAsync<EventDto>();

        var response = await _client.GetAsync($"/api/events/{dto!.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<EventDto>();
        Assert.Equal(dto.Id, result!.Id);
    }

    [Fact]
    public async Task GetById_NotFound_Returns404()
    {
        var response = await _client.GetAsync($"/api/events/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_ReturnsOk()
    {
        var created = await _client.PostAsJsonAsync("/api/events", NewEvent());
        var dto = await created.Content.ReadFromJsonAsync<EventDto>();

        var newTitle = $"Event-{Guid.NewGuid()}";
        var response = await _client.PutAsJsonAsync($"/api/events/{dto!.Id}", new
        {
            title = newTitle,
            description = "Updated description",
            targetAudience = new[] { 30 },
            topics = new[] { 200 },
            dateStart = "2026-07-01T10:00:00Z",
            dateEnd = "2026-07-01T12:00:00Z",
            classification = "OnDemand",
            fees = true,
            isOnline = false,
            organizationId = Guid.NewGuid()
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<EventDto>();
        Assert.Equal(newTitle, updated!.Title);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var created = await _client.PostAsJsonAsync("/api/events", NewEvent());
        var dto = await created.Content.ReadFromJsonAsync<EventDto>();

        var response = await _client.DeleteAsync($"/api/events/{dto!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_AlreadyDeleted_Returns404()
    {
        var created = await _client.PostAsJsonAsync("/api/events", NewEvent());
        var dto = await created.Content.ReadFromJsonAsync<EventDto>();

        await _client.DeleteAsync($"/api/events/{dto!.Id}");
        var response = await _client.DeleteAsync($"/api/events/{dto.Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_NotExisting_Returns404()
    {
        var response = await _client.DeleteAsync($"/api/events/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task SoftDeleted_NotReturnedInGetAll()
    {
        var title = $"Event-{Guid.NewGuid()}";
        var created = await _client.PostAsJsonAsync("/api/events", NewEvent(title));
        var dto = await created.Content.ReadFromJsonAsync<EventDto>();

        await _client.DeleteAsync($"/api/events/{dto!.Id}");

        var response = await _client.GetAsync($"/api/events?title={title}");
        var list = await response.Content.ReadFromJsonAsync<List<EventDto>>();

        Assert.DoesNotContain(list!, e => e.Id == dto.Id);
    }

    [Fact]
    public async Task SoftDeleted_GetById_Returns404()
    {
        var created = await _client.PostAsJsonAsync("/api/events", NewEvent());
        var dto = await created.Content.ReadFromJsonAsync<EventDto>();

        await _client.DeleteAsync($"/api/events/{dto!.Id}");

        var response = await _client.GetAsync($"/api/events/{dto.Id}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_EmptyTitle_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/events", new
        {
            title = "",
            description = "Test",
            targetAudience = new[] { 20 },
            topics = new[] { 100 },
            dateStart = "2026-06-01T10:00:00Z",
            dateEnd = "2026-06-01T12:00:00Z",
            classification = "Scheduled",
            fees = false,
            isOnline = true,
            organizationId = Guid.NewGuid()
        });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_TitleTooLong_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/events", new
        {
            title = new string('A', 256),
            description = "Test",
            targetAudience = new[] { 20 },
            topics = new[] { 100 },
            dateStart = "2026-06-01T10:00:00Z",
            dateEnd = "2026-06-01T12:00:00Z",
            classification = "Scheduled",
            fees = false,
            isOnline = true,
            organizationId = Guid.NewGuid()
        });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_InvalidDateRange_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/events", new
        {
            title = $"Event-{Guid.NewGuid()}",
            description = "Test",
            targetAudience = new[] { 20 },
            topics = new[] { 100 },
            dateStart = "2026-06-01T12:00:00Z",
            dateEnd = "2026-06-01T10:00:00Z",
            classification = "Scheduled",
            fees = false,
            isOnline = true,
            organizationId = Guid.NewGuid()
        });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_InvalidClassification_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/events", new
        {
            title = $"Event-{Guid.NewGuid()}",
            description = "Test",
            targetAudience = new[] { 20 },
            topics = new[] { 100 },
            dateStart = "2026-06-01T10:00:00Z",
            dateEnd = "2026-06-01T12:00:00Z",
            classification = "Invalid",
            fees = false,
            isOnline = true,
            organizationId = Guid.NewGuid()
        });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_InvalidTargetAudience_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/events", new
        {
            title = $"Event-{Guid.NewGuid()}",
            description = "Test",
            targetAudience = new[] { 999 },
            topics = new[] { 100 },
            dateStart = "2026-06-01T10:00:00Z",
            dateEnd = "2026-06-01T12:00:00Z",
            classification = "Scheduled",
            fees = false,
            isOnline = true,
            organizationId = Guid.NewGuid()
        });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateStatus_DraftToApproved_ReturnsOk()
    {
        var created = await _client.PostAsJsonAsync("/api/events", NewEvent());
        var dto = await created.Content.ReadFromJsonAsync<EventDto>();

        var response = await _client.PatchAsJsonAsync($"/api/events/{dto!.Id}/status", new { status = "Approved" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<EventDto>();
        Assert.Equal("Approved", updated!.Status);
    }

    [Fact]
    public async Task UpdateStatus_InvalidTransition_ReturnsBadRequest()
    {
        var created = await _client.PostAsJsonAsync("/api/events", NewEvent());
        var dto = await created.Content.ReadFromJsonAsync<EventDto>();

        var response = await _client.PatchAsJsonAsync($"/api/events/{dto!.Id}/status", new { status = "Transferred" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateStatus_NotFound_Returns404()
    {
        var response = await _client.PatchAsJsonAsync($"/api/events/{Guid.NewGuid()}/status", new { status = "Approved" });
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PublicEvents_OnlyReturnsApproved()
    {
        var created = await _client.PostAsJsonAsync("/api/events", NewEvent());
        var dto = await created.Content.ReadFromJsonAsync<EventDto>();

        var publicBefore = await _client.GetAsync("/api/public/events");
        var listBefore = await publicBefore.Content.ReadFromJsonAsync<List<EventDto>>();
        Assert.DoesNotContain(listBefore!, e => e.Id == dto!.Id);

        await _client.PatchAsJsonAsync($"/api/events/{dto!.Id}/status", new { status = "Approved" });

        var publicAfter = await _client.GetAsync("/api/public/events");
        var listAfter = await publicAfter.Content.ReadFromJsonAsync<List<EventDto>>();
        Assert.Contains(listAfter!, e => e.Id == dto.Id);
    }

    [Fact]
    public async Task GetAll_FilterByTitle_ReturnsMatching()
    {
        var uniqueTitle = $"UniqueTitle-{Guid.NewGuid()}";
        await _client.PostAsJsonAsync("/api/events", NewEvent(uniqueTitle));

        var response = await _client.GetAsync($"/api/events?title={uniqueTitle}");
        var list = await response.Content.ReadFromJsonAsync<List<EventDto>>();

        Assert.NotNull(list);
        Assert.Single(list!);
        Assert.Equal(uniqueTitle, list[0].Title);
    }
}
