namespace ServerApp.Domain.Events;

using ServerApp.Shared.Abstractions.Domain;

public record PaintingUpdatedEvent(Guid PaintingId, string Title, string CategorySlug) : IDomainEvent;