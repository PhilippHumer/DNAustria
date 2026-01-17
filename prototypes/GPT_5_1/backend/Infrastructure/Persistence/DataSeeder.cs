using Domain.Entities;
using System;
using System.Linq;

namespace Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (!db.Organizations.Any())
        {
            var org = new Organization
            {
                Id = Guid.NewGuid(),
                Name = "FH Upper Austria",
                AddressCity = "Hagenberg",
                AddressStreet = "Softwarepark 1",
                AddressZip = "4232",
                RegionId = 4
            };
            db.Organizations.Add(org);

            var contact = new Contact
            {
                Id = Guid.NewGuid(),
                Name = "Max Mustermann",
                Email = "max@example.com",
                Phone = "+43 123 4567",
                Organization = org
            };
            db.Contacts.Add(contact);

            db.Events.AddRange(
                new Event
                {
                    Id = Guid.NewGuid(),
                    Title = "AI in Education",
                    Description = "Seminar zu KI-Anwendungen im Bildungsbereich",
                    DateStart = DateTime.UtcNow.AddDays(14),
                    DateEnd = DateTime.UtcNow.AddDays(14).AddHours(3),
                    Organization = org,
                    Contact = contact,
                    IsOnline = true,
                    Topics = new[]{1,2},
                    TargetAudience = new[]{101,102},
                    Status = EventStatus.Approved,
                    ModifiedAt = DateTime.UtcNow
                },
                new Event
                {
                    Id = Guid.NewGuid(),
                    Title = "Cybersecurity Basics",
                    Description = "Grundlagen der IT-Sicherheit",
                    DateStart = DateTime.UtcNow.AddDays(30),
                    DateEnd = DateTime.UtcNow.AddDays(30).AddHours(2),
                    Organization = org,
                    Contact = contact,
                    IsOnline = false,
                    Topics = new[]{5},
                    TargetAudience = new[]{201},
                    Status = EventStatus.Transferred,
                    ModifiedAt = DateTime.UtcNow
                }
            );
            await db.SaveChangesAsync();
        }
    }
}
