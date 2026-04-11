using System.DirectoryServices.Protocols;
using System.Globalization;
using System.Net;
using Microsoft.Extensions.Options;

namespace DNAustria.Api.Authentication;

public sealed class LdapAuthenticationService(IOptions<LdapOptions> options, ILogger<LdapAuthenticationService> logger)
    : IAuthenticationService, ILdapConnectivityProbe
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
            _logger.LogWarning(
                ex,
                "LDAP authentication failed for user {Username}. Host={Host}, Port={Port}, SearchBase={SearchBase}, BindDn={BindDn}, UserFilter={UserFilter}",
                username,
                _options.Host,
                _options.Port,
                _options.SearchBase,
                _options.BindDn,
                _options.UserFilter);
            return Task.FromResult<LdapUserInfo?>(null);
        }
    }

    public Task ProbeAsync(CancellationToken cancellationToken = default)
    {
        ExecuteWithConnection(_options.BindDn, _options.BindPassword, connection => connection.Bind());
        return Task.CompletedTask;
    }

    private SearchResultEntry? FindUser(string username)
    {
        if (string.IsNullOrWhiteSpace(_options.SearchBase))
        {
            return null;
        }

        var filter = string.Format(
            CultureInfo.InvariantCulture,
            _options.UserFilter,
            EscapeFilterValue(username));

        return ExecuteWithConnection(_options.BindDn, _options.BindPassword, connection =>
        {
            connection.Bind();

            var request = new SearchRequest(
                _options.SearchBase,
                filter,
                SearchScope.Subtree,
                RequestedAttributes);

            var response = (SearchResponse)connection.SendRequest(request);
            return response.Entries.Cast<SearchResultEntry>().FirstOrDefault();
        });
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
        ExecuteWithConnection(distinguishedName, password, connection => connection.Bind());
    }

    private T ExecuteWithConnection<T>(string? username, string? password, Func<LdapConnection, T> operation)
    {
        LdapException? lastException = null;

        foreach (var host in GetCandidateHosts())
        {
            try
            {
                using var connection = CreateConnection(host, username, password);
                return operation(connection);
            }
            catch (LdapException ex) when (IsServerUnavailable(ex))
            {
                lastException = ex;
                _logger.LogInformation(
                    ex,
                    "LDAP connection attempt failed against host {Host}:{Port}. Trying next candidate if available.",
                    host,
                    _options.Port);
            }
        }

        throw lastException ?? new LdapException("Unable to connect to the LDAP server.");
    }

    private void ExecuteWithConnection(string? username, string? password, Action<LdapConnection> operation)
    {
        ExecuteWithConnection<object?>(username, password, connection =>
        {
            operation(connection);
            return null;
        });
    }

    private LdapConnection CreateConnection(string host, string? username, string? password)
    {
        var identifier = new LdapDirectoryIdentifier(host, _options.Port, false, false);
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

    private IEnumerable<string> GetCandidateHosts()
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (!string.IsNullOrWhiteSpace(_options.Host) && seen.Add(_options.Host))
        {
            yield return _options.Host;
        }

        if (string.Equals(_options.Host, "localhost", StringComparison.OrdinalIgnoreCase))
        {
            if (seen.Add("127.0.0.1"))
            {
                yield return "127.0.0.1";
            }
        }
    }

    private static bool IsServerUnavailable(LdapException ex) =>
        ex.ErrorCode == 81
        || ex.ServerErrorMessage?.Contains("unavailable", StringComparison.OrdinalIgnoreCase) == true;

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
