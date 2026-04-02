namespace ServerApp.Application.Queries;

using ServerApp.Application.DTOs;
using MediatR;

/// <summary>
/// Query to get all new paintings (where IsNew=true).
/// </summary>
public record GetNewPaintings : IRequest<List<PaintingDto>>;