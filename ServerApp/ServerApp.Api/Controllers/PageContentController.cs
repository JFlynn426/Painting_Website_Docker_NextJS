using Microsoft.AspNetCore.Mvc;
using ServerApp.Application.Commands;
using ServerApp.Application.DTOs;
using ServerApp.Application.Queries;
using ServerApp.Shared.Abstractions.Commands;
using ServerApp.Shared.Abstractions.Queries;

namespace ServerApp.Api.Controllers;

/// <summary>
/// Controller for managing page content with CQRS pattern.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PageContentController : BaseController
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;

    public PageContentController(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
    }

    /// <summary>
    /// Gets page content by its address.
    /// </summary>
    /// <param name="address">The page address.</param>
    /// <returns>The page content if found, otherwise NotFound.</returns>
    [HttpGet("{address}")]
    public async Task<ActionResult<PageContentDto>> Get(string address)
    {
        var query = new GetPageContent(address);
        var result = await _queryDispatcher.QueryAsync(query);
        return OkOrNotFound(result);
    }

    /// <summary>
    /// Adds new page content.
    /// </summary>
    /// <param name="command">The add page content command from the Application layer.</param>
    /// <returns>201 Created status with the created page content.</returns>
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddPageContent command)
    {
        await _commandDispatcher.DispatchAsync(command);
        return CreatedAtAction(nameof(Get), new { address = command.Address }, new { message = "Page content created successfully" });
    }

    /// <summary>
    /// Deletes page content by its address.
    /// </summary>
    /// <param name="command">The delete page content command from the Application layer.</param>
    /// <returns>204 No Content status.</returns>
    [HttpDelete("{address}")]
    public async Task<IActionResult> Delete([FromRoute] DeletePageContent command)
    {
        await _commandDispatcher.DispatchAsync(command);
        return NoContent();
    }
}