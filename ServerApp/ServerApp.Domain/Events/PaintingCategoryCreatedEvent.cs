namespace ServerApp.Domain.Events;

using ServerApp.Shared.Domain;

public record PaintingCategoryCreatedEvent(Guid PaintingCategoryId, string Name, string Slug) : IDomainEvent;
