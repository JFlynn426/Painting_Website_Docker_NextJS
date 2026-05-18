namespace ServerApp.Domain.Events;

using ServerApp.Shared.Domain;

public record AdminLoggedInEvent(Guid AdminId, string Email) : IDomainEvent;
