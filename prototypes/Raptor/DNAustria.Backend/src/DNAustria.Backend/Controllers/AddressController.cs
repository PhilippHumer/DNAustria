using DNAustria.Backend.Models;
using DNAustria.Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace DNAustria.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AddressController : ControllerBase
{
    private readonly IAddressService _service;
    public AddressController(IAddressService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var a = await _service.GetAsync(id);
        if (a is null) return NotFound();
        return Ok(a);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Address a)
    {
        var created = await _service.CreateAsync(a);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, Address a)
    {
        var updated = await _service.UpdateAsync(id, a);
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