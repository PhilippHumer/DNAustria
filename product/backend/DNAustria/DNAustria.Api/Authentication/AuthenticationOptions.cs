namespace DNAustria.Api.Authentication;

public sealed class AuthenticationOptions
{
    public const string SectionName = "Authentication";

    public string Mode { get; set; } = AuthenticationMode.Ldap;
}
