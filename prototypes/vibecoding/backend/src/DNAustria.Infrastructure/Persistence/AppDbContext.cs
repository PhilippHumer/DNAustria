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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Organization>(entity =>
        {
            entity.ToTable("organizations");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.ModifiedAt).HasColumnName("modified_at");

            // Partial unique index: name must be unique among active (non-deleted) organizations
            entity.HasIndex(e => e.Name)
                .IsUnique()
                .HasFilter("is_deleted = false");

            entity.HasIndex(e => e.IsDeleted);
        });
    }
}
