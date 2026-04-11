namespace DNAustria.Api.Authentication;

public sealed class MockAuthUserOptions
{
    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string? Email { get; set; }
}
