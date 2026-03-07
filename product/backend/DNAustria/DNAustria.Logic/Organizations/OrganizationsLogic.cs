using DNAustria.Dal.Data;
using DNAustria.Domain;
using DNAustria.Logic.Mapper;
using Microsoft.EntityFrameworkCore;

namespace DNAustria.Logic.Organizations;

public class OrganizationsLogic(AppDbContext db) : IOrganizationsLogic
{
    public IEnumerable<Organization> GetAllOrganizations()
    {
        return db.Organizations
            .Include(x => x.AdressNavigation)
            .Where(x => !x.IsDeleted)
            .Select(x => x.ToDomain());
    }

    public IEnumerable<Organization> GetOrganizationsByName(string name)
    {
        return db.Organizations
            .Include(x => x.AdressNavigation)
            .Where(x => x.Name.Contains(name) && !x.IsDeleted)
            .Select(x => x.ToDomain());
    }

    public Organization? GetOrganizationById(int id)
    {
        return db.Organizations
            .Include(x => x.AdressNavigation)
            .Where(x => x.Id == id)
            .Select(x => x.ToDomain())
            .FirstOrDefault();
    }

    public async Task<bool> DeleteOrganization(int id)
    {
        var orgToDelete = db.Organizations.FirstOrDefault(x => x.Id == id);
        
        if (orgToDelete == null) 
            return false;
        
        orgToDelete.IsDeleted = true;
        db.Organizations.Update(orgToDelete);
        return await db.SaveChangesAsync() > 0;

    }

    public async Task<(Organization? organization, string msg)> AddOrganization(Organization organization)
    {
        if (db.Organizations.Any(org => org.Name == organization.Name))
        {
            return (null, "Organization already exists");
        }
        
        var addedAddress = db.Addresses.Add(organization.Adress.ToEntity());
        var dalOrg = organization.ToEntity();
        dalOrg.AdressNavigation = addedAddress.Entity;
        var addedOrg = db.Organizations.Add(dalOrg);
        await db.SaveChangesAsync();
        
        return (addedOrg.Entity.ToDomain(), string.Empty);
    }

    public async Task<(Organization? organization, string msg)> UpdateOrganization(Organization organization)
    {
        var orgDal = db.Organizations
            .Include(x => x.AdressNavigation)
            .FirstOrDefault(x => x.Id == organization.Id);

        if (orgDal == null)
        {
            return (null, "Organization not found");
        }
        
        orgDal.Name = organization.Name;
        var updatedOrg = db.Organizations.Update(orgDal);
        await db.SaveChangesAsync();
        return (updatedOrg.Entity.ToDomain(), string.Empty);
    }
}