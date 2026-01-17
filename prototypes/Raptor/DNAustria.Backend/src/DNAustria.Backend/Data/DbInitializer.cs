using DNAustria.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace DNAustria.Backend.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext db)
    {
        // Ensure DB created (migrations already applied in Program)
        // Seed only when tables are empty
        // Seed organizations instead of addresses
        if (!await db.Organizations.AnyAsync())
        {
            var contact1 = new Contact { Id = Guid.NewGuid(), Name = "Anna Müller", Org = "FH OOE", Email = "anna.mueller@fh-ooe.at", Phone = "+43 123 456" };
            var contact2 = new Contact { Id = Guid.NewGuid(), Name = "Max Mustermann", Org = "Culture Org", Email = "max@culture.at", Phone = "+43 987 654" };
            db.Contacts.AddRange(contact1, contact2);

            var org1 = new Organization
            {
                Id = Guid.NewGuid(),
                Name = "FH OOE",
                Address = "Hauptstraße 1, 4040 Linz",
                City = "Linz",
                Zip = "4040",
                State = "Upper Austria",
                Street = "Hauptstraße 1",
                Latitude = 48.30694,
                Longitude = 14.28583
            };
            var org2 = new Organization
            {
                Id = Guid.NewGuid(),
                Name = "Kulturhaus",
                Address = "Kulturstrasse 2, 4020 Linz",
                City = "Linz",
                Zip = "4020",
                State = "Upper Austria",
                Street = "Kulturstrasse 2",
                Latitude = 48.306,
                Longitude = 14.287
            };
            db.Organizations.AddRange(org1, org2);

            await db.SaveChangesAsync();

            if (!await db.Events.AnyAsync())
            {
                var e1 = new Event
                {
                    Id = Guid.NewGuid(),
                    Title = "Intro to AI",
                    Description = "Lecture introducing AI basics for students.",
                    DateStart = DateTime.UtcNow.AddDays(7),
                    DateEnd = DateTime.UtcNow.AddDays(7).AddHours(2),
                    Classification = Classification.Scheduled,
                    Fees = false,
                    IsOnline = false,
                    OrganizationId = org1.Id,
                    ProgramName = "AI101",
                    Format = "Lecture",
                    SchoolBookable = true,
                    AgeMinimum = null,
                    AgeMaximum = null,
                    Location = org1,
                    LocationId = org1.Id,
                    Contact = contact1,
                    ContactId = contact1.Id,
                    Status = EventStatus.Approved,
                    TargetAudience = new List<TargetAudience> { TargetAudience.Erwachsene },
                    Topics = new List<EventTopic> { EventTopic.Digitalisierung_IT }
                };

                var e2 = new Event
                {
                    Id = Guid.NewGuid(),
                    Title = "Kids Science Day",
                    Description = "Hands-on science activities for children.",
                    DateStart = DateTime.UtcNow.AddDays(14),
                    DateEnd = DateTime.UtcNow.AddDays(14).AddHours(4),
                    Classification = Classification.Scheduled,
                    Fees = false,
                    IsOnline = false,
                    OrganizationId = org1.Id,
                    ProgramName = "ScienceFun",
                    Format = "Workshop",
                    SchoolBookable = true,
                    AgeMinimum = 6,
                    AgeMaximum = 12,
                    Location = org2,
                    LocationId = org2.Id,
                    Contact = contact2,
                    ContactId = contact2.Id,
                    Status = EventStatus.Draft,
                    TargetAudience = new List<TargetAudience> { TargetAudience.Schulkinder },
                    Topics = new List<EventTopic> { EventTopic.Naturwissenschaft_Klima_Umwelt }
                };

                db.Events.AddRange(e1, e2);
                await db.SaveChangesAsync();
            }
        }
    }
}