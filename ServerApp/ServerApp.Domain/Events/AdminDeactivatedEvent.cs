namespace ServerApp.Domain.Events;

using ServerApp.Shared.Domain;

public record AdminDeactivatedEvent(Guid AdminId, string Email) : IDomainEvent;
