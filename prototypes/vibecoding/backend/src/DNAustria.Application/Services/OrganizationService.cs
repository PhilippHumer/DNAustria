using DNAustria.Application.DTOs;
using DNAustria.Application.Exceptions;
using DNAustria.Application.Interfaces;
using DNAustria.Domain.Entities;

namespace DNAustria.Application.Services;

public class OrganizationService
{
    private readonly IOrganizationRepository _repository;

    public OrganizationService(IOrganizationRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<OrganizationDto>> GetAllAsync(string? nameFilter, CancellationToken ct = default)
    {
        var orgs = await _repository.GetAllAsync(nameFilter?.Trim(), ct);
        return orgs.Select(ToDto).ToList();
    }

    public async Task<OrganizationDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var org = await _repository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Organization {id} not found.");
        return ToDto(org);
    }

    public async Task<OrganizationDto> CreateAsync(CreateOrganizationRequest request, CancellationToken ct = default)
    {
        var name = request.Name?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(name) || name.Length > 50)
            throw new ArgumentException("Name is required and must be at most 50 characters.");

        if (await _repository.ExistsByNameAsync(name, ct: ct))
            throw new ConflictException($"Organization with name '{name}' already exists.");

        var org = new Organization
        {
            Id = Guid.NewGuid(),
            Name = name,
            AddressId = request.AddressId,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(org, ct);
        await _repository.SaveChangesAsync(ct);

        return ToDto(org);
    }

    public async Task<OrganizationDto> UpdateAsync(Guid id, UpdateOrganizationRequest request, CancellationToken ct = default)
    {
        var org = await _repository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Organization {id} not found.");

        var name = request.Name?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(name) || name.Length > 50)
            throw new ArgumentException("Name is required and must be at most 50 characters.");

        if (await _repository.ExistsByNameAsync(name, excludeId: id, ct: ct))
            throw new ConflictException($"Organization with name '{name}' already exists.");

        org.Name = name;
        org.AddressId = request.AddressId;
        org.ModifiedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync(ct);

        return ToDto(org);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var org = await _repository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Organization {id} not found.");

        org.IsDeleted = true;
        org.ModifiedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync(ct);
    }

    private static OrganizationDto ToDto(Organization org) =>
        new(org.Id, org.Name, org.AddressId, org.CreatedAt, org.ModifiedAt);
}
