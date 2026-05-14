using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DNAustria.Api.Dtos;

/// <summary>
/// DTO for LLM requests. Contains text or prompt supplied by the caller.
/// </summary>
public class LlmRequestDto
{
    public string? Text { get; set; }
    public string? Prompt { get; set; }

    public string? GetInputText() => string.IsNullOrWhiteSpace(Text) ? Prompt : Text;
}
