using System.DirectoryServices.Protocols;
using System.Globalization;
using System.Net;
using Microsoft.Extensions.Options;

namespace DNAustria.Api.Authentication;

public sealed class LdapAuthenticationService(IOptions<LdapOptions> options, ILogger<LdapAuthenticationService> logger)
    : IAuthenticationService
{
    private static readonly string[] RequestedAttributes = ["cn", "displayName", "mail"];
    private readonly LdapOptions _options = options.Value;
    private readonly ILogger<LdapAuthenticationService> _logger = logger;

    public Task<LdapUserInfo?> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return Task.FromResult<LdapUserInfo?>(null);
        }

        if (!_options.IsConfigured)
        {
            throw new InvalidOperationException("LDAP authentication is not configured.");
        }

        try
        {
            var userEntry = FindUser(username);
            var distinguishedName = userEntry?.DistinguishedName ?? BuildUserDn(username);
            BindUser(distinguishedName, password);

            var displayName = userEntry?.GetAttributeValue("displayName")
                ?? userEntry?.GetAttributeValue("cn")
                ?? username;
            var email = userEntry?.GetAttributeValue("mail");

            return Task.FromResult<LdapUserInfo?>(new LdapUserInfo(username, distinguishedName, displayName, email));
        }
        catch (LdapException ex)
        {
            _logger.LogWarning(ex, "LDAP authentication failed for user {Username}", username);
            return Task.FromResult<LdapUserInfo?>(null);
        }
    }

    private SearchResultEntry? FindUser(string username)
    {
        if (string.IsNullOrWhiteSpace(_options.SearchBase))
        {
            return null;
        }

        using var connection = CreateConnection(_options.BindDn, _options.BindPassword);
        connection.Bind();

        var filter = string.Format(
            CultureInfo.InvariantCulture,
            _options.UserFilter,
            EscapeFilterValue(username));

        var request = new SearchRequest(
            _options.SearchBase,
            filter,
            SearchScope.Subtree,
            RequestedAttributes);

        var response = (SearchResponse)connection.SendRequest(request);
        return response.Entries.Cast<SearchResultEntry>().FirstOrDefault();
    }

    private string BuildUserDn(string username)
    {
        if (string.IsNullOrWhiteSpace(_options.UserDnPattern))
        {
            throw new LdapException("Unable to determine LDAP distinguished name for the supplied user.");
        }

        return string.Format(
            CultureInfo.InvariantCulture,
            _options.UserDnPattern,
            EscapeDistinguishedNameValue(username));
    }

    private void BindUser(string distinguishedName, string password)
    {
        using var connection = CreateConnection(distinguishedName, password);
        connection.Bind();
    }

    private LdapConnection CreateConnection(string? username, string? password)
    {
        var identifier = new LdapDirectoryIdentifier(_options.Host, _options.Port);
        var credential = string.IsNullOrWhiteSpace(username)
            ? null
            : new NetworkCredential(username, password);

        var connection = credential is null
            ? new LdapConnection(identifier)
            : new LdapConnection(identifier, credential, AuthType.Basic);

        connection.Timeout = TimeSpan.FromSeconds(10);
        connection.SessionOptions.ProtocolVersion = 3;
        connection.SessionOptions.SecureSocketLayer = _options.UseSsl;

        if (_options.IgnoreCertificateErrors)
        {
            connection.SessionOptions.VerifyServerCertificate += (_, _) => true;
        }

        return connection;
    }

    private static string EscapeFilterValue(string value) => value
        .Replace("\\", "\\5c", StringComparison.Ordinal)
        .Replace("*", "\\2a", StringComparison.Ordinal)
        .Replace("(", "\\28", StringComparison.Ordinal)
        .Replace(")", "\\29", StringComparison.Ordinal)
        .Replace("\0", "\\00", StringComparison.Ordinal);

    private static string EscapeDistinguishedNameValue(string value) => value
        .Replace("\\", "\\\\", StringComparison.Ordinal)
        .Replace(",", "\\,", StringComparison.Ordinal)
        .Replace("+", "\\+", StringComparison.Ordinal)
        .Replace("\"", "\\\"", StringComparison.Ordinal)
        .Replace("<", "\\<", StringComparison.Ordinal)
        .Replace(">", "\\>", StringComparison.Ordinal)
        .Replace(";", "\\;", StringComparison.Ordinal)
        .Replace("=", "\\=", StringComparison.Ordinal)
        .Replace("#", "\\#", StringComparison.Ordinal);
}

file static class SearchResultEntryExtensions
{
    public static string? GetAttributeValue(this SearchResultEntry entry, string attributeName)
    {
        if (!entry.Attributes.Contains(attributeName))
        {
            return null;
        }

        return entry.Attributes[attributeName]?[0]?.ToString();
    }
}
