using DNAustria.Dal.Data;
using DNAustria.Domain;
using DNAustria.Logic.Mapper;
using Microsoft.EntityFrameworkCore;

namespace DNAustria.Logic.Organizations;

public class OrganizationsLogic(AppDbContext db) : IOrganizationsLogic
{
    public IEnumerable<Organization> GetAllOrganizations()
    {
        return db.Organizations.Select(x => x.ToDomain());
    }

    public IEnumerable<Organization> GetOrganizationsByName(string name)
    {
        return db.Organizations
            .Where(x => x.Name.Contains(name))
            .Select(x => x.ToDomain());
    }

    public Organization? GetOrganizationById(int id)
    {
        return db.Organizations
            .Where(x => x.Id == id)
            .Select(x => x.ToDomain())
            .FirstOrDefault();
    }

    public async Task<bool> DeleteOrganization(int id)
    {
        var orgToDelete = db.Organizations.FirstOrDefault(x => x.Id == id);
        if (orgToDelete != null) 
            db.Organizations.Remove(orgToDelete);
        return await db.SaveChangesAsync() > 0;
    }

    public async Task<Organization> AddOrganization(Organization organization)
    {
        var addedOrg = db.Organizations.Add(organization.ToEntity());
        await db.SaveChangesAsync();
        return addedOrg.Entity.ToDomain();
    }

    public async Task<Organization> UpdateOrganization(Organization organization)
    {
        var updatedOrg = db.Organizations.Update(organization.ToEntity());
        await db.SaveChangesAsync();
        return updatedOrg.Entity.ToDomain();
    }
}