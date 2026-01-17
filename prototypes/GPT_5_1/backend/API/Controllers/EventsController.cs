using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers;

[ApiController]
[Route("server/api/events")]
public class EventsController(IEventService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventDto>>> List([FromQuery] string? search, [FromQuery] string? filter, [FromQuery] int page=1, [FromQuery] int pageSize=20)
    {
        var items= await service.ListAsync(search,filter,page,pageSize);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var item = await service.GetByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    [Authorize(Roles="Editor,Admin")]
    public async Task<IActionResult> Create([FromBody] EventDto dto)
    {
        if(string.IsNullOrWhiteSpace(dto.Title)) return BadRequest("Title required");
        var created = await service.CreateAsync(dto);
        return Created($"/server/api/events/{created.Id}", created);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles="Editor,Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] EventDto dto)
    {
        var updated = await service.UpdateAsync(id,dto);
        if(updated==null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles="Editor,Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var ok = await service.DeleteAsync(id);
        if(!ok) return NotFound();
        return NoContent();
    }

    [HttpPost("import")]
    [Authorize(Roles="Editor,Admin")]
    public async Task<IActionResult> Import([FromBody] string raw)
    {
        var imported = await service.ImportAsync(raw);
        return Ok(imported);
    }
}
