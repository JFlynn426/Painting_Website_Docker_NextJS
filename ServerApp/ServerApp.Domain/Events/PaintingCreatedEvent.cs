namespace ServerApp.Domain.Events;

using ServerApp.Shared.Domain;

public record PaintingCreatedEvent(Guid PaintingId, string Title, string CategorySlug) : IDomainEvent;