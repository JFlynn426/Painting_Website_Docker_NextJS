namespace ServerApp.Application.Commands;

using MediatR;
using ServerApp.Application.DTOs;

public record LoginWithYahoo(
    string Code,
    string State
) : IRequest<AuthResponse>;
