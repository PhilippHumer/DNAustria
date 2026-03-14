using Microsoft.AspNetCore.Mvc;
using EventApp.Core.Interfaces;

namespace EventApp.Api.Controllers
{
    [ApiController]
    [Route("api/export")]
    public class ExportController : ControllerBase
    {
        private readonly IExportService _exportService;

        public ExportController(IExportService exportService)
        {
            _exportService = exportService;
        }

        [HttpGet("status")]
        public IActionResult Status()
        {
            return StatusCode(501, new { error = "Export service not implemented" });
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using EventApp.Core.Interfaces;

namespace EventApp.Api.Controllers
{
    [ApiController]
    [Route("api/export")]
    public class ExportController : ControllerBase
    {
        private readonly IExportService _exportService;

        public ExportController(IExportService exportService)
        {
            _exportService = exportService;
        }

        [HttpGet("status")]
        public IActionResult Status()
        {
            return StatusCode(501, new { error = "Export service not implemented" });
        }
    }
}
using Microsoft.AspNetCore.Mvc;

namespace EventApp.Api.Controllers;
















}    }        return StatusCode(501, new { error = "Export service not implemented" });    {    public IActionResult Status()
n    [HttpGet("status")]    }        _exportService = exportService;    {
n    public ExportController(EventApp.Core.Interfaces.IExportService exportService)    private readonly EventApp.Core.Interfaces.IExportService _exportService;{public class ExportController : ControllerBase[Route("api/export")]n[ApiController]