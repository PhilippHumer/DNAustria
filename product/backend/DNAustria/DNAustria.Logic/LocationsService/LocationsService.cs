using DNAustria.Dal.Data;
using DNAustria.Domain;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DNAustria.Logic.LocationsService;

public class LocationsService(AppDbContext db) : ILocationsService
{
    public async Task<List<Location>> GetAvailableLocations()
    {
        return await db.Locations
            .Include(x => x.AddressNavigation)
            .Select(x => x.ToDomainLocation())
            .ToListAsync();
    }

    public async Task<Location?> GetLocationById(int id)
    {
        return (await db.Locations
                .Include(x => x.AddressNavigation)
            .SingleOrDefaultAsync(x => x.Id == id))
            ?.ToDomainLocation() ?? null;
    }

    public async Task<(Location? item, string msg)> AddLocation(Location location)
    {
        if (string.IsNullOrWhiteSpace(location.Name))
            return (null, "location name is required");
        if (location.Latitude < -90 || location.Latitude > 90 || location.Longitude < -180 || location.Longitude > 180)
            return (null, "latitude has to be in range [-90;90] and longitude has to be in range [-180;180]");
        var mapped = location.ToDalLocation();
        try
        {
            if (location.Address != null)
            {
                var mappedAddress = location.Address.ToDalAddress();
                db.Addresses.Add(mappedAddress);
                mapped.AddressNavigation = mappedAddress;
            }
            db.Locations.Add(mapped);
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            Log.Error("Error while adding location: {msg} {stck}", ex.Message, ex.StackTrace);
            return (null, "error adding location, retry later");
        }
        return (mapped.ToDomainLocation(), string.Empty);
    }

    public async Task<(Location? item, string msg)> UpdateLocation(int locationId, Location location)
    {
        if(locationId <= 0 || string.IsNullOrWhiteSpace(location.Name))
            return (null, "location name is required");

        if (location.Latitude < -90 || location.Latitude > 90 || location.Longitude < -180 || location.Longitude > 180)
            return (null, "latitude has to be in range [-90;90] and longitude in [-180;180]");

        try
        {
            var existing = await db.Locations.Include(loc => loc.AddressNavigation!)
                .FirstOrDefaultAsync(l => l.Id == locationId);

            if (existing == null)
                return (null, "location not found");

            existing.Name = location.Name;
            existing.Latitude = location.Latitude;
            existing.Longitude = location.Longitude;

            if (location.Address != null)
            {
                if (existing.Address == null)
                {
                    var newAddress = location.Address.ToDalAddress();
                    existing.AddressNavigation = newAddress;
                    db.Addresses.Add(newAddress);
                }
                else
                {
                    existing.AddressNavigation!.Street = location.Address.Street;
                    existing.AddressNavigation!.City = location.Address.City;
                    existing.AddressNavigation!.Zip = location.Address.Zip;
                    existing.AddressNavigation!.State = location.Address.State;
                }
            }

            await db.SaveChangesAsync();

            return (existing.ToDomainLocation(), string.Empty);
        }
        catch (DbUpdateException ex)
        {
            Log.Error("Error while updating location: {msg}", ex.Message);
            return (null, "error updating location, retry later");
        }
    }
    public async Task<bool> DeleteLocation(int locationId)
    {
        if (locationId <= 0)
            return false;
        try
        {
            var entity = new Dal.Models.Location { Id = locationId };
            db.Entry(entity).State = EntityState.Deleted;
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            Log.Error("Error while deleting location: {msg}", ex.Message);
            return false;
        }
      
        return true;
    }
}