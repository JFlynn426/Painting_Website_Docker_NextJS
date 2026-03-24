namespace ServerApp.Application.Commands;

using ServerApp.Shared.Abstractions.Commands;

public record AddPageContent(
    string Address,
    string Title,
    string Content
) : ICommand;