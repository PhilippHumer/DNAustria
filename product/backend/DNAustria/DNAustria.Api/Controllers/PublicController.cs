using DNAustria.Api.Dtos.Events;
using DNAustria.Logic.Events;
using Microsoft.AspNetCore.Mvc;
using DNAustria.Api.MapperExtensions;
using Microsoft.AspNetCore.Authorization;

namespace DNAustria.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("[controller]")]
public class PublicController(ILogger<PublicController> logger, IEventLogic eventLogic) : ControllerBase
{
    private readonly ILogger<PublicController> _logger = logger;

    [HttpGet("events")]
    public async Task<IActionResult> GetEvents()
    {
        var events = await eventLogic.HandlePublishEventsAsync();
        return Ok(new { events = events.ToPublicDtos() });
    }
}