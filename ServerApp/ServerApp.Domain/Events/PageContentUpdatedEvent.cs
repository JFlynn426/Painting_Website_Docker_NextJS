namespace ServerApp.Domain.Events;

using ServerApp.Shared.Domain;

public record PageContentUpdatedEvent(Guid PageContentId, string Address) : IDomainEvent;
