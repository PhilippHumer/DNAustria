using System.Net;
using System.Net.Http.Json;
using DNAustria.Application.DTOs;
using DNAustria.Api.Tests.Infrastructure;

namespace DNAustria.Api.Tests;

public class OrganizationEndpointTests : IClassFixture<OrganizationApiFactory>
{
    private readonly HttpClient _client;

    public OrganizationEndpointTests(OrganizationApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_ReturnsCreated()
    {
        var response = await _client.PostAsJsonAsync("/api/organizations",
            new { name = $"Org-{Guid.NewGuid()}" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<OrganizationDto>();
        Assert.NotNull(dto);
        Assert.NotEqual(Guid.Empty, dto!.Id);
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/organizations");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsOrganization()
    {
        var name = $"Org-{Guid.NewGuid()}";
        var created = await _client.PostAsJsonAsync("/api/organizations", new { name });
        var dto = await created.Content.ReadFromJsonAsync<OrganizationDto>();

        var response = await _client.GetAsync($"/api/organizations/{dto!.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<OrganizationDto>();
        Assert.Equal(dto.Id, result!.Id);
    }

    [Fact]
    public async Task GetById_NotFound_Returns404()
    {
        var response = await _client.GetAsync($"/api/organizations/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_ReturnsOk()
    {
        var name = $"Org-{Guid.NewGuid()}";
        var created = await _client.PostAsJsonAsync("/api/organizations", new { name });
        var dto = await created.Content.ReadFromJsonAsync<OrganizationDto>();

        var newName = $"Org-{Guid.NewGuid()}";
        var response = await _client.PutAsJsonAsync($"/api/organizations/{dto!.Id}",
            new { name = newName });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<OrganizationDto>();
        Assert.Equal(newName, updated!.Name);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var name = $"Org-{Guid.NewGuid()}";
        var created = await _client.PostAsJsonAsync("/api/organizations", new { name });
        var dto = await created.Content.ReadFromJsonAsync<OrganizationDto>();

        var response = await _client.DeleteAsync($"/api/organizations/{dto!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_AlreadyDeleted_Returns404()
    {
        var name = $"Org-{Guid.NewGuid()}";
        var created = await _client.PostAsJsonAsync("/api/organizations", new { name });
        var dto = await created.Content.ReadFromJsonAsync<OrganizationDto>();

        await _client.DeleteAsync($"/api/organizations/{dto!.Id}");
        var response = await _client.DeleteAsync($"/api/organizations/{dto.Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_DuplicateName_ReturnsConflict()
    {
        var name = $"Org-{Guid.NewGuid()}";
        await _client.PostAsJsonAsync("/api/organizations", new { name });

        var response = await _client.PostAsJsonAsync("/api/organizations", new { name });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task SoftDeleted_NotReturnedInGetAll()
    {
        var name = $"Org-{Guid.NewGuid()}";
        var created = await _client.PostAsJsonAsync("/api/organizations", new { name });
        var dto = await created.Content.ReadFromJsonAsync<OrganizationDto>();

        await _client.DeleteAsync($"/api/organizations/{dto!.Id}");

        var response = await _client.GetAsync($"/api/organizations?name={name}");
        var list = await response.Content.ReadFromJsonAsync<List<OrganizationDto>>();

        Assert.DoesNotContain(list!, o => o.Id == dto.Id);
    }

    [Fact]
    public async Task SoftDeleted_GetById_Returns404()
    {
        var name = $"Org-{Guid.NewGuid()}";
        var created = await _client.PostAsJsonAsync("/api/organizations", new { name });
        var dto = await created.Content.ReadFromJsonAsync<OrganizationDto>();

        await _client.DeleteAsync($"/api/organizations/{dto!.Id}");

        var response = await _client.GetAsync($"/api/organizations/{dto.Id}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_EmptyName_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/organizations", new { name = "" });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_NameTooLong_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/organizations",
            new { name = new string('A', 51) });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
