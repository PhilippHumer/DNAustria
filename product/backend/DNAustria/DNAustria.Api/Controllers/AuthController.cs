using System.Security.Claims;
using DNAustria.Api.Authentication;
using DNAustria.Api.Dtos.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DNAustria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(DNAustria.Api.Authentication.IAuthenticationService authenticationService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType<LoginResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
    {
        var user = await authenticationService.AuthenticateAsync(request.Username, request.Password, cancellationToken);
        if (user is null)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Authentication failed",
                Detail = "Invalid username or password.",
                Status = StatusCodes.Status401Unauthorized
            });
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Username),
            new(ClaimTypes.Name, user.DisplayName),
            new("username", user.Username)
        };

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
        }

        var principal = new ClaimsPrincipal(new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme));

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = false,
                AllowRefresh = true,
                IssuedUtc = DateTimeOffset.UtcNow
            });

        return Ok(new LoginResponseDto(ToDto(user)));
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return NoContent();
    }

    [HttpGet("me")]
    [ProducesResponseType<UserInfoDto>(StatusCodes.Status200OK)]
    public IActionResult Me()
    {
        return Ok(new UserInfoDto(
            User.FindFirstValue("username") ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty,
            User.Identity?.Name ?? User.FindFirstValue("username") ?? string.Empty,
            User.FindFirstValue(ClaimTypes.Email)));
    }

    private static UserInfoDto ToDto(LdapUserInfo user) => new(
        user.Username,
        user.DisplayName,
        user.Email);
}
