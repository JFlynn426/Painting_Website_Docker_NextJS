namespace ServerApp.Domain.Events;

using ServerApp.Shared.Domain;

public record AdminReactivatedEvent(Guid AdminId, string Email) : IDomainEvent;
