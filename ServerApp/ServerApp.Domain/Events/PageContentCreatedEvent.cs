namespace ServerApp.Domain.Events;

using ServerApp.Shared.Domain;

public record PageContentCreatedEvent(Guid PageContentId, string Address) : IDomainEvent;
