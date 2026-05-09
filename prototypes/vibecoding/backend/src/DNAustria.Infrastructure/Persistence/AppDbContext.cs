using DNAustria.Domain.Entities;
using DNAustria.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DNAustria.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Event> Events => Set<Event>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Organization>(entity =>
        {
            entity.ToTable("organizations");
            entity.HasKey(o => o.Id);
            entity.Property(o => o.Id).HasColumnName("id");
            entity.Property(o => o.Name).HasColumnName("name").IsRequired().HasMaxLength(50);
            entity.Property(o => o.AddressId).HasColumnName("address_id");
            entity.Property(o => o.IsDeleted).HasColumnName("is_deleted");
            entity.Property(o => o.CreatedAt).HasColumnName("created_at");
            entity.Property(o => o.ModifiedAt).HasColumnName("modified_at");
            entity.HasIndex(o => o.IsDeleted);
            entity.HasIndex(o => o.Name)
                .IsUnique()
                .HasFilter("is_deleted = false");
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.ToTable("contacts");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).HasColumnName("id");
            entity.Property(c => c.Name).HasColumnName("name").IsRequired().HasMaxLength(50);
            entity.Property(c => c.Email).HasColumnName("email").IsRequired().HasMaxLength(100);
            entity.Property(c => c.Phone).HasColumnName("phone").IsRequired().HasMaxLength(50);
            entity.Property(c => c.OrganizationId).HasColumnName("organization_id");
            entity.Property(c => c.Org).HasColumnName("org").HasMaxLength(50);
            entity.Property(c => c.CreatedBy).HasColumnName("created_by");
            entity.Property(c => c.ModifiedBy).HasColumnName("modified_by");
            entity.Property(c => c.CreatedAt).HasColumnName("created_at");
            entity.Property(c => c.ModifiedAt).HasColumnName("modified_at");
            entity.Property(c => c.IsDeleted).HasColumnName("is_deleted");
            entity.HasIndex(c => c.IsDeleted);
            entity.HasIndex(c => c.Name)
                .IsUnique()
                .HasFilter("is_deleted = false");
        });

        modelBuilder.Entity<Address>(entity =>
        {
            entity.ToTable("addresses");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).HasColumnName("id");
            entity.Property(a => a.LocationName).HasColumnName("location_name").IsRequired().HasMaxLength(50);
            entity.Property(a => a.Street).HasColumnName("street").IsRequired().HasMaxLength(50);
            entity.Property(a => a.City).HasColumnName("city").IsRequired().HasMaxLength(50);
            entity.Property(a => a.Zip).HasColumnName("zip").IsRequired().HasMaxLength(10);
            entity.Property(a => a.State).HasColumnName("state").IsRequired().HasMaxLength(50);
            entity.Property(a => a.Latitude).HasColumnName("latitude").HasColumnType("numeric(9,6)");
            entity.Property(a => a.Longitude).HasColumnName("longitude").HasColumnType("numeric(9,6)");
            entity.Property(a => a.IsDeleted).HasColumnName("is_deleted");
            entity.Property(a => a.CreatedAt).HasColumnName("created_at");
            entity.Property(a => a.ModifiedAt).HasColumnName("modified_at");
            entity.HasIndex(a => a.IsDeleted);
            entity.HasIndex(a => new { a.Zip, a.Latitude, a.Longitude })
                .IsUnique()
                .HasFilter("is_deleted = false");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.ToTable("locations");
            entity.HasKey(l => l.Id);
            entity.Property(l => l.Id).HasColumnName("id");
            entity.Property(l => l.Name).HasColumnName("name").IsRequired().HasMaxLength(50);
            entity.Property(l => l.AddressId).HasColumnName("address_id").IsRequired();
            entity.Property(l => l.IsDeleted).HasColumnName("is_deleted");
            entity.Property(l => l.CreatedAt).HasColumnName("created_at");
            entity.Property(l => l.ModifiedAt).HasColumnName("modified_at");
            entity.HasIndex(l => l.IsDeleted);
            entity.HasOne(l => l.Address)
                .WithMany()
                .HasForeignKey(l => l.AddressId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.ToTable("events");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasColumnName("title").IsRequired().HasMaxLength(255);
            entity.Property(e => e.Description).HasColumnName("description").IsRequired();
            entity.Property(e => e.EventLink).HasColumnName("event_link");
            entity.Property(e => e.TargetAudience).HasColumnName("target_audience").HasColumnType("integer[]");
            entity.Property(e => e.Topics).HasColumnName("topics").HasColumnType("integer[]");
            entity.Property(e => e.DateStart).HasColumnName("date_start");
            entity.Property(e => e.DateEnd).HasColumnName("date_end");
            entity.Property(e => e.Classification).HasColumnName("classification").HasConversion<string>();
            entity.Property(e => e.Fees).HasColumnName("fees");
            entity.Property(e => e.IsOnline).HasColumnName("is_online");
            entity.Property(e => e.OrganizationId).HasColumnName("organization_id");
            entity.Property(e => e.ProgramName).HasColumnName("program_name");
            entity.Property(e => e.Format).HasColumnName("format");
            entity.Property(e => e.SchoolBookable).HasColumnName("school_bookable");
            entity.Property(e => e.AgeMinimum).HasColumnName("age_minimum");
            entity.Property(e => e.AgeMaximum).HasColumnName("age_maximum");
            entity.Property(e => e.LocationId).HasColumnName("location_id");
            entity.Property(e => e.ContactId).HasColumnName("contact_id");
            entity.Property(e => e.Status).HasColumnName("status").HasConversion<string>().HasDefaultValue(EventStatus.Draft);
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.ModifiedAt).HasColumnName("modified_at");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
            entity.HasIndex(e => e.IsDeleted);
            entity.HasIndex(e => e.Status);
        });
    }
}
