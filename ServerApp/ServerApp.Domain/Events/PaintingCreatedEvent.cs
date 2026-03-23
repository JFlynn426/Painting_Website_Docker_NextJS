namespace ServerApp.Domain.Events;

using ServerApp.Shared.Abstractions.Domain;

public record PaintingCreatedEvent(Guid PaintingId, string Title, string CategorySlug) : IDomainEvent;