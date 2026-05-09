using System.Net;
using System.Net.Http.Json;
using DNAustria.Application.DTOs;
using DNAustria.Api.Tests.Infrastructure;

namespace DNAustria.Api.Tests;

public class AddressEndpointTests : IClassFixture<AddressApiFactory>
{
    private readonly HttpClient _client;

    public AddressEndpointTests(AddressApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    private static object NewAddress(string zip = "4232", decimal lat = 48.368000m, decimal lon = 14.513000m) =>
        new
        {
            locationName = "FH Hagenberg",
            street = "Softwarepark 11",
            city = "Hagenberg",
            zip,
            state = "Oberösterreich",
            latitude = lat,
            longitude = lon
        };

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/addresses");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsCreated()
    {
        var response = await _client.PostAsJsonAsync("/api/addresses", NewAddress($"{Guid.NewGuid()}"[..6]));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<AddressDto>();
        Assert.NotNull(dto);
        Assert.NotEqual(Guid.Empty, dto!.Id);
    }

    [Fact]
    public async Task Create_ReturnExistingAddressWhenDuplicateExists()
    {
        var zip = $"Z{Guid.NewGuid()}"[..6];
        var lat = 10.000001m;
        var lon = 20.000001m;

        var first = await _client.PostAsJsonAsync("/api/addresses", NewAddress(zip, lat, lon));
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        var firstDto = await first.Content.ReadFromJsonAsync<AddressDto>();

        var second = await _client.PostAsJsonAsync("/api/addresses", NewAddress(zip, lat, lon));
        Assert.Equal(HttpStatusCode.Created, second.StatusCode);
        var secondDto = await second.Content.ReadFromJsonAsync<AddressDto>();

        Assert.Equal(firstDto!.Id, secondDto!.Id);
    }

    [Fact]
    public async Task Create_CreatesNewAddressWhenNoDuplicate()
    {
        var first = await _client.PostAsJsonAsync("/api/addresses", NewAddress("1111", 1.000001m, 1.000001m));
        var firstDto = await first.Content.ReadFromJsonAsync<AddressDto>();

        var second = await _client.PostAsJsonAsync("/api/addresses", NewAddress("2222", 2.000002m, 2.000002m));
        var secondDto = await second.Content.ReadFromJsonAsync<AddressDto>();

        Assert.NotEqual(firstDto!.Id, secondDto!.Id);
    }

    [Fact]
    public async Task GetById_ReturnsAddress()
    {
        var zip = $"G{Guid.NewGuid()}"[..6];
        var created = await _client.PostAsJsonAsync("/api/addresses", NewAddress(zip, 11.000001m, 11.000001m));
        var dto = await created.Content.ReadFromJsonAsync<AddressDto>();

        var response = await _client.GetAsync($"/api/addresses/{dto!.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<AddressDto>();
        Assert.Equal(dto.Id, result!.Id);
    }

    [Fact]
    public async Task GetById_Returns404ForNonExistent()
    {
        var response = await _client.GetAsync($"/api/addresses/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_ReturnsOk()
    {
        var zip = $"U{Guid.NewGuid()}"[..6];
        var created = await _client.PostAsJsonAsync("/api/addresses", NewAddress(zip, 12.000001m, 12.000001m));
        var dto = await created.Content.ReadFromJsonAsync<AddressDto>();

        var newZip = $"U{Guid.NewGuid()}"[..6];
        var response = await _client.PutAsJsonAsync($"/api/addresses/{dto!.Id}",
            NewAddress(newZip, 12.000002m, 12.000002m));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<AddressDto>();
        Assert.Equal(newZip, updated!.Zip);
    }

    [Fact]
    public async Task Update_Returns400WhenDuplicateZipLatLonWouldOccur()
    {
        var zip = $"D{Guid.NewGuid()}"[..6];
        var lat = 13.000001m;
        var lon = 13.000001m;

        var first = await _client.PostAsJsonAsync("/api/addresses", NewAddress(zip, lat, lon));
        var firstDto = await first.Content.ReadFromJsonAsync<AddressDto>();

        var otherZip = $"D{Guid.NewGuid()}"[..6];
        var second = await _client.PostAsJsonAsync("/api/addresses", NewAddress(otherZip, 14.000002m, 14.000002m));
        var secondDto = await second.Content.ReadFromJsonAsync<AddressDto>();

        // Try to update second address to have the same dedup key as first
        var response = await _client.PutAsJsonAsync($"/api/addresses/{secondDto!.Id}",
            NewAddress(zip, lat, lon));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_SucceedsWhenCombinationStaysUnique()
    {
        var zip = $"S{Guid.NewGuid()}"[..6];
        var created = await _client.PostAsJsonAsync("/api/addresses", NewAddress(zip, 15.000001m, 15.000001m));
        var dto = await created.Content.ReadFromJsonAsync<AddressDto>();

        // Update to same combination (self) — should succeed
        var response = await _client.PutAsJsonAsync($"/api/addresses/{dto!.Id}",
            NewAddress(zip, 15.000001m, 15.000001m));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var zip = $"X{Guid.NewGuid()}"[..6];
        var created = await _client.PostAsJsonAsync("/api/addresses", NewAddress(zip, 16.000001m, 16.000001m));
        var dto = await created.Content.ReadFromJsonAsync<AddressDto>();

        var response = await _client.DeleteAsync($"/api/addresses/{dto!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Returns404ForDeletedAddress()
    {
        var zip = $"Y{Guid.NewGuid()}"[..6];
        var created = await _client.PostAsJsonAsync("/api/addresses", NewAddress(zip, 17.000001m, 17.000001m));
        var dto = await created.Content.ReadFromJsonAsync<AddressDto>();

        await _client.DeleteAsync($"/api/addresses/{dto!.Id}");

        var response = await _client.GetAsync($"/api/addresses/{dto.Id}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_BlockedWhenReferencedByActiveLocation()
    {
        var zip = $"B{Guid.NewGuid()}"[..6];
        var addrResponse = await _client.PostAsJsonAsync("/api/addresses", NewAddress(zip, 18.000001m, 18.000001m));
        var addrDto = await addrResponse.Content.ReadFromJsonAsync<AddressDto>();

        await _client.PostAsJsonAsync("/api/locations",
            new { name = $"Loc-{Guid.NewGuid()}", addressId = addrDto!.Id });

        var response = await _client.DeleteAsync($"/api/addresses/{addrDto.Id}");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Delete_AllowedWhenNotReferenced()
    {
        var zip = $"N{Guid.NewGuid()}"[..6];
        var addrResponse = await _client.PostAsJsonAsync("/api/addresses", NewAddress(zip, 19.000001m, 19.000001m));
        var addrDto = await addrResponse.Content.ReadFromJsonAsync<AddressDto>();

        var response = await _client.DeleteAsync($"/api/addresses/{addrDto!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task SoftDeleted_NotReturnedInGetAll()
    {
        var zip = $"SD{Guid.NewGuid()}"[..5];
        var created = await _client.PostAsJsonAsync("/api/addresses", NewAddress(zip, 20.000001m, 20.000001m));
        var dto = await created.Content.ReadFromJsonAsync<AddressDto>();

        await _client.DeleteAsync($"/api/addresses/{dto!.Id}");

        var response = await _client.GetAsync("/api/addresses");
        var list = await response.Content.ReadFromJsonAsync<List<AddressDto>>();

        Assert.DoesNotContain(list!, a => a.Id == dto.Id);
    }

    [Fact]
    public async Task Create_MissingLocationName_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/addresses", new
        {
            locationName = "",
            street = "Street",
            city = "City",
            zip = "1234",
            state = "State",
            latitude = 1.0m,
            longitude = 1.0m
        });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
