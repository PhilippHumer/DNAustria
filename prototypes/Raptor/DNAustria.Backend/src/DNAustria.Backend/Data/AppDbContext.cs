using DNAustria.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace DNAustria.Backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Event> Events => Set<Event>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Organization> Organizations => Set<Organization>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>(eb =>
        {
            eb.HasKey(e => e.Id);
            eb.HasOne(e => e.Address).WithMany().HasForeignKey(e => e.LocationId).OnDelete(DeleteBehavior.SetNull);
            eb.HasOne(e => e.Contact).WithMany().HasForeignKey(e => e.ContactId).OnDelete(DeleteBehavior.SetNull);

            // Persist enum lists as JSON
            eb.Property(e => e.TargetAudience).HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, new System.Text.Json.JsonSerializerOptions()),
                v => System.Text.Json.JsonSerializer.Deserialize<List<TargetAudience>>(v, new System.Text.Json.JsonSerializerOptions()) ?? new List<TargetAudience>());

            eb.Property(e => e.Topics).HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, new System.Text.Json.JsonSerializerOptions()),
                v => System.Text.Json.JsonSerializer.Deserialize<List<EventTopic>>(v, new System.Text.Json.JsonSerializerOptions()) ?? new List<EventTopic>());
        });

        modelBuilder.Entity<Address>(ab =>
        {
            ab.HasKey(a => a.Id);
            ab.HasIndex(a => new { a.Zip, a.Latitude, a.Longitude });
        });

        modelBuilder.Entity<Contact>(cb =>
        {
            cb.HasKey(c => c.Id);
            cb.HasIndex(c => c.Email).IsUnique(false);
        });

        modelBuilder.Entity<Organization>(ob => ob.HasKey(o => o.Id));

        base.OnModelCreating(modelBuilder);
    }
}