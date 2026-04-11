namespace DNAustria.Api.Authentication;

public interface ILdapConnectivityProbe
{
    Task ProbeAsync(CancellationToken cancellationToken = default);
}
