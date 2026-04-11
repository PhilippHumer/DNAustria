using Microsoft.Extensions.Options;

namespace DNAustria.Api.Authentication;

public sealed class LdapStartupProbeHostedService(
    IServiceProvider serviceProvider,
    IOptions<AuthenticationOptions> authenticationOptions,
    IOptions<LdapOptions> ldapOptions,
    ILogger<LdapStartupProbeHostedService> logger) : IHostedService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly AuthenticationOptions _authenticationOptions = authenticationOptions.Value;
    private readonly LdapOptions _ldapOptions = ldapOptions.Value;
    private readonly ILogger<LdapStartupProbeHostedService> _logger = logger;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!string.Equals(_authenticationOptions.Mode, AuthenticationMode.Ldap, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (!_ldapOptions.IsConfigured)
        {
            _logger.LogWarning(
                "LDAP startup probe skipped because LDAP is not fully configured. Host={Host}, Port={Port}, SearchBase={SearchBase}, UserDnPattern={UserDnPattern}",
                _ldapOptions.Host,
                _ldapOptions.Port,
                _ldapOptions.SearchBase,
                _ldapOptions.UserDnPattern);
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var probe = scope.ServiceProvider.GetRequiredService<ILdapConnectivityProbe>();

        try
        {
            await probe.ProbeAsync(cancellationToken);
            _logger.LogInformation(
                "LDAP startup probe succeeded. Host={Host}, Port={Port}, SearchBase={SearchBase}, BindDn={BindDn}",
                _ldapOptions.Host,
                _ldapOptions.Port,
                _ldapOptions.SearchBase,
                _ldapOptions.BindDn);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "LDAP startup probe failed. Host={Host}, Port={Port}, SearchBase={SearchBase}, BindDn={BindDn}. The app will keep running, but LDAP logins will fail until the directory is reachable.",
                _ldapOptions.Host,
                _ldapOptions.Port,
                _ldapOptions.SearchBase,
                _ldapOptions.BindDn);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
