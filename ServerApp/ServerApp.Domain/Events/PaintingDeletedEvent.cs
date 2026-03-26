namespace ServerApp.Domain.Events;

using ServerApp.Shared.Domain;

public record PaintingDeletedEvent(Guid PaintingId, string Title) : IDomainEvent;