using System.Net;
using System.Net.Http.Json;
using DNAustria.Api.Tests.Infrastructure;
using DNAustria.Application.DTOs;

namespace DNAustria.Api.Tests;

public class OrganizationEndpointTests : IClassFixture<OrganizationApiFactory>, IAsyncLifetime
{
    private readonly OrganizationApiFactory _factory;
    private readonly HttpClient _client;

    public OrganizationEndpointTests(OrganizationApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync() => await _factory.ApplyMigrationsAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    // --- GET /api/organizations ---

    [Fact]
    public async Task GetAll_ReturnsOk_WithEmptyList()
    {
        var response = await _client.GetAsync("/api/organizations");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<List<OrganizationDto>>();
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetAll_WithNameFilter_ReturnsMatchingOrganizations()
    {
        await CreateOrganizationAsync("Filter Hagenberg");
        await CreateOrganizationAsync("Filter Vienna");

        var response = await _client.GetAsync("/api/organizations?name=hagen");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<List<OrganizationDto>>();
        Assert.NotNull(result);
        Assert.Contains(result, o => o.Name == "Filter Hagenberg");
        Assert.DoesNotContain(result, o => o.Name == "Filter Vienna");
    }

    // --- GET /api/organizations/{id} ---

    [Fact]
    public async Task GetById_ReturnsOrganization_WhenExists()
    {
        var created = await CreateOrganizationAsync("GetById Org");

        var response = await _client.GetAsync($"/api/organizations/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<OrganizationDto>();
        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
        Assert.Equal("GetById Org", result.Name);
    }

    [Fact]
    public async Task GetById_Returns404_WhenNotFound()
    {
        var response = await _client.GetAsync($"/api/organizations/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // --- POST /api/organizations ---

    [Fact]
    public async Task Create_Returns201_WithBody()
    {
        var request = new { name = "New Create Org" };
        var response = await _client.PostAsJsonAsync("/api/organizations", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<OrganizationDto>();
        Assert.NotNull(result);
        Assert.Equal("New Create Org", result.Name);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task Create_Returns400_WhenNameIsEmpty()
    {
        var request = new { name = "   " };
        var response = await _client.PostAsJsonAsync("/api/organizations", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_Returns400_WhenNameExceeds50Chars()
    {
        var request = new { name = new string('A', 51) };
        var response = await _client.PostAsJsonAsync("/api/organizations", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_Returns409_WhenNameAlreadyExists()
    {
        await CreateOrganizationAsync("Duplicate Org");

        var request = new { name = "Duplicate Org" };
        var response = await _client.PostAsJsonAsync("/api/organizations", request);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Create_Returns409_WhenNameAlreadyExists_CaseInsensitive()
    {
        await CreateOrganizationAsync("CaseTest Org");

        var request = new { name = "casetest org" };
        var response = await _client.PostAsJsonAsync("/api/organizations", request);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    // --- PUT /api/organizations/{id} ---

    [Fact]
    public async Task Update_Returns200_WithUpdatedBody()
    {
        var created = await CreateOrganizationAsync("Before Update");

        var request = new { name = "After Update" };
        var response = await _client.PutAsJsonAsync($"/api/organizations/{created.Id}", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<OrganizationDto>();
        Assert.NotNull(result);
        Assert.Equal("After Update", result.Name);
    }

    [Fact]
    public async Task Update_Returns404_WhenNotFound()
    {
        var request = new { name = "Doesn't matter" };
        var response = await _client.PutAsJsonAsync($"/api/organizations/{Guid.NewGuid()}", request);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_Returns409_WhenNameTakenByOther()
    {
        await CreateOrganizationAsync("Taken Name");
        var second = await CreateOrganizationAsync("Second Org");

        var request = new { name = "Taken Name" };
        var response = await _client.PutAsJsonAsync($"/api/organizations/{second.Id}", request);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    // --- DELETE /api/organizations/{id} ---

    [Fact]
    public async Task Delete_Returns204_AndOrganizationIsGone()
    {
        var created = await CreateOrganizationAsync("To Delete Org");

        var deleteResponse = await _client.DeleteAsync($"/api/organizations/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/api/organizations/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_Returns404_WhenNotFound()
    {
        var response = await _client.DeleteAsync($"/api/organizations/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Returns404_WhenAlreadyDeleted()
    {
        var created = await CreateOrganizationAsync("Double Delete Org");

        await _client.DeleteAsync($"/api/organizations/{created.Id}");
        var response = await _client.DeleteAsync($"/api/organizations/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // --- Soft Delete behavior ---

    [Fact]
    public async Task SoftDelete_DoesNotAppearInGetAll()
    {
        var created = await CreateOrganizationAsync("Soft Delete List Org");
        await _client.DeleteAsync($"/api/organizations/{created.Id}");

        var response = await _client.GetAsync("/api/organizations");
        var result = await response.Content.ReadFromJsonAsync<List<OrganizationDto>>();
        Assert.NotNull(result);
        Assert.DoesNotContain(result, o => o.Id == created.Id);
    }

    [Fact]
    public async Task SoftDelete_AllowsReuseOfName()
    {
        var created = await CreateOrganizationAsync("Reusable Name Org");
        await _client.DeleteAsync($"/api/organizations/{created.Id}");

        var request = new { name = "Reusable Name Org" };
        var response = await _client.PostAsJsonAsync("/api/organizations", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    // --- Helper ---

    private async Task<OrganizationDto> CreateOrganizationAsync(string name, Guid? addressId = null)
    {
        var request = new { name, address_id = addressId };
        var response = await _client.PostAsJsonAsync("/api/organizations", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<OrganizationDto>())!;
    }
}
