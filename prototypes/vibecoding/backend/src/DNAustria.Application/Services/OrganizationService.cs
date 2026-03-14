using DNAustria.Application.DTOs;
using DNAustria.Application.Exceptions;
using DNAustria.Application.Interfaces;
using DNAustria.Domain.Entities;

namespace DNAustria.Application.Services;

public class OrganizationService(IOrganizationRepository repository) : IOrganizationService
{
    public async Task<IEnumerable<OrganizationDto>> GetAllAsync(string? nameFilter = null)
    {
        var organizations = await repository.GetAllAsync(nameFilter);
        return organizations.Select(ToDto);
    }

    public async Task<OrganizationDto> GetByIdAsync(Guid id)
    {
        var org = await repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Organization {id} not found.");
        return ToDto(org);
    }

    public async Task<OrganizationDto> CreateAsync(CreateOrganizationRequest request)
    {
        var name = request.Name.Trim();

        if (name.Length == 0)
            throw new ArgumentException("Name must not be empty.");
        if (name.Length > 50)
            throw new ArgumentException("Name must not exceed 50 characters.");

        if (await repository.ExistsWithNameAsync(name))
            throw new ConflictException($"An organization named '{name}' already exists.");

        var org = new Organization
        {
            Id = Guid.NewGuid(),
            Name = name,
            AddressId = request.AddressId,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };

        await repository.CreateAsync(org);
        return ToDto(org);
    }

    public async Task<OrganizationDto> UpdateAsync(Guid id, UpdateOrganizationRequest request)
    {
        var org = await repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Organization {id} not found.");

        var name = request.Name.Trim();

        if (name.Length == 0)
            throw new ArgumentException("Name must not be empty.");
        if (name.Length > 50)
            throw new ArgumentException("Name must not exceed 50 characters.");

        if (await repository.ExistsWithNameAsync(name, excludeId: id))
            throw new ConflictException($"An organization named '{name}' already exists.");

        org.Name = name;
        org.AddressId = request.AddressId;
        org.ModifiedAt = DateTime.UtcNow;

        await repository.UpdateAsync(org);
        return ToDto(org);
    }

    public async Task DeleteAsync(Guid id)
    {
        var org = await repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Organization {id} not found.");

        org.IsDeleted = true;
        org.ModifiedAt = DateTime.UtcNow;

        await repository.DeleteAsync(org);
    }

    private static OrganizationDto ToDto(Organization org) =>
        new(org.Id, org.Name, org.AddressId, org.CreatedAt, org.ModifiedAt);
}
