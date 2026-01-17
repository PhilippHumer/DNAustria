using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DNAustria.Backend.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DNAustria.Backend.Tests.Integration;

public class ExportApprovedTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public ExportApprovedTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptors = services.Where(d => d.ServiceType == typeof(DNAustria.Backend.Data.AppDbContext) || d.ServiceType == typeof(DbContextOptions<DNAustria.Backend.Data.AppDbContext>)).ToList();
                foreach (var d in descriptors) services.Remove(d);
                services.AddDbContext<DNAustria.Backend.Data.AppDbContext>(options => options.UseInMemoryDatabase("test_export_db"));

                // Seed an approved event
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<DNAustria.Backend.Data.AppDbContext>();
                db.Addresses.Add(new DNAustria.Backend.Models.Address { Id = Guid.NewGuid(), LocationName = "Loc", City = "City", Zip = "0001", State = "Wien", Street = "Street", Latitude = 1, Longitude = 2 });
                db.Contacts.Add(new DNAustria.Backend.Models.Contact { Id = Guid.NewGuid(), Name = "C", Org = "O", Email = "c@o.com", Phone = "123" });
                db.Organizations.Add(new DNAustria.Backend.Models.Organization { Id = Guid.NewGuid(), Name = "Org", Address = "Addr" });
                db.SaveChanges();

                var addr = db.Addresses.First();
                var contact = db.Contacts.First();
                var org = db.Organizations.First();
                db.Events.Add(new DNAustria.Backend.Models.Event
                {
                    Id = Guid.NewGuid(),
                    Title = "ExportEvent",
                    Description = "Desc",
                    DateStart = DateTime.UtcNow,
                    DateEnd = DateTime.UtcNow.AddHours(1),
                    Classification = DNAustria.Backend.Models.Classification.Scheduled,
                    Fees = true,
                    IsOnline = false,
                    OrganizationId = org.Id,
                    Address = addr,
                    LocationId = addr.Id,
                    Contact = contact,
                    ContactId = contact.Id,
                    Status = DNAustria.Backend.Models.EventStatus.Approved,
                    TargetAudience = new List<DNAustria.Backend.Models.TargetAudience> { DNAustria.Backend.Models.TargetAudience.Erwachsene },
                    Topics = new List<DNAustria.Backend.Models.EventTopic> { DNAustria.Backend.Models.EventTopic.Digitalisierung_IT }
                });
                db.SaveChanges();
            });
        });
    }

    [Fact]
    public async Task ExportApproved_Returns_Events_Array_With_Required_Fields()
    {
        var client = _factory.CreateClient();
        var resp = await client.GetAsync("/api/events/export/approved");
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<ExportEventsResultDto>();
        Assert.NotNull(result);
        Assert.NotEmpty(result.Events);
        var ev = result.Events[0];
        Assert.False(string.IsNullOrWhiteSpace(ev.Event_Title));
        Assert.False(string.IsNullOrWhiteSpace(ev.Event_Description));
        Assert.NotNull(ev.Event_Start);
        Assert.NotNull(ev.Event_End);
        Assert.NotEmpty(ev.Event_Target_Audience);
        Assert.NotEmpty(ev.Event_Topics);
        Assert.False(string.IsNullOrWhiteSpace(ev.Organization_Name));
    }
}