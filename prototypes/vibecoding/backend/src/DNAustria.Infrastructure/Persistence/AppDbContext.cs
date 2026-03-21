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
    }
}
