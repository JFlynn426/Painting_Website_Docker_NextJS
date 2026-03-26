namespace ServerApp.Domain.Events;

using ServerApp.Shared.Domain;

public record PaintingUpdatedEvent(Guid PaintingId, string Title, string CategorySlug) : IDomainEvent;