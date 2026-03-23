namespace ServerApp.Application.Queries;

using ServerApp.Shared.Abstractions.Queries;
using ServerApp.Application.DTOs;

public record GetPainting(Guid Id) : IQuery<PaintingDto>;