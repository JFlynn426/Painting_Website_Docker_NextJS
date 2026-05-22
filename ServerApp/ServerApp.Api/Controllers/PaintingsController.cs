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

    /// <summary>
    /// Updates a painting by its ID.
    /// </summary>
    /// <param name="id">The painting ID.</param>
    /// <param name="request">The update painting request from the Application layer.</param>
    /// <param name="idempotencyKey">Optional idempotency key for safe retries.</param>
    /// <returns>200 OK with command completion response.</returns>
    [ServerApp.Api.Filters.AdminAuthorized]
    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<CommandCompletionResponse>> Update(
        [FromRoute] Guid id,
        [FromBody] UpdatePaintingRequest request,
        [FromHeader(Name = "X-Idempotency-Key")] string? idempotencyKey)
    {
        var adminId = (Guid)HttpContext.Items["AdminId"]!;
        var command = new UpdatePainting(id, request.Name, request.Description, request.ImageUrl,
            request.ThumbnailUrl, request.Slug, request.CategoryId, request.Width, request.Height,
            request.Depth, request.Year, request.Price, request.IsAvailable, request.IsNew,
            request.IsCarouselPainting, adminId, idempotencyKey);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Assigns a painting to a different category.
    /// </summary>
    /// <param name="paintingId">The painting ID.</param>
    /// <param name="categoryId">The target category ID.</param>
    /// <param name="idempotencyKey">Optional idempotency key for safe retries.</param>
    /// <returns>200 OK with command completion response.</returns>
    [ServerApp.Api.Filters.AdminAuthorized]
    [HttpPatch("{paintingId:guid}/category/{categoryId:guid}")]
    public async Task<ActionResult<CommandCompletionResponse>> AssignCategory(
        [FromRoute] Guid paintingId,
        [FromRoute] Guid categoryId,
        [FromHeader(Name = "X-Idempotency-Key")] string? idempotencyKey)
    {
        var adminId = (Guid)HttpContext.Items["AdminId"]!;
        var command = new AssignPaintingCategory(paintingId, categoryId, adminId, idempotencyKey);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Bulk reassigns paintings to different categories.
    /// </summary>
    /// <param name="request">The reassign paintings request containing painting-to-category mappings.</param>
    /// <param name="idempotencyKey">Optional idempotency key for safe retries.</param>
    /// <returns>200 OK with command completion response.</returns>
    [ServerApp.Api.Filters.AdminAuthorized]
    [HttpPost("reassign")]
    public async Task<ActionResult<CommandCompletionResponse>> ReassignPaintings(
        [FromBody] ReassignPaintingsRequest request,
        [FromHeader(Name = "X-Idempotency-Key")] string? idempotencyKey)
    {
        var adminId = (Guid)HttpContext.Items["AdminId"]!;
        var command = new ReassignPaintings(request.PaintingIdToCategoryId, adminId, idempotencyKey);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}