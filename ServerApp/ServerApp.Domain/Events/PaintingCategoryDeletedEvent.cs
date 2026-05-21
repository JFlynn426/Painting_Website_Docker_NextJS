namespace ServerApp.Domain.Events;

using ServerApp.Shared.Domain;

public record PaintingCategoryDeletedEvent(Guid PaintingCategoryId, string Name) : IDomainEvent;
