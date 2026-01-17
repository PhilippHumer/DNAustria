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

    private EventService CreateService(AppDbContext db, IMapper mapper, ILLMService llm) => new EventService(db, mapper, llm, null);

    [Fact]
    public async Task CreateEvent_WithInlineLocation_SavesInlineFields_OnEvent()
    {
        var db = CreateDbContext("create_with_inline_location");
        var mapper = CreateMapper();
        var llm = new StubLLMService();
        var svc = CreateService(db, mapper, llm);

        var dto = new Dtos.EventCreateDto
        {
            Title = "Test",
            Description = "desc",
            DateStart = DateTime.UtcNow,
            DateEnd = DateTime.UtcNow.AddHours(2),
            Location = new Dtos.OrganizationCreateDto { Name = "Hall", Zip = "4020", City = "Linz" }
        };

        var created = await svc.CreateEventAsync(dto);

        Assert.NotEqual(Guid.Empty, created.Id);
        // No OrganizationId should be set when inline location doesn't match an existing Organization
        Assert.Null(created.OrganizationId);
        var orgCount = await db.Organizations.CountAsync();
        Assert.Equal(0, orgCount);

        // Inline fields should be stored on the event DTO's nested Location object
        Assert.NotNull(created.Location);
        Assert.Equal("Hall", created.Location!.Name);
        Assert.Equal("4020", created.Location.Zip);
        Assert.Equal("Linz", created.Location.City);
    }

    [Fact]
    public async Task CreateEvent_WithLocationId_UsesExistingAddress()
    {
        var db = CreateDbContext("create_with_locationid");
        var org = new Organization { Id = Guid.NewGuid(), Name = "Existing", Zip = "4020", City = "Linz" };
        db.Organizations.Add(org);
        await db.SaveChangesAsync();

        var mapper = CreateMapper();
        var llm = new StubLLMService();
        var svc = CreateService(db, mapper, llm);

        var dto = new Dtos.EventCreateDto { Title = "T2", DateStart = DateTime.UtcNow, DateEnd = DateTime.UtcNow.AddHours(1), OrganizationId = org.Id };
        var created = await svc.CreateEventAsync(dto);

        Assert.Equal(org.Id, created.OrganizationId);
        var ev = await db.Events.Include(e => e.Location).FirstOrDefaultAsync(e => e.Id == created.Id);
        Assert.NotNull(ev);
        Assert.Equal("Existing", ev.Location?.Name);

        // The returned DTO should include a nested Location populated from the selected Organization
        Assert.NotNull(created.Location);
        Assert.Equal("Existing", created.Location!.Name);
        Assert.Equal("4020", created.Location.Zip);
        Assert.Equal("Linz", created.Location.City);
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
        var svc = CreateService(db, mapper, llm);

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
        var svc = CreateService(db, mapper, llm);

        var ok = await svc.UpdateStatusAsync(ev.Id, EventStatus.Approved);
        Assert.False(ok);
    }

    [Fact]
    public async Task ParseEvent_ReturnsDto_WithTitle()
    {
        var db = CreateDbContext("parse_event");
        var mapper = CreateMapper();
        var llm = new StubLLMService();
        var svc = CreateService(db, mapper, llm);

        var parsed = await svc.ParseEventAsync("<h1>My Event</h1>\n12.01.2026", true);
        Assert.NotNull(parsed);
        Assert.Contains("My Event", parsed.Title);
    }
}
