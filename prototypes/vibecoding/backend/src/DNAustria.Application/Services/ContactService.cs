using DNAustria.Application.DTOs;
using DNAustria.Application.Exceptions;
using DNAustria.Application.Interfaces;
using DNAustria.Domain.Entities;

namespace DNAustria.Application.Services;

public class ContactService
{
    private readonly IContactRepository _repository;

    public ContactService(IContactRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ContactDto>> GetAllAsync(string? nameFilter, CancellationToken ct = default)
    {
        var contacts = await _repository.GetAllAsync(nameFilter?.Trim(), ct);
        return contacts.Select(ToDto).ToList();
    }

    public async Task<ContactDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var contact = await _repository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Contact {id} not found.");
        return ToDto(contact);
    }

    public async Task<ContactDto> CreateAsync(CreateContactRequest request, CancellationToken ct = default)
    {
        var (name, email, phone, org) = ValidateAndTrim(request.Name, request.Email, request.Phone, request.Org);

        ValidateOrgConsistency(request.OrganizationId, org);

        if (await _repository.ExistsByNameAsync(name, ct: ct))
            throw new ConflictException($"Contact with name '{name}' already exists.");

        var contact = new Contact
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            Phone = phone,
            OrganizationId = request.OrganizationId,
            Org = org,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
        };

        await _repository.AddAsync(contact, ct);
        await _repository.SaveChangesAsync(ct);

        return ToDto(contact);
    }

    public async Task<ContactDto> UpdateAsync(Guid id, UpdateContactRequest request, CancellationToken ct = default)
    {
        var contact = await _repository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Contact {id} not found.");

        var (name, email, phone, org) = ValidateAndTrim(request.Name, request.Email, request.Phone, request.Org);

        ValidateOrgConsistency(request.OrganizationId, org);

        if (await _repository.ExistsByNameAsync(name, excludeId: id, ct: ct))
            throw new ConflictException($"Contact with name '{name}' already exists.");

        contact.Name = name;
        contact.Email = email;
        contact.Phone = phone;
        contact.OrganizationId = request.OrganizationId;
        contact.Org = org;
        contact.ModifiedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync(ct);

        return ToDto(contact);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var contact = await _repository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Contact {id} not found.");

        contact.IsDeleted = true;
        contact.ModifiedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync(ct);
    }

    private static void ValidateOrgConsistency(Guid? organizationId, string? org)
    {
        if (organizationId.HasValue && org == null)
            throw new ArgumentException("Org must be set when OrganizationId is provided.");
        if (!organizationId.HasValue && org != null)
            throw new ArgumentException("OrganizationId must be set when Org is provided.");
    }

    private static (string name, string email, string phone, string? org) ValidateAndTrim(
        string? rawName, string? rawEmail, string? rawPhone, string? rawOrg)
    {
        var name = rawName?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(name) || name.Length > 50)
            throw new ArgumentException("Name is required and must be at most 50 characters.");

        var email = rawEmail?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(email) || email.Length > 100)
            throw new ArgumentException("Email is required and must be at most 100 characters.");
        if (!email.Contains('@'))
            throw new ArgumentException("Email must be a valid email address.");

        var phone = rawPhone?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(phone) || phone.Length > 50)
            throw new ArgumentException("Phone is required and must be at most 50 characters.");

        var org = rawOrg?.Trim();
        if (org != null && org.Length > 50)
            throw new ArgumentException("Org must be at most 50 characters.");

        return (name, email, phone, string.IsNullOrEmpty(org) ? null : org);
    }

    private static ContactDto ToDto(Contact c) =>
        new(c.Id, c.Name, c.OrganizationId, c.Org, c.Email, c.Phone, c.CreatedAt, c.ModifiedAt);
}
