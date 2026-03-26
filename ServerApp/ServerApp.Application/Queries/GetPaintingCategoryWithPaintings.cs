namespace ServerApp.Application.Queries;

using MediatR;
using ServerApp.Application.DTOs;

public record GetPaintingCategoryWithPaintings(string Slug) : IRequest<PaintingCategoryWithPaintingsDto>;