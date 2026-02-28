using DNAustria.Dal.Data;
using DNAustria.Dal.Models;
using Microsoft.AspNetCore.Mvc;

namespace DNAustria.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class LocationsController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        context.Locations.Add(new Location()
        {
            Events= [],
            Latitude = 12,
            Longitude = 123,
            Name = "asdf"
        });
        context.SaveChanges();
        return Ok(context.Locations.ToList());
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Get(Guid id)
    {
        var found = false;
        if (!found)
            return NotFound("...");
        return Ok("...");
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        var found = true;
        if (!found)
            return NotFound("...");
        return Ok("...");
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Post([FromBody] int a)
    {
        return Ok();
    }
    
    
}