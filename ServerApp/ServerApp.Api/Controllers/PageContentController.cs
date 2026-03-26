using Microsoft.AspNetCore.Mvc;
using MediatR;
using ServerApp.Application.Commands;
using ServerApp.Application.DTOs;
using ServerApp.Application.Queries;

namespace ServerApp.Api.Controllers;

/// <summary>
/// Controller for managing page content with CQRS pattern.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PageContentController : BaseController
{
    private readonly IMediator _mediator;

    public PageContentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets page content by its address.
    /// </summary>
    /// <param name="address">The page address.</param>
    /// <returns>The page content if found, otherwise NotFound.</returns>
    [HttpGet("{address}")]
    public async Task<ActionResult<PageContentDto>> Get(string address)
    {
        var result = await _mediator.Send(new GetPageContent(address));
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
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(Get), new { address = result.Address }, result);
    }

    /// <summary>
    /// Deletes page content by its address.
    /// </summary>
    /// <param name="command">The delete page content command from the Application layer.</param>
    /// <returns>204 No Content status.</returns>
    [HttpDelete("{address}")]
    public async Task<IActionResult> Delete([FromRoute] DeletePageContent command)
    {
        await _mediator.Send(command);
        return NoContent();
    }
}