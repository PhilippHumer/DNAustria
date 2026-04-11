namespace DNAustria.Api.Authentication;

public sealed class LdapOptions
{
    public const string SectionName = "Ldap";

    public string Host { get; set; } = string.Empty;

    public int Port { get; set; } = 389;

    public bool UseSsl { get; set; }

    public bool IgnoreCertificateErrors { get; set; }

    public string? SearchBase { get; set; }

    public string UserFilter { get; set; } = "(uid={0})";

    public string? UserDnPattern { get; set; }

    public string? BindDn { get; set; }

    public string? BindPassword { get; set; }

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(Host)
        && (!string.IsNullOrWhiteSpace(UserDnPattern) || !string.IsNullOrWhiteSpace(SearchBase));
}
