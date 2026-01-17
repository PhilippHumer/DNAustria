using System;
using AutoMapper;
using DNAustria.Backend.Data;
using DNAustria.Backend.Mapping;
using DNAustria.Backend.Models;
using DNAustria.Backend.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;

namespace DNAustria.Backend.Tests;

public class EventServiceTests
{
    private AppDbContext CreateDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AppDbContext(options);
    }

    private IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
        return config.CreateMapper();
    }

    [Fact]
    public async Task CreateEvent_WithInlineAddress_CreatesAddressAndSetsLocationId()
    {
        var db = CreateDbContext("create_with_address");
        var mapper = CreateMapper();
        var llm = new StubLLMService();
        var svc = new EventService(db, mapper, llm);

        var dto = new Dtos.EventCreateDto
        {
            Title = "Test",
            Description = "desc",
            DateStart = DateTime.UtcNow,
            DateEnd = DateTime.UtcNow.AddHours(2),
            Address = new Dtos.AddressCreateDto { LocationName = "Hall", Zip = "4020", City = "Linz" }
        };

        var created = await svc.CreateEventAsync(dto);

        Assert.NotEqual(Guid.Empty, created.Id);
        Assert.NotNull(created.LocationId);
        var addr = await db.Addresses.FindAsync(created.LocationId.Value);
        Assert.NotNull(addr);
        Assert.Equal("Hall", addr.LocationName);
    }

    [Fact]
    public async Task CreateEvent_WithLocationId_UsesExistingAddress()
    {
        var db = CreateDbContext("create_with_locationid");
        var addr = new Address { Id = Guid.NewGuid(), LocationName = "Existing", Zip = "4020", City = "Linz" };
        db.Addresses.Add(addr);
        await db.SaveChangesAsync();

        var mapper = CreateMapper();
        var llm = new StubLLMService();
        var svc = new EventService(db, mapper, llm);

        var dto = new Dtos.EventCreateDto { Title = "T2", DateStart = DateTime.UtcNow, DateEnd = DateTime.UtcNow.AddHours(1), LocationId = addr.Id };
        var created = await svc.CreateEventAsync(dto);

        Assert.Equal(addr.Id, created.LocationId);
        var ev = await db.Events.Include(e => e.Address).FirstOrDefaultAsync(e => e.Id == created.Id);
        Assert.NotNull(ev);
        Assert.Equal("Existing", ev.Address?.LocationName);
    }

    [Fact]
    public async Task UpdateEvent_WhenTransferred_ReturnsNull()
    {
        var db = CreateDbContext("update_transferred");
        var ev = new Event { Id = Guid.NewGuid(), Title = "T", DateStart = DateTime.UtcNow, DateEnd = DateTime.UtcNow.AddHours(1), Status = EventStatus.Transferred };
        db.Events.Add(ev);
        await db.SaveChangesAsync();

        var mapper = CreateMapper();
        var llm = new StubLLMService();
        var svc = new EventService(db, mapper, llm);

        var dto = new Dtos.EventCreateDto { Title = "New" };
        var updated = await svc.UpdateEventAsync(ev.Id, dto);
        Assert.Null(updated);
    }

    [Fact]
    public async Task UpdateStatus_CannotUpdateIfTransferred()
    {
        var db = CreateDbContext("status_transferred");
        var ev = new Event { Id = Guid.NewGuid(), Title = "T", DateStart = DateTime.UtcNow, DateEnd = DateTime.UtcNow.AddHours(1), Status = EventStatus.Transferred };
        db.Events.Add(ev);
        await db.SaveChangesAsync();

        var mapper = CreateMapper();
        var llm = new StubLLMService();
        var svc = new EventService(db, mapper, llm);

        var ok = await svc.UpdateStatusAsync(ev.Id, EventStatus.Approved);
        Assert.False(ok);
    }

    [Fact]
    public async Task ParseEvent_ReturnsDto_WithTitle()
    {
        var db = CreateDbContext("parse_event");
        var mapper = CreateMapper();
        var llm = new StubLLMService();
        var svc = new EventService(db, mapper, llm);

        var parsed = await svc.ParseEventAsync("<h1>My Event</h1>\n12.01.2026", true);
        Assert.NotNull(parsed);
        Assert.Contains("My Event", parsed.Title);
    }
}
