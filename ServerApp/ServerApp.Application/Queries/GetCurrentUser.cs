namespace ServerApp.Application.Queries;

using MediatR;
using ServerApp.Application.DTOs;

public record GetCurrentUser(
    Guid AdminId
) : IRequest<AdminUserDto?>;
