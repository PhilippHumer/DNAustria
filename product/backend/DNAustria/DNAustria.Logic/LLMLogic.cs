using DNAustria.Domain;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DNAustria.Logic;

public class LLMLogic : ILLMLogic, IDisposable
{
    private readonly OpenAIParameters _settings;
    private readonly HttpClient _httpClient;
    private bool _disposed;

    public LLMLogic(IConfiguration configuration)
    {
        var section = configuration.GetSection("OpenAI");
        if (!section.Exists())
            throw new ArgumentException("OpenAI configuration section is missing.");

        _settings = new OpenAIParameters
        {
            ApiKey = section["ApiKey"],
            Model = section["Model"],
            BaseUrl = section["BaseUrl"],
            MaxTokens = int.TryParse(section["MaxTokens"], NumberStyles.Integer, CultureInfo.InvariantCulture, out var mt) ? mt : 100,
            Temperature = double.TryParse(section["Temperature"], NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var temp) ? temp : 0.7,
            TopP = double.TryParse(section["TopP"], NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var tp) ? tp : 1.0
        };

        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
            throw new ArgumentException("OpenAI API key is missing.");

        _httpClient = new HttpClient { BaseAddress = new Uri(_settings.BaseUrl ?? "https://api.openai.com/") };
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ApiKey);
    }

    /// <summary>
    /// Convenience static helper to construct and run LLMLogic from a caller (e.g. Program.Main).
    /// Builds an instance from the provided <see cref="IConfiguration"/> and returns the chat result.
    /// </summary>
    public static async Task<string?> StartAsync(IConfiguration configuration, string prompt)
    {
        using var logic = new LLMLogic(configuration);
        return await logic.GetChatCompletionAsync(prompt);
    }

    /// <summary>
    /// Convenience static helper to construct and run LLMLogic from an OpenAIParameters instance.
    /// </summary>
    public static async Task<string?> StartAsync(OpenAIParameters settings, string prompt)
    {
        using var logic = new LLMLogic(settings);
        return await logic.GetChatCompletionAsync(prompt);
    }

    // Convenience constructor for simple testing without IConfiguration
    public LLMLogic(OpenAIParameters settings)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));

        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
            throw new ArgumentException("OpenAI API key is missing.");

        _httpClient = new HttpClient { BaseAddress = new Uri(_settings.BaseUrl ?? "https://api.openai.com/") };
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ApiKey);
    }

    public async Task<string?> GetChatCompletionAsync(string prompt)
    {
        var request = new
        {
            model = _settings.Model,
            messages = new[] { new { role = "user", content = prompt } },
            temperature = _settings.Temperature,
            max_tokens = _settings.MaxTokens,
            top_p = _settings.TopP
        };

        var json = JsonSerializer.Serialize(request);

        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("v1/chat/completions", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"Error calling OpenAI: {responseBody}");

        using var document = JsonDocument.Parse(responseBody);
        var result = document
            .RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return result;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _httpClient.Dispose();
            _disposed = true;
        }
    }
}

