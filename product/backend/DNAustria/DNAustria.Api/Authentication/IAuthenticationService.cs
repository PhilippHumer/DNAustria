namespace DNAustria.Api.Authentication;

public interface IAuthenticationService
{
    Task<LdapUserInfo?> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default);
}
