using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Infrastructure.Persistence;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(AppDbContext))]
    public class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "9.0.1");

            modelBuilder.Entity("Domain.Entities.Organization", b =>
            {
                b.Property<Guid>("Id").HasColumnType("uuid");
                b.Property<string>("Name").IsRequired().HasColumnType("text");
                b.Property<string>("AddressStreet").HasColumnType("text");
                b.Property<string>("AddressCity").HasColumnType("text");
                b.Property<string>("AddressZip").HasColumnType("text");
                b.Property<int?>("RegionId").HasColumnType("integer");
                b.HasKey("Id");
                b.ToTable("Organizations");
            });

            modelBuilder.Entity("Domain.Entities.Contact", b =>
            {
                b.Property<Guid>("Id").HasColumnType("uuid");
                b.Property<string>("Name").IsRequired().HasColumnType("text");
                b.Property<string>("Email").HasColumnType("text");
                b.Property<string>("Phone").HasColumnType("text");
                b.Property<Guid?>("OrganizationId").HasColumnType("uuid");
                b.HasKey("Id");
                b.HasIndex("OrganizationId");
                b.ToTable("Contacts");
            });

            modelBuilder.Entity("Domain.Entities.Event", b =>
            {
                b.Property<Guid>("Id").HasColumnType("uuid");
                b.Property<string>("Title").IsRequired().HasColumnType("text");
                b.Property<string>("Description").HasColumnType("text");
                b.Property<int[]>("Topics").HasColumnType("integer[]");
                b.Property<DateTime?>("DateStart").HasColumnType("timestamp with time zone");
                b.Property<DateTime?>("DateEnd").HasColumnType("timestamp with time zone");
                b.Property<Guid?>("OrganizationId").HasColumnType("uuid");
                b.Property<Guid?>("ContactId").HasColumnType("uuid");
                b.Property<int[]>("TargetAudience").HasColumnType("integer[]");
                b.Property<bool>("IsOnline").HasColumnType("boolean");
                b.Property<string>("EventLink").HasColumnType("text");
                b.Property<int>("Status").HasColumnType("integer");
                b.Property<string>("CreatedBy").HasColumnType("text");
                b.Property<string>("ModifiedBy").HasColumnType("text");
                b.Property<DateTime>("ModifiedAt").HasColumnType("timestamp with time zone");
                b.HasKey("Id");
                b.HasIndex("ContactId");
                b.HasIndex("OrganizationId");
                b.ToTable("Events");
            });
        }
    }
}
