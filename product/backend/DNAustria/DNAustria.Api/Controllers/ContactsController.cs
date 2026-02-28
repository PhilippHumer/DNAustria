using DNAustria.Api.Dtos.Contacts;
using DNAustria.Domain;
using DNAustria.Logic;
using Microsoft.AspNetCore.Mvc;

namespace DNAustria.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ContactsController : ControllerBase
{
    private readonly IContactsLogic _contactLogic;

    public ContactsController(IContactsLogic contactLogic)
    {
        _contactLogic = contactLogic;
    }

    [HttpGet]
    [ProducesResponseType<IEnumerable<ContactDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var contacts = await _contactLogic.GetAllAsync();
        return Ok(contacts.Select(MapToDto));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<ContactDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var contact = await _contactLogic.GetByIdAsync(id);
        if (contact is null) return NotFound();
        return Ok(MapToDto(contact));
    }

    [HttpPost]
    [ProducesResponseType<ContactDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateContactDto dto)
    {
        try
        {
            var created = await _contactLogic.AddAsync(dto.Name, dto.Email, dto.PhoneNumber, dto.OrganisationId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToDto(created));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<ContactDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateContactDto dto)
    {
        try
        {
            var updated = await _contactLogic.UpdateAsync(id, dto.Name, dto.Email, dto.PhoneNumber, dto.OrganisationId);
            return Ok(MapToDto(updated));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _contactLogic.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    private static ContactDto MapToDto(Contact c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Email = c.Email,
        PhoneNumber = c.PhoneNumber,
        Organization = c.Organization
    };
}