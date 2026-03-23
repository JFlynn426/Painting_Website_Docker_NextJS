namespace ServerApp.Domain.Events;

using ServerApp.Shared.Abstractions.Domain;

public record PaintingDeletedEvent(Guid PaintingId, string Title) : IDomainEvent;