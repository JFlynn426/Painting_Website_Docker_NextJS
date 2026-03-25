using Microsoft.AspNetCore.Mvc;
using ServerApp.Application.Commands;
using ServerApp.Application.DTOs;
using ServerApp.Application.Queries;
using ServerApp.Shared.Abstractions.Commands;
using ServerApp.Shared.Abstractions.Queries;

namespace ServerApp.Api.Controllers;

/// <summary>
/// Controller for managing painting categories with CQRS pattern.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PaintingCategoriesController : BaseController
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;

    public PaintingCategoriesController(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
    }

    /// <summary>
    /// Gets all painting categories.
    /// </summary>
    /// <returns>List of all painting categories.</returns>
    [HttpGet]
    public async Task<ActionResult<List<PaintingCategoryDto>>> GetAll()
    {
        var query = new GetAllPaintingCategories();
        var result = await _queryDispatcher.QueryAsync(query);
        return Ok(result);
    }

    /// <summary>
    /// Gets a painting category by its slug with its paintings.
    /// </summary>
    /// <param name="slug">The category slug.</param>
    /// <returns>The painting category with its paintings if found, otherwise NotFound.</returns>
    [HttpGet("{slug}")]
    public async Task<ActionResult<PaintingCategoryWithPaintingsDto>> GetBySlug(string slug)
    {
        var query = new GetPaintingCategoryWithPaintings(slug);
        var result = await _queryDispatcher.QueryAsync(query);
        return OkOrNotFound(result);
    }

    /// <summary>
    /// Adds a new painting category.
    /// </summary>
    /// <param name="command">The add painting category command from the Application layer.</param>
    /// <returns>201 Created status with the created category.</returns>
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddPaintingCategory command)
    {
        await _commandDispatcher.DispatchAsync(command);
        return CreatedAtAction(nameof(GetBySlug), new { slug = "new" }, new { message = "Painting category created successfully" });
    }

    /// <summary>
    /// Deletes a painting category by its ID.
    /// </summary>
    /// <param name="command">The delete painting category command from the Application layer.</param>
    /// <returns>204 No Content status.</returns>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] DeletePaintingCategory command)
    {
        await _commandDispatcher.DispatchAsync(command);
        return NoContent();
    }
}