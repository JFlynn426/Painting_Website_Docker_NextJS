using Microsoft.AspNetCore.Mvc;

namespace ServerApp.Api.Controllers;

/// <summary>
/// Abstract base controller that provides common helper methods for API controllers.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// Returns Ok(result) if the result is not null, otherwise returns NotFound().
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="result">The result to return. If null, returns NotFound().</param>
    /// <returns>Ok(result) if result is not null, otherwise NotFound().</returns>
    protected ActionResult<TResult> OkOrNotFound<TResult>(TResult? result)
    {
        return result is null ? NotFound() : Ok(result);
    }
}