namespace DNAustria.Api.Authentication;

public sealed record LdapUserInfo(
    string Username,
    string DistinguishedName,
    string DisplayName,
    string? Email
);
