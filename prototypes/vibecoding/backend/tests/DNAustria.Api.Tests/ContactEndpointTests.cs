using System.Net;
using System.Net.Http.Json;
using DNAustria.Application.DTOs;
using DNAustria.Api.Tests.Infrastructure;

namespace DNAustria.Api.Tests;

public class ContactEndpointTests : IClassFixture<ContactApiFactory>
{
    private readonly HttpClient _client;

    public ContactEndpointTests(ContactApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    private static object NewContact(string? name = null) => new
    {
        name = name ?? $"Contact-{Guid.NewGuid()}",
        email = "test@example.com",
        phone = "+43 123 456789"
    };

    [Fact]
    public async Task Create_ReturnsCreated()
    {
        var response = await _client.PostAsJsonAsync("/api/contacts", NewContact());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<ContactDto>();
        Assert.NotNull(dto);
        Assert.NotEqual(Guid.Empty, dto!.Id);
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/contacts");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsContact()
    {
        var created = await _client.PostAsJsonAsync("/api/contacts", NewContact());
        var dto = await created.Content.ReadFromJsonAsync<ContactDto>();

        var response = await _client.GetAsync($"/api/contacts/{dto!.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<ContactDto>();
        Assert.Equal(dto.Id, result!.Id);
    }

    [Fact]
    public async Task GetById_NotFound_Returns404()
    {
        var response = await _client.GetAsync($"/api/contacts/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_ReturnsOk()
    {
        var created = await _client.PostAsJsonAsync("/api/contacts", NewContact());
        var dto = await created.Content.ReadFromJsonAsync<ContactDto>();

        var newName = $"Contact-{Guid.NewGuid()}";
        var response = await _client.PutAsJsonAsync($"/api/contacts/{dto!.Id}", new
        {
            name = newName,
            email = "updated@example.com",
            phone = "+43 999 000000"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<ContactDto>();
        Assert.Equal(newName, updated!.Name);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var created = await _client.PostAsJsonAsync("/api/contacts", NewContact());
        var dto = await created.Content.ReadFromJsonAsync<ContactDto>();

        var response = await _client.DeleteAsync($"/api/contacts/{dto!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_AlreadyDeleted_Returns404()
    {
        var created = await _client.PostAsJsonAsync("/api/contacts", NewContact());
        var dto = await created.Content.ReadFromJsonAsync<ContactDto>();

        await _client.DeleteAsync($"/api/contacts/{dto!.Id}");
        var response = await _client.DeleteAsync($"/api/contacts/{dto.Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_NotExisting_Returns404()
    {
        var response = await _client.DeleteAsync($"/api/contacts/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_DuplicateName_ReturnsConflict()
    {
        var name = $"Contact-{Guid.NewGuid()}";
        await _client.PostAsJsonAsync("/api/contacts", NewContact(name));

        var response = await _client.PostAsJsonAsync("/api/contacts", NewContact(name));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task SoftDeleted_NotReturnedInGetAll()
    {
        var name = $"Contact-{Guid.NewGuid()}";
        var created = await _client.PostAsJsonAsync("/api/contacts", NewContact(name));
        var dto = await created.Content.ReadFromJsonAsync<ContactDto>();

        await _client.DeleteAsync($"/api/contacts/{dto!.Id}");

        var response = await _client.GetAsync($"/api/contacts?name={name}");
        var list = await response.Content.ReadFromJsonAsync<List<ContactDto>>();

        Assert.DoesNotContain(list!, c => c.Id == dto.Id);
    }

    [Fact]
    public async Task SoftDeleted_GetById_Returns404()
    {
        var created = await _client.PostAsJsonAsync("/api/contacts", NewContact());
        var dto = await created.Content.ReadFromJsonAsync<ContactDto>();

        await _client.DeleteAsync($"/api/contacts/{dto!.Id}");

        var response = await _client.GetAsync($"/api/contacts/{dto.Id}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_EmptyName_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/contacts", new
        {
            name = "",
            email = "test@example.com",
            phone = "+43 123 456789"
        });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_NameTooLong_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/contacts", new
        {
            name = new string('A', 51),
            email = "test@example.com",
            phone = "+43 123 456789"
        });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_InvalidEmail_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/contacts", new
        {
            name = $"Contact-{Guid.NewGuid()}",
            email = "not-an-email",
            phone = "+43 123 456789"
        });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_DuplicateName_CaseInsensitive_ReturnsConflict()
    {
        var baseName = $"Contact-{Guid.NewGuid()}";
        await _client.PostAsJsonAsync("/api/contacts", NewContact(baseName.ToLower()));

        var response = await _client.PostAsJsonAsync("/api/contacts", NewContact(baseName.ToUpper()));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Create_OrgWithoutOrganizationId_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/contacts", new
        {
            name = $"Contact-{Guid.NewGuid()}",
            email = "test@example.com",
            phone = "+43 123 456789",
            org = "Some Org"
        });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_OrganizationIdWithoutOrg_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/contacts", new
        {
            name = $"Contact-{Guid.NewGuid()}",
            email = "test@example.com",
            phone = "+43 123 456789",
            organizationId = Guid.NewGuid()
        });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithBothOrganizationIdAndOrg_ReturnsCreated()
    {
        var orgId = Guid.NewGuid();
        var response = await _client.PostAsJsonAsync("/api/contacts", new
        {
            name = $"Contact-{Guid.NewGuid()}",
            email = "test@example.com",
            phone = "+43 123 456789",
            organizationId = orgId,
            org = "DNAustria"
        });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<ContactDto>();
        Assert.Equal(orgId, dto!.OrganizationId);
        Assert.Equal("DNAustria", dto.Org);
    }
}
