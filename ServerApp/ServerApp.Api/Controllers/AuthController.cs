using Microsoft.AspNetCore.Mvc;
using MediatR;
using ServerApp.Application.Commands;
using ServerApp.Application.DTOs;
using ServerApp.Application.Queries;
using ServerApp.Application.Services;

namespace ServerApp.Api.Controllers;

/// <summary>
/// Controller for admin authentication with Google and Yahoo OAuth.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IGoogleAuthService _googleAuthService;
    private readonly IYahooAuthService _yahooAuthService;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthController(IMediator mediator, IGoogleAuthService googleAuthService, IYahooAuthService yahooAuthService, IJwtTokenService jwtTokenService)
    {
        _mediator = mediator;
        _googleAuthService = googleAuthService;
        _yahooAuthService = yahooAuthService;
        _jwtTokenService = jwtTokenService;
    }

    /// <summary>
    /// Gets the Google OAuth authorization URL to redirect the user.
    /// </summary>
    /// <returns>Object containing the authorization URL.</returns>
    [HttpGet("google/url")]
    public ActionResult<Dictionary<string, string>> GetGoogleAuthorizationUrl()
    {
        var (url, state) = _googleAuthService.GetAuthorizationUrl();
        return Ok(new Dictionary<string, string> { { "url", url }, { "state", state } });
    }

    /// <summary>
    /// Handles the Google OAuth callback and exchanges the authorization code for a JWT token.
    /// </summary>
    /// <param name="request">The OAuth callback request containing the authorization code.</param>
    /// <returns>JWT token and admin user information.</returns>
    [HttpPost("google/callback")]
    public async Task<ActionResult<AuthResponse>> LoginWithGoogle([FromBody] AuthRequest request)
    {
        var command = new LoginWithGoogle(request.Code, request.State);
        var result = await _mediator.Send(command);

        // Set httpOnly cookie with the JWT token
        Response.Cookies.Append("admin_token", result.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddHours(1)
        });

        return Ok(result);
    }

    /// <summary>
    /// Gets the Yahoo OAuth authorization URL to redirect the user.
    /// </summary>
    /// <returns>Object containing the authorization URL.</returns>
    [HttpGet("yahoo/url")]
    public ActionResult<Dictionary<string, string>> GetYahooAuthorizationUrl()
    {
        var (url, state) = _yahooAuthService.GetAuthorizationUrl();
        return Ok(new Dictionary<string, string> { { "url", url }, { "state", state } });
    }

    /// <summary>
    /// Handles the Yahoo OAuth callback and exchanges the authorization code for a JWT token.
    /// </summary>
    /// <param name="request">The OAuth callback request containing the authorization code.</param>
    /// <returns>JWT token and admin user information.</returns>
    [HttpPost("yahoo/callback")]
    public async Task<ActionResult<AuthResponse>> LoginWithYahoo([FromBody] AuthRequest request)
    {
        var command = new LoginWithYahoo(request.Code, request.State);
        var result = await _mediator.Send(command);

        // Set httpOnly cookie with the JWT token
        Response.Cookies.Append("admin_token", result.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddHours(1)
        });

        return Ok(result);
    }

    /// <summary>
    /// Gets the current authenticated admin user by validating the httpOnly cookie.
    /// </summary>
    /// <returns>Admin user information if authenticated, otherwise Unauthorized.</returns>
    [HttpGet("me")]
    public async Task<ActionResult<AdminUserDto>> GetCurrentUser()
    {
        var token = Request.Cookies["admin_token"];
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized(new { error = "Unauthorized", message = "Missing authentication token." });
        }

        var principal = _jwtTokenService.ValidateToken(token);
        if (principal == null)
        {
            return Unauthorized(new { error = "Unauthorized", message = "Invalid or expired token." });
        }

        var adminIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(adminIdClaim) || !Guid.TryParse(adminIdClaim, out var adminId))
        {
            return Unauthorized(new { error = "Unauthorized", message = "Invalid token claims." });
        }

        var result = await _mediator.Send(new GetCurrentUser(adminId));
        return OkOrNotFound(result);
    }

    /// <summary>
    /// Logs out the current admin user by clearing the authentication cookie.
    /// </summary>
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("admin_token");
        return Ok(new { message = "Logged out successfully" });
    }

    /// <summary>
    /// Updates an admin user by its ID.
    /// </summary>
    /// <param name="id">The admin user ID.</param>
    /// <param name="request">The update admin user request from the Application layer.</param>
    /// <param name="idempotencyKey">Optional idempotency key for safe retries.</param>
    /// <returns>200 OK with command completion response.</returns>
    [ServerApp.Api.Filters.AdminAuthorized]
    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<CommandCompletionResponse>> UpdateAdminUser(
        [FromRoute] Guid id,
        [FromBody] UpdateAdminUserRequest request,
        [FromHeader(Name = "X-Idempotency-Key")] string? idempotencyKey)
    {
        var adminId = (Guid)HttpContext.Items["AdminId"]!;
        var command = new UpdateAdminUser(id, request.DisplayName, request.PictureUrl, request.IsActive, adminId, idempotencyKey);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
