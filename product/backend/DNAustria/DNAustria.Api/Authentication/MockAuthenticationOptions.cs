namespace DNAustria.Api.Authentication;

public sealed class MockAuthenticationOptions
{
    public const string SectionName = "MockAuthentication";

    public List<MockAuthUserOptions> Users { get; set; } = [];
}
