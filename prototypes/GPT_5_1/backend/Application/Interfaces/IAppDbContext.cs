using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces;

public interface IAppDbContext
{
    DbSet<Event> Events { get; }
    DbSet<Contact> Contacts { get; }
    DbSet<Organization> Organizations { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

