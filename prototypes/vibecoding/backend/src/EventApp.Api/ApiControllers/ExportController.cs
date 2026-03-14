using Microsoft.AspNetCore.Mvc;
using EventApp.Core.Interfaces;

namespace EventApp.Api.ApiControllers
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
