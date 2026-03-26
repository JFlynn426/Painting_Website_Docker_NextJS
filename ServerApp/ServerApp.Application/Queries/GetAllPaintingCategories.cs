namespace ServerApp.Application.Queries;

using MediatR;
using ServerApp.Application.DTOs;

public record GetAllPaintingCategories : IRequest<List<PaintingCategoryDto>>;