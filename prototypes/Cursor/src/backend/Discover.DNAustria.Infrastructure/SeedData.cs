using Discover.DNAustria.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Discover.DNAustria.Infrastructure
{
    public static class SeedData
    {
        public static void EnsureSeeded(AppDbContext db)
        {
            db.Database.Migrate();
            if (!db.Organizations.Any())
            {
                var orgId = Guid.NewGuid();
                var contactId = Guid.NewGuid();
                db.Organizations.Add(new Organization {
                    Id = orgId,
                    Name = "FH Upper Austria",
                    AddressStreet = "Stelzhamerstraße 23",
                    AddressCity = "Wels",
                    AddressZip = "4600",
                    RegionId = 4 });
                db.Contacts.Add(new Contact {
                    Id = contactId,
                    Name = "Maria Musterfrau",
                    Email = "maria.musterfrau@fh-ooe.at",
                    Phone = "+43 7242 12345",
                    OrganizationId = orgId });
                db.Events.AddRange(
                new Event {
                    Id = Guid.NewGuid(), Title = "DNAustria Conference", Description = "Annual educational DNA conference.", Topics = new List<int>{101,102}, DateStart = DateTime.UtcNow.AddDays(10), DateEnd = DateTime.UtcNow.AddDays(11), OrganizationId = orgId, ContactId = contactId, TargetAudience = new List<int>{1,2}, IsOnline = false, EventLink = null, Status = EventStatus.Approved, CreatedBy = "system", ModifiedBy = "system", ModifiedAt = DateTime.UtcNow },
                new Event {
                    Id = Guid.NewGuid(), Title = "Workshop: Biotech Essentials", Description = "Basic hands-on biotechnology course.", Topics = new List<int>{201,202}, DateStart = DateTime.UtcNow.AddDays(30), DateEnd = DateTime.UtcNow.AddDays(31), OrganizationId = orgId, ContactId = contactId, TargetAudience = new List<int>{3}, IsOnline = true, EventLink = "https://fh-ooe.at/biotech-essentials", Status = EventStatus.Draft, CreatedBy = "system", ModifiedBy = "system", ModifiedAt = DateTime.UtcNow }
                    );
                db.SaveChanges();
            }
        }
    }
}

