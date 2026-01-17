using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("server/api/public/events")]
public class PublicExportController(IEventService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var data = await service.PublicExportAsync();
        return Ok(new { events = data });
    }
}
