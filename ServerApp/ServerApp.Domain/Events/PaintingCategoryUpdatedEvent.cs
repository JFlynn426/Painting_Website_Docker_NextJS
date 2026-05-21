namespace ServerApp.Domain.Events;

using ServerApp.Shared.Domain;

public record PaintingCategoryUpdatedEvent(Guid PaintingCategoryId, string Name, string Slug) : IDomainEvent;
