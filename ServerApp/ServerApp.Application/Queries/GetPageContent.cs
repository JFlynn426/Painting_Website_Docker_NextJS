namespace ServerApp.Application.Queries;

using MediatR;
using ServerApp.Application.DTOs;

public record GetPageContent(string Address) : IRequest<PageContentDto>;