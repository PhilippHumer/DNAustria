using DNAustria.Api.Dtos.Contact;
using DNAustria.Api.Dtos.Locations;
using DNAustria.Api.MapperExtensions;
using DNAustria.Logic.LocationsService;
using Microsoft.AspNetCore.Mvc;

namespace DNAustria.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class LocationsController(ILocationsService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LocationReplyDto>>> Get()
    {
        return Ok((await service
            .GetAvailableLocations())
            .Select(x => x.ToLocationReplyDto()));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LocationReplyDto?>> Get(int id)
    {
        var found = await service.GetLocationById(id);
        return found != null ?  Ok(found.ToLocationReplyDto()) : NotFound();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id)
    {
        var found = await service.DeleteLocation(id);
        return found ?  NoContent() : NotFound();
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LocationReplyDto>> Post([FromBody] CreateUpdateLocationDto request)
    {
        var addResult = await service.AddLocation(request.ToLocation());
        return addResult.item != null ?  Created("", addResult.item.ToLocationReplyDto()) : BadRequest(addResult.msg);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LocationReplyDto>> Put(int id, [FromBody] CreateUpdateLocationDto request)
    {
        var updateResult = await service.UpdateLocation(id, request.ToLocation());
        return updateResult.item != null ?  Ok(updateResult.item.ToLocationReplyDto()) : NotFound(updateResult.msg);
    }
}