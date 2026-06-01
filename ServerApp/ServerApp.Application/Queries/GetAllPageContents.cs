namespace ServerApp.Application.Queries;

using MediatR;
using ServerApp.Application.DTOs;

public record GetAllPageContents : IRequest<List<PageContentDto>>;