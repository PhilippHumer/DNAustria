using System.Net;
using System.Net.Http.Json;
using DNAustria.Application.DTOs;
using DNAustria.Api.Tests.Infrastructure;

namespace DNAustria.Api.Tests;

public class LocationEndpointTests : IClassFixture<LocationApiFactory>
{
    private readonly HttpClient _client;

    public LocationEndpointTests(LocationApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<AddressDto> CreateAddressAsync(string zip, decimal lat, decimal lon)
    {
        var response = await _client.PostAsJsonAsync("/api/addresses", new
        {
            locationName = "Test Location",
            street = "Test Street",
            city = "Test City",
            zip,
            state = "Test State",
            latitude = lat,
            longitude = lon
        });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AddressDto>())!;
    }

    [Fact]
    public async Task Create_ReturnsCreated()
    {
        var address = await CreateAddressAsync($"C{Guid.NewGuid()}"[..6], 30.000001m, 30.000001m);

        var response = await _client.PostAsJsonAsync("/api/locations",
            new { name = $"Loc-{Guid.NewGuid()}", addressId = address.Id });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<LocationDto>();
        Assert.NotNull(dto);
        Assert.NotEqual(Guid.Empty, dto!.Id);
        Assert.Equal(address.Id, dto.Address.Id);
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/locations");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsLocation()
    {
        var address = await CreateAddressAsync($"G{Guid.NewGuid()}"[..6], 31.000001m, 31.000001m);
        var created = await _client.PostAsJsonAsync("/api/locations",
            new { name = $"Loc-{Guid.NewGuid()}", addressId = address.Id });
        var dto = await created.Content.ReadFromJsonAsync<LocationDto>();

        var response = await _client.GetAsync($"/api/locations/{dto!.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<LocationDto>();
        Assert.Equal(dto.Id, result!.Id);
        Assert.NotNull(result.Address);
    }

    [Fact]
    public async Task GetById_Returns404ForNonExistent()
    {
        var response = await _client.GetAsync($"/api/locations/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_ReturnsOk()
    {
        var address = await CreateAddressAsync($"U{Guid.NewGuid()}"[..6], 32.000001m, 32.000001m);
        var created = await _client.PostAsJsonAsync("/api/locations",
            new { name = $"Loc-{Guid.NewGuid()}", addressId = address.Id });
        var dto = await created.Content.ReadFromJsonAsync<LocationDto>();

        var newName = $"Loc-{Guid.NewGuid()}";
        var response = await _client.PutAsJsonAsync($"/api/locations/{dto!.Id}",
            new { name = newName, addressId = address.Id });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<LocationDto>();
        Assert.Equal(newName, updated!.Name);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var address = await CreateAddressAsync($"D{Guid.NewGuid()}"[..6], 33.000001m, 33.000001m);
        var created = await _client.PostAsJsonAsync("/api/locations",
            new { name = $"Loc-{Guid.NewGuid()}", addressId = address.Id });
        var dto = await created.Content.ReadFromJsonAsync<LocationDto>();

        var response = await _client.DeleteAsync($"/api/locations/{dto!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Returns404ForDeletedLocation()
    {
        var address = await CreateAddressAsync($"SD{Guid.NewGuid()}"[..5], 34.000001m, 34.000001m);
        var created = await _client.PostAsJsonAsync("/api/locations",
            new { name = $"Loc-{Guid.NewGuid()}", addressId = address.Id });
        var dto = await created.Content.ReadFromJsonAsync<LocationDto>();

        await _client.DeleteAsync($"/api/locations/{dto!.Id}");

        var response = await _client.GetAsync($"/api/locations/{dto.Id}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_Returns404ForNonExistentAddressId()
    {
        var response = await _client.PostAsJsonAsync("/api/locations",
            new { name = "SomeLoc", addressId = Guid.NewGuid() });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_Returns404ForDeletedAddressId()
    {
        var address = await CreateAddressAsync($"DA{Guid.NewGuid()}"[..5], 35.000001m, 35.000001m);
        await _client.DeleteAsync($"/api/addresses/{address.Id}");

        var response = await _client.PostAsJsonAsync("/api/locations",
            new { name = "SomeLoc", addressId = address.Id });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Location_AddressRemainsIntact()
    {
        var address = await CreateAddressAsync($"AR{Guid.NewGuid()}"[..5], 36.000001m, 36.000001m);
        var created = await _client.PostAsJsonAsync("/api/locations",
            new { name = $"Loc-{Guid.NewGuid()}", addressId = address.Id });
        var dto = await created.Content.ReadFromJsonAsync<LocationDto>();

        await _client.DeleteAsync($"/api/locations/{dto!.Id}");

        var addressResponse = await _client.GetAsync($"/api/addresses/{address.Id}");
        Assert.Equal(HttpStatusCode.OK, addressResponse.StatusCode);
    }

    [Fact]
    public async Task ResponseContainsEmbeddedAddress()
    {
        var address = await CreateAddressAsync($"EM{Guid.NewGuid()}"[..5], 37.000001m, 37.000001m);
        var created = await _client.PostAsJsonAsync("/api/locations",
            new { name = $"Loc-{Guid.NewGuid()}", addressId = address.Id });

        var dto = await created.Content.ReadFromJsonAsync<LocationDto>();

        Assert.NotNull(dto!.Address);
        Assert.Equal(address.Id, dto.Address.Id);
        Assert.Equal(address.Street, dto.Address.Street);
        Assert.Equal(address.City, dto.Address.City);
    }

    [Fact]
    public async Task Create_EmptyName_ReturnsBadRequest()
    {
        var address = await CreateAddressAsync($"EN{Guid.NewGuid()}"[..5], 38.000001m, 38.000001m);
        var response = await _client.PostAsJsonAsync("/api/locations",
            new { name = "", addressId = address.Id });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task SoftDeleted_NotReturnedInGetAll()
    {
        var address = await CreateAddressAsync($"SL{Guid.NewGuid()}"[..5], 39.000001m, 39.000001m);
        var name = $"Loc-{Guid.NewGuid()}";
        var created = await _client.PostAsJsonAsync("/api/locations",
            new { name, addressId = address.Id });
        var dto = await created.Content.ReadFromJsonAsync<LocationDto>();

        await _client.DeleteAsync($"/api/locations/{dto!.Id}");

        var response = await _client.GetAsync("/api/locations");
        var list = await response.Content.ReadFromJsonAsync<List<LocationDto>>();

        Assert.DoesNotContain(list!, l => l.Id == dto.Id);
    }
}
