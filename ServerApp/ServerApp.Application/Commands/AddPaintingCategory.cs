namespace ServerApp.Application.Commands;

using ServerApp.Shared.Abstractions.Commands;

public record AddPaintingCategory(
    Guid Id,
    string Name,
    string? Description
) : ICommand;