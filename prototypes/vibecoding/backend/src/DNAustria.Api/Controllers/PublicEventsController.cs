using DNAustria.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace DNAustria.Api.Controllers;

[ApiController]
[Route("api/public/events")]
public class PublicEventsController : ControllerBase
{
    private readonly EventService _service;

    public PublicEventsController(EventService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetPublic(CancellationToken ct)
    {
        var result = await _service.GetPublicAsync(ct);
        return Ok(result);
    }
}
