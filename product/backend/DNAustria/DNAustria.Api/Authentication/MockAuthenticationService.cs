using Microsoft.Extensions.Options;

namespace DNAustria.Api.Authentication;

public sealed class MockAuthenticationService(
    IOptions<MockAuthenticationOptions> options,
    ILogger<MockAuthenticationService> logger) : IAuthenticationService
{
    private readonly MockAuthenticationOptions _options = options.Value;
    private readonly ILogger<MockAuthenticationService> _logger = logger;

    public Task<LdapUserInfo?> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        var user = _options.Users.FirstOrDefault(candidate =>
            string.Equals(candidate.Username, username, StringComparison.OrdinalIgnoreCase)
            && candidate.Password == password);

        if (user is null)
        {
            _logger.LogInformation("Mock authentication rejected user {Username}", username);
            return Task.FromResult<LdapUserInfo?>(null);
        }

        var displayName = string.IsNullOrWhiteSpace(user.DisplayName) ? user.Username : user.DisplayName;
        var distinguishedName = $"mock:{user.Username}";
        return Task.FromResult<LdapUserInfo?>(new LdapUserInfo(user.Username, distinguishedName, displayName, user.Email));
    }
}
