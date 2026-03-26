namespace ServerApp.Application.Queries;

using MediatR;
using ServerApp.Application.DTOs;

public record GetPainting(string Slug) : IRequest<PaintingDto>;