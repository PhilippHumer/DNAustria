
using DNAustria.Domain;

namespace DNAustria.Logic.LocationsService;

/// <summary>
/// abstraction layer for defining location-related services
/// </summary>
public interface ILocationsService
{
    /// <summary>
    /// fetches all available locations
    /// </summary>
    /// <returns>all entries in the location table</returns>
    public Task<List<Location>> GetAvailableLocations();
    
    /// <summary>
    /// tries to fetch a single location by its id
    /// </summary>
    /// <param name="id">the unique identifier of the location</param>
    /// <returns>either a valid Location object if present or null otherwise</returns>
    public Task<Location?> GetLocationById(int id);

    /// <summary>
    /// adds a new location to the locations table
    /// </summary>
    /// <param name="location">data to create the new location</param>
    /// <returns>the created location</returns>
    public Task<(Location? item, string msg)> AddLocation(Location location);
    
    /// <summary>
    /// tries to update the location with given id
    /// </summary>
    /// <param name="locationId">the unique identifier of the location object to update</param>
    /// <param name="location">update data for the object</param>
    /// <returns>the updated location object</returns>
    public Task<(Location? item, string msg)> UpdateLocation(int locationId, Location location);
    
    /// <summary>
    /// tries to delete the location with given id
    /// </summary>
    /// <param name="locationId">the unique identifier of the location object to delete</param>
    /// <returns>true if the operation was successful, false otherwise</returns>
    public Task<bool> DeleteLocation(int locationId);
}