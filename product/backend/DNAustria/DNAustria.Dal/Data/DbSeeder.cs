using Bogus;
using DNAustria.Dal.Models;

namespace DNAustria.Dal.Data;

public static class DbSeeder
{
    private static readonly string[] AustrianStates =
    [
        "Wien", "Niederösterreich", "Oberösterreich", "Salzburg",
        "Tirol", "Vorarlberg", "Kärnten", "Steiermark", "Burgenland"
    ];

    private static readonly string[] EventFormats =
    [
        "Workshop", "Vortrag", "Seminar", "Konferenz", "Webinar",
        "Laborführung", "Exkursion", "Podiumsdiskussion", "Hackathon", "Filmvorführung"
    ];

    private static readonly string[] ProgramNames =
    [
        "DNA erleben", "BioLab", "Science Talk", "GenomAustria",
        "MINT fördern", "Forschung live", "Wissen schafft",
        "Labor erleben", "Zukunft Bio", "Technik trifft Natur"
    ];

    private static readonly string[] EventNamePrefixes =
    [
        "DNA-Analyse", "Genetik", "Biotechnologie", "Mikrobiologie",
        "Molekularbiologie", "Genomforschung", "Bioinformatik",
        "Zellbiologie", "Ökologie", "Evolution", "Biochemie",
        "Stammzellforschung", "CRISPR", "PCR-Technik", "Bioethik"
    ];

    private static readonly string[] EventNameSuffixes =
    [
        "für Einsteiger", "Intensivkurs", "Praxisworkshop",
        "im Dialog", "hautnah", "verstehen", "erleben",
        "für Schulen", "für alle", "Masterclass"
    ];

    private static readonly string[] HistoryActions =
    [
        "Created", "Updated", "Published", "StatusChanged", "Reviewed"
    ];

