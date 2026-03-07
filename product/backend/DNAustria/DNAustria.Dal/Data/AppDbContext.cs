using System;
using System.Collections.Generic;
using DNAustria.Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace DNAustria.Dal.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventHistory> EventHistories { get; set; }

    public virtual DbSet<EventTargetAudience> EventTargetAudiences { get; set; }

    public virtual DbSet<EventTopic> EventTopics { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("citext");

        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("address_pkey");

            entity.ToTable("address");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .HasColumnName("city");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.State)
                .HasMaxLength(50)
                .HasColumnName("state");
            entity.Property(e => e.Street)
                .HasMaxLength(50)
                .HasColumnName("street");
            entity.Property(e => e.Zip)
                .HasMaxLength(10)
                .HasColumnName("zip");
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("contact_pkey");

            entity.ToTable("contact");

            entity.HasIndex(e => e.Email, "ux_contact_email_active")
                .IsUnique()
                .HasFilter("(is_deleted = false)");

            entity.HasIndex(e => e.Phone, "ux_contact_phone_active")
                .IsUnique()
                .HasFilter("(is_deleted = false)");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasColumnType("citext")
                .HasColumnName("email");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Organization)
                .HasMaxLength(50)
                .HasColumnName("organization");
            entity.Property(e => e.Phone).HasColumnName("phone");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("event_pkey");

            entity.ToTable("event");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AgeMaximum).HasColumnName("age_maximum");
            entity.Property(e => e.AgeMinimum).HasColumnName("age_minimum");
            entity.Property(e => e.Classification).HasColumnName("classification");
            entity.Property(e => e.Contact).HasColumnName("contact");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.Format)
                .HasMaxLength(100)
                .HasColumnName("format");
            entity.Property(e => e.HasFees).HasColumnName("has_fees");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.IsOnline).HasColumnName("is_online");
            entity.Property(e => e.Link)
                .HasMaxLength(200)
                .HasColumnName("link");
            entity.Property(e => e.Location).HasColumnName("location");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Organization).HasColumnName("organization");
            entity.Property(e => e.ProgramName)
                .HasMaxLength(50)
                .HasColumnName("program_name");
            entity.Property(e => e.SchoolBookable).HasColumnName("school_bookable");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.ContactNavigation).WithMany(p => p.Events)
                .HasForeignKey(d => d.Contact)
                .HasConstraintName("event_contact_fkey");

            entity.HasOne(d => d.LocationNavigation).WithMany(p => p.Events)
                .HasForeignKey(d => d.Location)
                .HasConstraintName("event_location_fkey");

            entity.HasOne(d => d.OrganizationNavigation).WithMany(p => p.Events)
                .HasForeignKey(d => d.Organization)
                .HasConstraintName("event_organization_fkey");
        });

        modelBuilder.Entity<EventHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("event_history_pkey");

            entity.ToTable("event_history");

            entity.HasIndex(e => e.CreatedAt, "ix_event_history_created_at");

            entity.HasIndex(e => e.EventId, "ix_event_history_event_id");

            entity.HasIndex(e => new { e.EventId, e.CreatedAt }, "ix_event_history_event_id_created_at").IsDescending(false, true);

            entity.HasIndex(e => e.UserId, "ix_event_history_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Action).HasColumnName("action");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Event).WithMany(p => p.EventHistories)
                .HasForeignKey(d => d.EventId)
                .HasConstraintName("fk_event_history_event");

            entity.HasOne(d => d.User).WithMany(p => p.EventHistories)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_event_history_user");
        });

        modelBuilder.Entity<EventTargetAudience>(entity =>
        {
            entity.HasKey(e => new { e.Event, e.TargetAudience }).HasName("event_target_audience_pkey");

            entity.ToTable("event_target_audience");

            entity.Property(e => e.Event).HasColumnName("event");
            entity.Property(e => e.TargetAudience).HasColumnName("target_audience");

            entity.HasOne(d => d.EventNavigation).WithMany(p => p.EventTargetAudiences)
                .HasForeignKey(d => d.Event)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("event_target_audience_event_fkey");
        });

        modelBuilder.Entity<EventTopic>(entity =>
        {
            entity.HasKey(e => new { e.Event, e.Topic }).HasName("event_topic_pkey");

            entity.ToTable("event_topic");

            entity.Property(e => e.Event).HasColumnName("event");
            entity.Property(e => e.Topic).HasColumnName("topic");

            entity.HasOne(d => d.EventNavigation).WithMany(p => p.EventTopics)
                .HasForeignKey(d => d.Event)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("event_topic_event_fkey");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("location_pkey");

            entity.ToTable("location");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Latitude).HasColumnName("latitude");
            entity.Property(e => e.Longitude).HasColumnName("longitude");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");

            entity.HasOne(d => d.AddressNavigation).WithMany(p => p.Locations)
                .HasForeignKey(d => d.Address)
                .HasConstraintName("location_address_fkey");
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("organization_pkey");

            entity.ToTable("organization");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Adress).HasColumnName("adress");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");

            entity.HasOne(d => d.AdressNavigation).WithMany(p => p.Organizations)
                .HasForeignKey(d => d.Adress)
                .HasConstraintName("organization_adress_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.ExternalId, "uq_users_external_id").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ExternalId).HasColumnName("external_id");
            entity.Property(e => e.Username).HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
