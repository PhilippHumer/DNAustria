using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("server/api/contacts")]
public class ContactsController(IContactService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var items = await service.ListAsync();
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ContactDto dto)
    {
        if(string.IsNullOrWhiteSpace(dto.Name)) return BadRequest("Name required");
        var created = await service.CreateAsync(dto);
        return Created($"/server/api/contacts/{created.Id}", created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ContactDto dto)
    {
        var updated = await service.UpdateAsync(id,dto);
        if(updated==null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var ok = await service.DeleteAsync(id);
        if(!ok) return NotFound();
        return NoContent();
    }
}
