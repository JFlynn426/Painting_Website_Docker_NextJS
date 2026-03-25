using Microsoft.AspNetCore.Mvc;
using ServerApp.Application.Commands;
using ServerApp.Application.DTOs;
using ServerApp.Application.Queries;
using ServerApp.Shared.Abstractions.Commands;
using ServerApp.Shared.Abstractions.Queries;

namespace ServerApp.Api.Controllers;

/// <summary>
/// Controller for managing paintings with CQRS pattern.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PaintingsController : BaseController
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;

    public PaintingsController(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
    }

    /// <summary>
    /// Gets all paintings.
    /// </summary>
    /// <returns>List of all paintings.</returns>
    [HttpGet]
    public async Task<ActionResult<List<PaintingDto>>> GetAll()
    {
        // Note: GetAllPaintings query not yet implemented, returning empty list as placeholder
        return Ok(new List<PaintingDto>());
    }

    /// <summary>
    /// Gets a painting by its slug.
    /// </summary>
    /// <param name="slug">The painting slug.</param>
    /// <returns>The painting if found, otherwise NotFound.</returns>
    [HttpGet("{slug}")]
    public async Task<ActionResult<PaintingDto>> GetBySlug(string slug)
    {
        var query = new GetPainting(slug);
        var result = await _queryDispatcher.QueryAsync(query);
        return OkOrNotFound(result);
    }

    /// <summary>
    /// Gets paintings by category slug.
    /// </summary>
    /// <param name="categorySlug">The category slug.</param>
    /// <returns>Painting category with its paintings.</returns>
    [HttpGet("category/{categorySlug}")]
    public async Task<ActionResult<PaintingCategoryWithPaintingsDto>> GetByCategory(string categorySlug)
    {
        var query = new GetPaintingCategoryWithPaintings(categorySlug);
        var result = await _queryDispatcher.QueryAsync(query);
        return OkOrNotFound(result);
    }

    /// <summary>
    /// Adds a new painting.
    /// </summary>
    /// <param name="command">The add painting command from the Application layer.</param>
    /// <returns>201 Created status with the created painting.</returns>
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddPainting command)
    {
        await _commandDispatcher.DispatchAsync(command);
        return CreatedAtAction(nameof(GetBySlug), new { slug = "new" }, new { message = "Painting created successfully" });
    }

    /// <summary>
    /// Deletes a painting by its ID.
    /// </summary>
    /// <param name="command">The delete painting command from the Application layer.</param>
    /// <returns>204 No Content status.</returns>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] DeletePainting command)
    {
        await _commandDispatcher.DispatchAsync(command);
        return NoContent();
    }
}