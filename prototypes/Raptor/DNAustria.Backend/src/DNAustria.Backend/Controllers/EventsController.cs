using DNAustria.Backend.Dtos;
using DNAustria.Backend.Models;
using DNAustria.Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace DNAustria.Backend.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController : ControllerBase
{
    private readonly IEventService _service;
    public EventsController(IEventService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] EventStatus? status, [FromQuery] string? q)
    {
        var list = await _service.GetEventsAsync(status, q);
        return Ok(list);
    }

    [HttpGet("export/approved")]
    public async Task<IActionResult> ExportApproved()
    {
        var result = await _service.GetApprovedEventsExportAsync();
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var ev = await _service.GetEventAsync(id);
        if (ev is null) return NotFound();
        return Ok(ev);
    }

    [HttpPost]
    public async Task<IActionResult> Create(EventCreateDto dto)
    {
        var created = await _service.CreateEventAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, EventCreateDto dto)
    {
        var updated = await _service.UpdateEventAsync(id, dto);
        if (updated is null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var ok = await _service.DeleteEventAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> PatchStatus(Guid id, [FromQuery] EventStatus status)
    {
        var ok = await _service.UpdateStatusAsync(id, status);
        if (!ok) return BadRequest("Unable to update status. Possibly already transferred or not found.");
        return NoContent();
    }

    [HttpPost("import")]
    public async Task<IActionResult> Import([FromBody] ImportRequest request)
    {
        var parsed = await _service.ParseEventAsync(request.Content, request.IsHtml);
        return Ok(parsed);
    }
}

public record ImportRequest(string Content, bool IsHtml = false);