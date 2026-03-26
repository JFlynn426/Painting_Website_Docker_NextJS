namespace ServerApp.Application.Queries;

using MediatR;
using ServerApp.Application.DTOs;

public record GetAllPaintings : IRequest<List<PaintingDto>>;