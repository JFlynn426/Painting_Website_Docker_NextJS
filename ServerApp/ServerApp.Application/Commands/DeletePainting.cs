namespace ServerApp.Application.Commands;

using ServerApp.Shared.Abstractions.Commands;

public record DeletePainting(Guid Id) : ICommand;