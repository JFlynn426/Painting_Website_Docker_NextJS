namespace ServerApp.Application.Commands;

using ServerApp.Shared.Abstractions.Commands;

public record DeletePageContent(string Address) : ICommand;