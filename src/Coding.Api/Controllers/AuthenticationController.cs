using Coding.DTOS.Auth;
using Coding.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Coding.Controllers;

[ApiController]
[Route("api/auth")]
[EnableRateLimiting("auth")]
public sealed class AuthenticationController : ControllerBase
{
    private const string RefreshTokenCookie = "refresh_token";
    private readonly IAuthenticationService authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        this.authenticationService = authenticationService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Register(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var response = await authenticationService.RegisterAsync(request, cancellationToken);
        SetRefreshTokenCookie(response.RefreshToken);
        return StatusCode(StatusCodes.Status201Created, ToPublicResponse(response));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await authenticationService.LoginAsync(request, cancellationToken);
        SetRefreshTokenCookie(response.RefreshToken);
        return Ok(ToPublicResponse(response));
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
    {
        if (!Request.Cookies.TryGetValue(RefreshTokenCookie, out var refreshToken))
            return Unauthorized();

        var response = await authenticationService.RefreshAsync(
            new RefreshTokenRequest { RefreshToken = refreshToken },
            cancellationToken);
        SetRefreshTokenCookie(response.RefreshToken);
        return Ok(ToPublicResponse(response));
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        if (Request.Cookies.TryGetValue(RefreshTokenCookie, out var refreshToken))
        {
            await authenticationService.RevokeAsync(
                new RefreshTokenRequest { RefreshToken = refreshToken },
                cancellationToken);
        }

        Response.Cookies.Delete(RefreshTokenCookie);
        return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("email-verification/request")]
    public async Task<IActionResult> RequestEmailVerification(
        EmailRequest request,
        CancellationToken cancellationToken)
    {
        await authenticationService.RequestEmailVerificationAsync(request, cancellationToken);
        return Accepted();
    }

    [AllowAnonymous]
    [HttpPost("email-verification/confirm")]
    public async Task<IActionResult> VerifyEmail(
        VerifyEmailRequest request,
        CancellationToken cancellationToken)
    {
        await authenticationService.VerifyEmailAsync(request, cancellationToken);
        return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("password/forgot")]
    public async Task<IActionResult> ForgotPassword(
        EmailRequest request,
        CancellationToken cancellationToken)
    {
        await authenticationService.RequestPasswordResetAsync(request, cancellationToken);
        return Accepted();
    }

    [AllowAnonymous]
    [HttpPost("password/reset")]
    public async Task<IActionResult> ResetPassword(
        ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        await authenticationService.ResetPasswordAsync(request, cancellationToken);
        return NoContent();
    }

    private void SetRefreshTokenCookie(string token)
    {
        Response.Cookies.Append(RefreshTokenCookie, token, new CookieOptions
        {
            HttpOnly = true,
            Secure = !HttpContext.RequestServices
                .GetRequiredService<IWebHostEnvironment>().IsDevelopment(),
            SameSite = SameSiteMode.Strict,
            Path = "/api/auth",
            MaxAge = TimeSpan.FromDays(30),
            IsEssential = true
        });
    }

    private static object ToPublicResponse(AuthResponse response) => new
    {
        response.AccessToken,
        response.AccessTokenExpiresAt,
        response.User
    };
}
