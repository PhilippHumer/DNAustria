using DNAustria.API.Models;
using DNAustria.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DNAustria.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportController : ControllerBase
    {
        private readonly LlmService _llmService;

        public ImportController(LlmService llmService)
        {
            _llmService = llmService;
        }

        [HttpPost("text")]
        public async Task<ActionResult<Event>> ImportFromText([FromBody] ImportRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return BadRequest("Text is required");
            }

            var extractedEvent = await _llmService.ExtractEventFromTextAsync(request.Text);
            return Ok(extractedEvent);
        }
    }

    public class ImportRequest
    {
        public string Text { get; set; }
    }
}
