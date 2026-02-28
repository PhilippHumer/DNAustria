using System;
using System.Collections.Generic;
using System.Text;

namespace DNAustria.Domain;

public class OpenAIParameters
{
    public string? ApiKey { get; set; }
    public string? Model { get; set; }
    public string? BaseUrl { get; set; }
    public int MaxTokens { get; set; } = 100;
    public double Temperature { get; set; } = 0.7;
    public double TopP { get; set; } = 1.0;
}
