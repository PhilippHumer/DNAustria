using DNAustria.Api.Dtos.Contacts;
using DNAustria.Domain;
using DNAustria.Logic;
using Microsoft.AspNetCore.Mvc;

namespace DNAustria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactsController(IContactsLogic contactLogic) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IEnumerable<ContactDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var contacts = await contactLogic.GetAllAsync();
        return Ok(contacts.ToDtoCollection());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<ContactDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var contact = await contactLogic.GetByIdAsync(id);
        if (contact is null) return NotFound();
        return Ok(contact.ToDto());
    }

    [HttpPost]
    [ProducesResponseType<ContactDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateContactDto dto)
    {
        var created = await contactLogic.CreateAsync(dto.ToDomain());
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToDto());
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType<ContactDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateContactDto dto)
    {
        var contact = dto.ToDomain();
        contact = contact with { Id = id };
        var updated = await contactLogic.UpdateAsync(contact);
        return Ok(updated.ToDto());
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await contactLogic.DeleteAsync(id);
        return NoContent();
    }
    
}