using DNAustria.Application.DTOs;
using DNAustria.Application.Exceptions;
using DNAustria.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace DNAustria.Api.Controllers;

[ApiController]
[Route("api/contacts")]
public class ContactsController : ControllerBase
{
    private readonly ContactService _service;

    public ContactsController(ContactService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? name, CancellationToken ct)
    {
        var result = await _service.GetAllAsync(name, ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        try
        {
            var result = await _service.GetByIdAsync(id, ct);
            return Ok(result);
        }
        catch (NotFoundException e)
        {
            return NotFound(new { error = e.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateContactRequest request, CancellationToken ct)
    {
        try
        {
            var result = await _service.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { error = e.Message });
        }
        catch (ConflictException e)
        {
            return Conflict(new { error = e.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateContactRequest request, CancellationToken ct)
    {
        try
        {
            var result = await _service.UpdateAsync(id, request, ct);
            return Ok(result);
        }
        catch (NotFoundException e)
        {
            return NotFound(new { error = e.Message });
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { error = e.Message });
        }
        catch (ConflictException e)
        {
            return Conflict(new { error = e.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        try
        {
            await _service.DeleteAsync(id, ct);
            return NoContent();
        }
        catch (NotFoundException e)
        {
            return NotFound(new { error = e.Message });
        }
    }
}
