namespace ServerApp.Api.Filters;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using ServerApp.Application.Services;

/// <summary>
/// Action filter that validates the admin JWT cookie before executing mutation actions.
/// Resolves IJwtTokenService from IServiceProvider at runtime.
/// </summary>
public class AdminAuthorizedAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var jwtTokenService = context.HttpContext.RequestServices.GetRequiredService<IJwtTokenService>();
        var cookie = context.HttpContext.Request.Cookies["admin_token"];

        if (string.IsNullOrEmpty(cookie))
        {
            context.Result = new UnauthorizedObjectResult(new { error = "Unauthorized", message = "Missing authentication token." });
            return;
        }

        var principal = jwtTokenService.ValidateToken(cookie);
        if (principal == null)
        {
            context.Result = new UnauthorizedObjectResult(new { error = "Unauthorized", message = "Invalid or expired token." });
            return;
        }

        // Store admin ID in HttpContext for downstream use
        var adminIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(adminIdClaim) && Guid.TryParse(adminIdClaim, out var adminId))
        {
            context.HttpContext.Items["AdminId"] = adminId;
        }
        else
        {
            context.Result = new UnauthorizedObjectResult(new { error = "Unauthorized", message = "Invalid token claims." });
        }
    }
}
