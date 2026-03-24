namespace ServerApp.Application.Commands;

using ServerApp.Shared.Abstractions.Commands;

public record DeletePaintingCategory(Guid Id) : ICommand;