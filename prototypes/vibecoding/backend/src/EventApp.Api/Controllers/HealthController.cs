using Microsoft.AspNetCore.Mvc;

namespace EventApp.Api.Controllers
{
    [ApiController]
    [Route("health")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { status = "Healthy", timestamp = System.DateTime.UtcNow });
        }
    }
}
using Microsoft.AspNetCore.Mvc;

namespace EventApp.Api.Controllers
{
    [ApiController]
    [Route("health")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { status = "Healthy", timestamp = System.DateTime.UtcNow });
        }
    }
}
using Microsoft.AspNetCore.Mvc;

namespace EventApp.Api.Controllers;











}    }        return Ok(new { status = "Healthy", timestamp = System.DateTime.UtcNow });    {    public IActionResult Get()    [HttpGet]{public class HealthController : ControllerBase[Route("health")]n[ApiController]