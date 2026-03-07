using DNAustria.Api.Dtos;
using DNAustria.Api.Mapper;
using DNAustria.Domain;
using DNAustria.Logic.Organizations;
using Microsoft.AspNetCore.Mvc;

namespace DNAustria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrganizationsController(IOrganizationsLogic organizationsLogic) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetOrganizations([FromQuery] string? name)
    {
        IEnumerable<Organization> orgs;

        orgs = string.IsNullOrEmpty(name) 
            ? organizationsLogic.GetAllOrganizations() 
            : organizationsLogic.GetOrganizationsByName(name);

        return Ok(orgs.Select(org => org.ToDto())); 
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetOrganization(int id)
    {
        var org = organizationsLogic.GetOrganizationById(id);

        if (org == null)
        {
            return NotFound();
        }

        return Ok(org.ToDto());
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteOrganization(int id)
    {
        if (!await organizationsLogic.DeleteOrganization(id))
        {
            return NotFound();
        }
        
        return NoContent();
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> AddOrganization([FromBody] CreateOrganizationDto organizationDto)
    {
        var org = await organizationsLogic.AddOrganization(organizationDto.ToDomain());
        return Created("Organization was created", org.ToDto()); 
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOrganization(int id, [FromBody] OrganizationDto organizationDto)
    {
        var updatedOrg = await organizationsLogic.UpdateOrganization(organizationDto.ToDomain());
        return Created("Organization was updated", updatedOrg.ToDto());
    }
    
}