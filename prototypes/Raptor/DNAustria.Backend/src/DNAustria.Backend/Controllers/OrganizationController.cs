using DNAustria.Backend.Models;
using DNAustria.Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace DNAustria.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrganizationController : ControllerBase
{
    private readonly IOrganizationService _service;
    public OrganizationController(IOrganizationService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var o = await _service.GetAsync(id);
        if (o is null) return NotFound();
        return Ok(o);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Organization o)
    {
        var created = await _service.CreateAsync(o);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, Organization o)
    {
        var updated = await _service.UpdateAsync(id, o);
        if (updated is null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }
}