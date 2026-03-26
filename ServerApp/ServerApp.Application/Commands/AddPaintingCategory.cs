namespace ServerApp.Application.Commands;

using MediatR;
using ServerApp.Application.DTOs;

public record AddPaintingCategory(
    string Name,
    string? Description
) : IRequest<PaintingCategoryCreatedResult>;