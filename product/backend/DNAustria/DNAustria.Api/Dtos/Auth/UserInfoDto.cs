namespace DNAustria.Api.Dtos.Auth;

public sealed record UserInfoDto(
    string Username,
    string DisplayName,
    string? Email
);