    public static async Task SeedAsync(AppDbContext context)
    {
        // Alle bestehenden Daten löschen (in richtiger Reihenfolge wegen FK-Constraints)
        context.EventHistories.RemoveRange(context.EventHistories);
        context.EventTopics.RemoveRange(context.EventTopics);
        context.EventTargetAudiences.RemoveRange(context.EventTargetAudiences);
        context.Events.RemoveRange(context.Events);
        context.Contacts.RemoveRange(context.Contacts);
        context.Locations.RemoveRange(context.Locations);
        context.Organizations.RemoveRange(context.Organizations);
        context.Addresses.RemoveRange(context.Addresses);
        context.Users.RemoveRange(context.Users);
        await context.SaveChangesAsync();

        Randomizer.Seed = new Random(42);
        var faker = new Faker("de");

        // --- Addresses ---
        var addresses = new Faker<Address>("de")
            .RuleFor(a => a.Street, f => f.Address.StreetAddress())
            .RuleFor(a => a.City, f => f.Address.City())
            .RuleFor(a => a.Zip, f => f.Address.ZipCode("####"))
            .RuleFor(a => a.State, f => f.PickRandom(AustrianStates))
            .RuleFor(a => a.IsDeleted, false)
            .Generate(20);

        context.Addresses.AddRange(addresses);
        await context.SaveChangesAsync();

        // --- Organizations ---
        var organizations = new Faker<Organization>("de")
            .RuleFor(o => o.Name, f => { var n = f.Company.CompanyName(); return n[..Math.Min(n.Length, 50)]; })
            .RuleFor(o => o.Adress, f => f.PickRandom(addresses).Id)
            .RuleFor(o => o.IsDeleted, false)
            .Generate(10);

        context.Organizations.AddRange(organizations);
        await context.SaveChangesAsync();

        // --- Locations ---
        var locations = new Faker<Location>("de")
            .RuleFor(l => l.Name, f => { var n = f.Company.CompanyName(); return n[..Math.Min(n.Length, 50)]; })
            .RuleFor(l => l.Address, f => f.PickRandom(addresses).Id)
            .RuleFor(l => l.Latitude, f => f.Address.Latitude(46.3, 48.9))
            .RuleFor(l => l.Longitude, f => f.Address.Longitude(9.5, 17.2))
            .RuleFor(l => l.IsDeleted, false)
            .Generate(15);

        context.Locations.AddRange(locations);
        await context.SaveChangesAsync();

        // --- Contacts ---
        var contactIndex = 0;
        var contacts = new Faker<Contact>("de")
            .RuleFor(c => c.Name, f => { var n = f.Name.FullName(); return n[..Math.Min(n.Length, 50)]; })
            .RuleFor(c => c.Email, f => f.Internet.Email())
            .RuleFor(c => c.Phone, _ => $"+43{++contactIndex:D9}")
            .RuleFor(c => c.Organization, f => f.PickRandom(organizations).Name)
            .RuleFor(c => c.IsDeleted, false)
            .Generate(15);

        context.Contacts.AddRange(contacts);
        await context.SaveChangesAsync();

        // --- Users ---
        var users = new Faker<User>("de")
            .RuleFor(u => u.ExternalId, f => f.Random.Guid().ToString())
            .RuleFor(u => u.Username, f => f.Internet.UserName())
            .Generate(5);

        context.Users.AddRange(users);
        await context.SaveChangesAsync();

        // --- Events ---
        var events = new Faker<Event>("de")
            .RuleFor(e => e.Name, f =>
            {
                var name = $"{f.PickRandom(EventNamePrefixes)} {f.PickRandom(EventNameSuffixes)}";
                return name[..Math.Min(name.Length, 50)];
            })
            .RuleFor(e => e.Description, f => f.Lorem.Paragraphs(2))
            .RuleFor(e => e.Link, f => { var u = f.Internet.Url(); return u[..Math.Min(u.Length, 200)]; })
            .RuleFor(e => e.StartDate, f => f.Date.Between(DateTime.UtcNow, DateTime.UtcNow.AddMonths(6)))
            .RuleFor(e => e.EndDate, (f, e) => e.StartDate.AddHours(f.Random.Int(1, 48)))
            .RuleFor(e => e.Classification, f => f.Random.Int(0, 1))
            .RuleFor(e => e.Status, f => f.Random.Int(0, 2))
            .RuleFor(e => e.HasFees, f => f.Random.Bool())
            .RuleFor(e => e.IsOnline, f => f.Random.Bool())
            .RuleFor(e => e.Organization, f => f.PickRandom(organizations).Id)
            .RuleFor(e => e.Location, f => f.PickRandom(locations).Id)
            .RuleFor(e => e.Contact, f => f.PickRandom(contacts).Id)
            .RuleFor(e => e.ProgramName, f => f.PickRandom(ProgramNames))
            .RuleFor(e => e.Format, f => f.PickRandom(EventFormats))
            .RuleFor(e => e.SchoolBookable, f => f.Random.Bool())
            .RuleFor(e => e.AgeMinimum, f => f.Random.Int(6, 14))
            .RuleFor(e => e.AgeMaximum, (f, e) => f.Random.Int(e.AgeMinimum, 99))
            .RuleFor(e => e.IsDeleted, false)
            .Generate(50);

        context.Events.AddRange(events);
        await context.SaveChangesAsync();

        // --- EventTargetAudiences ---
        var targetAudiences = new List<EventTargetAudience>();
        foreach (var ev in events)
        {
            var count = faker.Random.Int(1, 3);
            //nur 10er schritte erlaubt
            var audiences = faker.Random.ListItems(
                new List<int> { 10, 20, 30, 40, 50, 60, 70, 80 }, count);
            foreach (var audience in audiences)
            {
                targetAudiences.Add(new EventTargetAudience
                {
                    Event = ev.Id,
                    TargetAudience = audience
                });
            }
        }

        context.EventTargetAudiences.AddRange(targetAudiences);
        await context.SaveChangesAsync();

        // --- EventTopics ---
        var topics = new List<EventTopic>();
        foreach (var ev in events)
        {
            var count = faker.Random.Int(1, 4);
            //nur 100er schritte erlaubt
            var topicIds = faker.Random.ListItems(
                new List<int> { 100, 200, 300, 400, 500, 600, 700, 800 }, count);

            foreach (var topicId in topicIds)
            {
                topics.Add(new EventTopic
                {
                    Event = ev.Id,
                    Topic = topicId
                });
            }
        }

        context.EventTopics.AddRange(topics);
        await context.SaveChangesAsync();

        // --- EventHistories ---
        var histories = new List<EventHistory>();
        foreach (var ev in events)
        {
            var count = faker.Random.Int(1, 3);
            for (var i = 0; i < count; i++)
            {
                histories.Add(new EventHistory
                {
                    EventId = ev.Id,
                    UserId = faker.PickRandom(users).Id,
                    Action = faker.PickRandom(HistoryActions),
                    CreatedAt = faker.Date.Between(
                        DateTime.UtcNow.AddMonths(-3), DateTime.UtcNow)
                });
            }
        }

        context.EventHistories.AddRange(histories);
        await context.SaveChangesAsync();
    }
}
