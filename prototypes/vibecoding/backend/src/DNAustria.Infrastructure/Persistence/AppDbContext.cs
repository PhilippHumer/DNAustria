using DNAustria.Domain.Entities;
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
    }
}
