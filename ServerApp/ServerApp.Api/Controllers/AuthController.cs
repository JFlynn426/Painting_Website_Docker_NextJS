using Microsoft.AspNetCore.Mvc;
using MediatR;
using ServerApp.Application.Commands;
using ServerApp.Application.DTOs;
using ServerApp.Application.Queries;
using ServerApp.Application.Services;

namespace ServerApp.Api.Controllers;

/// <summary>
/// Controller for admin authentication with Google OAuth.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IGoogleAuthService _googleAuthService;

    public AuthController(IMediator mediator, IGoogleAuthService googleAuthService)
    {
        _mediator = mediator;
        _googleAuthService = googleAuthService;
    }

    /// <summary>
    /// Gets the Google OAuth authorization URL to redirect the user.
    /// </summary>
    /// <returns>Object containing the authorization URL.</returns>
    [HttpGet("google/url")]
    public ActionResult<Dictionary<string, string>> GetGoogleAuthorizationUrl()
    {
        var url = _googleAuthService.GetAuthorizationUrl();
        return Ok(new Dictionary<string, string> { { "url", url } });
    }

    /// <summary>
    /// Handles the Google OAuth callback and exchanges the authorization code for a JWT token.
    /// </summary>
    /// <param name="request">The OAuth callback request containing the authorization code.</param>
    /// <returns>JWT token and admin user information.</returns>
    [HttpPost("google/callback")]
    public async Task<ActionResult<GoogleAuthResponse>> LoginWithGoogle([FromBody] GoogleAuthRequest request)
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
    /// Gets the current authenticated admin user.
    /// </summary>
    /// <param name="adminId">The admin user ID from the JWT token claim.</param>
    /// <returns>Admin user information if authenticated, otherwise NotFound.</returns>
    [HttpGet("me")]
    public async Task<ActionResult<AdminUserDto>> GetCurrentUser([FromHeader(Name = "X-Admin-Id")] Guid adminId)
    {
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
}
