using DNAustria.API.Data;
using DNAustria.API.Models;
using DNAustria.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DNAustria.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrganizationsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly AddressService _addressService;

    public OrganizationsController(AppDbContext context, AddressService addressService)
    {
        _context = context;
        _addressService = addressService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Organization>>> GetOrganizations()
    {
        return await _context.Organizations.Include(o => o.Address).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Organization>> GetOrganization(Guid id)
    {
        var organization = await _context.Organizations
            .Include(o => o.Address)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (organization == null)
        {
            return NotFound();
        }

        return organization;
    }

    [HttpPost]
    public async Task<ActionResult<Organization>> CreateOrganization(Organization organization)
    {
        if (organization.Address != null)
        {
            var address = await _addressService.GetOrCreateAddressAsync(organization.Address);
            organization.AddressId = address.Id;
            organization.Address = null;
        }

        organization.Id = Guid.NewGuid();
        _context.Organizations.Add(organization);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetOrganization), new { id = organization.Id }, organization);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrganization(Guid id, Organization organization)
    {
        if (id != organization.Id)
        {
            return BadRequest();
        }

        if (organization.Address != null)
        {
            var address = await _addressService.GetOrCreateAddressAsync(organization.Address);
            organization.AddressId = address.Id;
            organization.Address = null;
        }

        _context.Entry(organization).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!OrganizationExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrganization(Guid id)
    {
        var organization = await _context.Organizations.FindAsync(id);
        if (organization == null)
        {
            return NotFound();
        }

        _context.Organizations.Remove(organization);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool OrganizationExists(Guid id)
    {
        return _context.Organizations.Any(e => e.Id == id);
    }
}
