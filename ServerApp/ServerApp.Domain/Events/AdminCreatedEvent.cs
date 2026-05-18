namespace ServerApp.Domain.Events;

using ServerApp.Shared.Domain;

public record AdminCreatedEvent(Guid AdminId, string Email) : IDomainEvent;
