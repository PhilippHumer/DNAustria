using Microsoft.AspNetCore.Mvc;

namespace DNAustria.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OrganisationsController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get() => Ok("fischlhammer macht pause!");
}