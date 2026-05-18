namespace ServerApp.Application.Commands;

using MediatR;
using ServerApp.Application.DTOs;

public record LoginWithGoogle(
    string Code,
    string State
) : IRequest<GoogleAuthResponse>;
