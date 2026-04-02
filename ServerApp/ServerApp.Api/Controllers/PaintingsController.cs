using Microsoft.AspNetCore.Mvc;
using MediatR;
using ServerApp.Application.Commands;
using ServerApp.Application.DTOs;
using ServerApp.Application.Queries;

namespace ServerApp.Api.Controllers;

/// <summary>
/// Controller for managing paintings with CQRS pattern.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PaintingsController : BaseController
{
    private readonly IMediator _mediator;

    public PaintingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets all paintings.
    /// </summary>
    /// <returns>List of all paintings.</returns>
    [HttpGet]
    public async Task<ActionResult<List<PaintingDto>>> GetAll()
    {
        var result = await _mediator.Send(new GetAllPaintings());
        return Ok(result);
    }

    /// <summary>
    /// Gets a painting by its slug.
    /// </summary>
    /// <param name="slug">The painting slug.</param>
    /// <returns>The painting if found, otherwise NotFound.</returns>
    [HttpGet("{slug}")]
    public async Task<ActionResult<PaintingDto>> GetBySlug(string slug)
    {
        var result = await _mediator.Send(new GetPainting(slug));
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
        var result = await _mediator.Send(new GetPaintingCategoryWithPaintings(categorySlug));
        return OkOrNotFound(result);
    }

    /// <summary>
    /// Gets all new paintings (where IsNew=true).
    /// </summary>
    /// <returns>List of new paintings.</returns>
    [HttpGet("new")]
    public async Task<ActionResult<List<PaintingDto>>> GetNewPaintings()
    {
        var result = await _mediator.Send(new GetNewPaintings());
        return Ok(result);
    }

    /// <summary>
    /// Adds a new painting.
    /// </summary>
    /// <param name="command">The add painting command from the Application layer.</param>
    /// <returns>201 Created status with the created painting.</returns>
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddPainting command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetBySlug), new { slug = result.Slug }, result);
    }

    /// <summary>
    /// Deletes a painting by its ID.
    /// </summary>
    /// <param name="command">The delete painting command from the Application layer.</param>
    /// <returns>204 No Content status.</returns>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] DeletePainting command)
    {
        await _mediator.Send(command);
        return NoContent();
    }
}