using DNAustria.Domain;

namespace DNAustria.Logic.Organizations;

public interface IOrganizationsLogic
{
    IEnumerable<Organization> GetAllOrganizations();
    IEnumerable<Organization> GetOrganizationsByName(string name);
    Organization? GetOrganizationById(int id);
    Task<bool> DeleteOrganization(int id);
    Task<Organization> AddOrganization(Organization organization);
    Task<Organization> UpdateOrganization(Domain.Organization organization);
}