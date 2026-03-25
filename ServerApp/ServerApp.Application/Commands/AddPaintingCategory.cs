namespace ServerApp.Application.Commands;

using ServerApp.Shared.Abstractions.Commands;

public record AddPaintingCategory(
    string Name,
    string? Description
) : ICommand;